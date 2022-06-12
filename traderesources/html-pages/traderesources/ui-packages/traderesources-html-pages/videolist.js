$(document).ready(
        function ($) {
            var playStream = function (url) {
                var $videoContainer = $(".video-container");
                $videoContainer.empty();

                var video = '<video controls="controls" name="media" width="100%">' +
                    '<source src="' + url + '" type="video/mp4">' +

                    '</video>';


                $videoContainer.append(video);

                var link = '<div style="padding-top:4px">' +
                    '<a style="font-family:Tahoma;font-size:12px;text-decoration:none;color:#36C;" href="' + url + '">Прямая ссылка на видео</a>' +
                    '</div>';

                $videoContainer.append(link);
            };

            $(".play-video-link").click(
                function () {
            $(".play-video-link-selected").removeClass("play-video-link-selected");
                    $(this).addClass("play-video-link-selected");
                    var streamUrl = $(this).attr("video-url");
                    playStream(streamUrl);
                }
            );
        }
    );
