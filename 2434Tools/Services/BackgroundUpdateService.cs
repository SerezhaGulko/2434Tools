using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _2434Tools.Data;
using _2434Tools.Models;
using Microsoft.Extensions.DependencyInjection;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml;

namespace _2434Tools.Services
{
    public class BackgroundUpdateService : IHostedService, IDisposable
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly ILogger<BackgroundUpdateService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _LiverUpdateTimer;
        private Timer _FeedUpdateTimer;
        private Timer _VideoUpdateTimer;
        private Boolean isUpdatingFeed = false;

        public BackgroundUpdateService(IServiceScopeFactory scopeFactory, ILogger<BackgroundUpdateService> logger)
        {
            this._scopeFactory = scopeFactory;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Update Service is starting...");

            // Change TimeSpan.Zero to another value, if you want to start timers with a delay
            _LiverUpdateTimer = new Timer(UpdateLivers, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60.0d * Variables.UpdateLiverInterval));
            _FeedUpdateTimer = new Timer(UpdateFeed, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(60.0d * Variables.UpdateFeedInterval));
            _VideoUpdateTimer = new Timer(UpdateVideos, null, TimeSpan.FromSeconds(30.0d),
                TimeSpan.FromSeconds(60.0d * Variables.UpdateVideoInterval));

