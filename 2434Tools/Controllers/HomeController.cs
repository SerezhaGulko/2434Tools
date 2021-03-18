using _2434Tools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace _2434Tools.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            const String API_KEY        = "API_KEY";
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY,
                ApplicationName = this.GetType().ToString()
            });

            // ChannelList Example
            /*
            const String ChannelId1     = "UCHVXbQzkl3rDfsXWo8xi2qw";
            const String ChannelId2     = "UCt5-0i4AVHXaWJrL8Wql3mw";
            var channel_request = youtubeService.Channels.List("snippet,statistics");
            channel_request.Id = ChannelId1 + "," + ChannelId2;
            channel_request.MaxResults = 50;
            var resp = await channel_request.ExecuteAsync();
            foreach(var channel in resp.Items)
            {
                channel.Snippet.Thumbnails;
                channel.Snippet.Title;
                channel.Statistics.SubscriberCount;
                channel.Statistics.VideoCount;
                channel.Statistics.ViewCount;
            }
            ViewData["resp"] = resp;*/

            /*
            // PlayList list 
            const string ChannelId = "UC1uv2Oq6kNxgATlCiez59hw";
            string uploadsPlaylistId = null;
            // Get Uploads playlist id
            var channelRequest = youtubeService.Channels.List("contentDetails");
            channelRequest.Id = ChannelId;
            var resp = await channelRequest.ExecuteAsync();
            foreach(var channel in resp.Items) { uploadsPlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads; }

            var playlistRequest = youtubeService.PlaylistItems.List("contentDetails,snippet");
            playlistRequest.PlaylistId = uploadsPlaylistId;
            playlistRequest.MaxResults = 50;
            var respPlaylist = await playlistRequest.ExecuteAsync();
            foreach(var playlistItem in respPlaylist.Items)
            {
                //playlistItem.ContentDetails.VideoId;
                //playlistItem.Snippet.Thumbnails
                //playlistItem.Snippet.Title
            }
            ViewData["resp"] = respPlaylist;
            */
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
