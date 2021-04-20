function UpdateLiveVideos() {
    var form = $('#__AntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    console.log(token);
    $.ajax({
        type: "POST",
        url: "/Home/LiveVideos",
        data: {
            "__RequestVerificationToken": token
        },
        success: function (data) {
            // Remove Old
            $("div[VideoId]").each((idx, _v) => {
                if (data.find(video => { return video.videoId == _v.getAttribute("videoid") })) {
                    // This video is still live
                } else {
                    if (!_v.classList.contains('selected')) {
                        // Not selected, can safely remove
                        _v.remove();
                    }
                }
            });
            // Add new
            data.forEach(_v => {
                var topLevelResult = $(`div[videoid=${_v.videoId}]`);
                if (topLevelResult.length == 0) {
                    // Is new
                    $('#appendix').append($(`<div class="video-selection" videoid="${_v.videoId}"><img class="list-liver-image" src="${_v.creatorThumbId}"/></div>`))
                }
            });
        }
    });
}
setInterval(UpdateLiveVideos, 120000);