            return Task.CompletedTask;
        }

        private async void UpdateLivers(object state)
        {
            _logger.LogInformation("Background service is updating livers...");
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Variables.API_KEY,
                ApplicationName = this.GetType().ToString()
            });
            using (var scope = _scopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var Livers = await _db.Livers.ToListAsync();
                int k_groups = (Livers.Count + 49) / 50;
                List<Task<Google.Apis.YouTube.v3.Data.ChannelListResponse>>
                    responses = new List<Task<Google.Apis.YouTube.v3.Data.ChannelListResponse>>(k_groups);
                for(int i = 0; i < k_groups; i++) responses.Add(null);
                for(int i = 0; i < k_groups; i++)
                {
                    int _start = i * 50, _end = Math.Min(_start + 50, Livers.Count);
                    var channel_request = youtubeService.Channels.List("snippet,statistics,brandingSettings");
                    String ChannelIds = Livers[_start].ChannelId;
                    for(int j = _start + 1; j < _end; j++)
                    {
                        ChannelIds += "," + Livers[j].ChannelId;
                    }
                    channel_request.Id = ChannelIds;
                    channel_request.MaxResults = 50;
                    responses[i] = channel_request.ExecuteAsync();
                }
                for(int i = 0; i < k_groups; i++)
                {
                    try
                    {
                        foreach (var channel in (await responses[i]).Items)
                        {
                            var Liver = Livers.Single(_liver => _liver.ChannelId == channel.Id);
                            Liver.ChannelName = channel.Snippet.Title;
                            Liver.Description = channel.Snippet.Description;
                            Liver.Subscribers = (uint)channel.Statistics.SubscriberCount;
                            Liver.Views       = (ulong)channel.Statistics.ViewCount;
                            if(channel.Snippet.Thumbnails.Standard != null)
                            {
                                Liver.PictureURL = channel.Snippet.Thumbnails.Standard.Url;
                            } else if(channel.Snippet.Thumbnails.High != null)
                            {
                                Liver.PictureURL = channel.Snippet.Thumbnails.High.Url;
                            } else if(channel.Snippet.Thumbnails.Medium != null)
                            {
                                Liver.PictureURL = channel.Snippet.Thumbnails.Medium.Url;
                            } else if(channel.Snippet.Thumbnails.Default__ != null)
                            {
                                Liver.PictureURL = channel.Snippet.Thumbnails.Default__.Url;
                            }
                            Liver.ThumbURL  = channel.Snippet.Thumbnails.Default__.Url;
                            if(channel.BrandingSettings.Image?.BannerExternalUrl != null)
                            {
                                Liver.BannerURL = channel.BrandingSettings.Image.BannerExternalUrl;
                            }
                        }
                    } catch(Exception e)
                    {
                        _logger.LogInformation($"An exception has occured in UpdateLivers. Mesasge = {e.Message}");
                    }
                }
                try
                {
                    await _db.SaveChangesAsync();
                } catch(Exception ex)
                {
                    _logger.LogInformation($"Database update failed. Reason: {ex.Message}");
                }
            }
            _logger.LogInformation("Background service has finished updating livers.");
        }

        private async void UpdateFeed(object state)
        {
            if (!isUpdatingFeed)
            {
                isUpdatingFeed = true;
                _logger.LogInformation("Background service is updating feeds...");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var updateQueue = await _db.Livers.Where(_liver => !_liver.Graduated)
                                                .OrderBy(_liver => _liver.FeedChecked)
                                                .Take(Variables.UpdateFeedBatch).ToListAsync();
                    List<Task<List<String>>> responses = new List<Task<List<String>>>(updateQueue.Count);
                    for (int i = 0; i < updateQueue.Count; i++)
                    {
                        responses.Add(GetVideosIds(updateQueue[i].ChannelId));
                    }
                    DateTime updateTime = DateTime.UtcNow;
                    for (int i = 0; i < updateQueue.Count; i++)
                    {
                        List<String> response = await responses[i];
                        if (response == null || response.Count == 0)
                        {
                            if (response != null && response.Count == 0)
                                _logger.LogInformation("No videoIds were returned in UpdateFeed");
                            goto next_iteration;
                        }
                        var found_videos = await _db.Videos
                            .Where(_video =>
                                _video.LiverId == updateQueue[i].Id &&
                                response.Contains(_video.Id)
                            ).ToListAsync();

                        foreach (var video in found_videos)
                        {
                            if (video.Status == VideoStatus.Unavailable)
                                video.Status = VideoStatus.Undefined;
                        }
                        foreach (var videoId in response.Where(_videoId => found_videos.All(_found => _found.Id != _videoId)))
                        {
                            _db.Add(new Video() { Id = videoId, LiverId = updateQueue[i].Id, Status = VideoStatus.Undefined });
                        }
                    next_iteration:;
                        updateQueue[i].FeedChecked = updateTime;
                    }
                    try
                    {
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Database update failed. Reason: {ex.Message}");
                    }
                }
                _logger.LogInformation("Background service has finished updating feeds.");
                isUpdatingFeed = false;
            }
        }

        private async void UpdateVideos(object state)
        {
            _logger.LogInformation("Background service is updating videos...");
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Variables.API_KEY,
                ApplicationName = this.GetType().ToString()
            });
            using (var scope = _scopeFactory.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var videos = await _db.Videos.Where(_v =>   _v.Status == VideoStatus.Live 
                                                        ||  _v.Status == VideoStatus.Upcoming 
                                                        ||  _v.Status == VideoStatus.Undefined).ToListAsync();

                Int32 k_groups = (videos.Count + 49) / 50;
                List<Task<Google.Apis.YouTube.v3.Data.VideoListResponse>>
                    requests = new List<Task<Google.Apis.YouTube.v3.Data.VideoListResponse>>(k_groups);
                for (int i = 0; i < k_groups; i++) requests.Add(null);
                for (int i = 0; i < k_groups; i++)
                {
                    int _start = 50 * i, _end = Math.Min(_start + 50, videos.Count);
                    var videos_request = youtubeService.Videos.List("snippet,statistics,contentDetails,liveStreamingDetails,status");
                    String VideoIds = videos[_start].Id;
                    for (int j = _start + 1; j < _end; j++)
                    {
                        VideoIds += "," + videos[j].Id;
                    }
                    videos_request.MaxResults = 50;
                    videos_request.Id = VideoIds;
                    requests[i] = videos_request.ExecuteAsync();
                }
                for (int i = 0; i < k_groups; i ++)
                {
                    try
                    {
                        foreach (var response in (await requests[i]).Items)
                        {
                            String id = response.Id;
                            var video = videos.Single(_video => _video.Id == id);
                            video.Title = response.Snippet.Title;
                            video.Description = response.Snippet.Description;
                            video.Published = response.Snippet.PublishedAt?.ToUniversalTime();
                            if (response.ContentDetails.Duration != null) 
                            {
                                video.Duration = (uint)(XmlConvert.ToTimeSpan(response.ContentDetails.Duration)).TotalSeconds;
                            }
                            video.Views = (uint)response.Statistics.ViewCount;
                            if(response.LiveStreamingDetails != null)
                            {
                                if (response.LiveStreamingDetails.ActualStartTime != null)
                                {
                                    video.LiveStartTime = response.LiveStreamingDetails.ActualStartTime.Value.ToUniversalTime();
                                    if (response.LiveStreamingDetails.ActualEndTime != null)
                                    {
                                        video.LiveEndTime = response.LiveStreamingDetails.ActualEndTime.Value.ToUniversalTime();
                                        video.Status = VideoStatus.Finished;
                                    } else
                                        video.Status = VideoStatus.Live;

                                    video.Viewers = (uint)(response.LiveStreamingDetails.ConcurrentViewers ?? 0);
                                    video.PeakViewers = Math.Max(video.Viewers, video.PeakViewers);

                                } else {
                                    // YouTube is sometimes insane and LiveStreamingDetails is not 
                                    if(response.LiveStreamingDetails.ScheduledStartTime != null)
                                    {
                                        video.LiveStartTime = response.LiveStreamingDetails.ScheduledStartTime.Value.ToUniversalTime();
                                        video.Status = VideoStatus.Upcoming;
                                    }
                                }
                            } else {
                                video.Status = VideoStatus.Finished;
                            }

                            var thumbs = response.Snippet.Thumbnails;
                            if(thumbs.Maxres != null)
                            {
                                video.PictureUrl = thumbs.Maxres.Url;
                            } else if(thumbs.Standard != null)
                            {
                                video.PictureUrl = thumbs.Standard.Url;
                            } else if(thumbs.High != null)
                            {
                                video.PictureUrl = thumbs.High.Url;
                            } else if(thumbs.Medium != null)
                            {
                                video.PictureUrl = thumbs.Medium.Url;
                            } else if(thumbs.Default__ != null)
                            {
                                video.PictureUrl = thumbs.Default__.Url;
                            }
                            if(video.PictureUrl == null && response.Status.PrivacyStatus != "private")
                            {
                                video.Status = VideoStatus.Undefined;
                            }
                        }
                    } catch(Exception ex)
                    {
                        _logger.LogInformation($"An exception has occured in UpdateVideos. Mesasge = {ex.Message}");
                    }
                }
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Database update failed. Reason: {ex.Message}");
                }
            }
            _logger.LogInformation("Background service has finished updating videos.");
        }

        private async Task<List<String>> GetVideosIds(String channelId)
        {
            var response = await _client.GetAsync(
                $"https://www.youtube.com/feeds/videos.xml?channel_id={channelId}");
            if(response.IsSuccessStatusCode)
            {
                Regex regx = new Regex("<yt:videoId>(.+?)</yt:videoId>");
                String contentAsText = await response.Content.ReadAsStringAsync();
                return regx.Matches(contentAsText).Select(_match => _match.Groups[1].Value).ToList();
            } else
            {
                _logger.LogInformation($"Could not make for {channelId}. Reason = {response.StatusCode} : {response.ReasonPhrase}");
                return null;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Update Service is stopping...");

            _LiverUpdateTimer?.Change(Timeout.Infinite, 0);
            _FeedUpdateTimer?.Change(Timeout.Infinite, 0);
            _VideoUpdateTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _logger.LogInformation("Disposing Background Update Service...");

            _LiverUpdateTimer?.Dispose();
            _FeedUpdateTimer?.Dispose();
            _VideoUpdateTimer?.Dispose();
        }
    }
}
