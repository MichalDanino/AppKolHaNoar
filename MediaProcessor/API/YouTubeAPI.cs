using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Channels;
using System.Threading.Tasks;
using DataAccess;
using DotNetEnv;
using DTO;
using static DTO.Enums;
using Google;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using static System.Net.WebRequestMethods;

namespace MediaProcessor.API;
public class YouTubeAPI
{

    static int MaxVidowToDownload;
    public static DBHandler bHandler;
    /* צריך להוסיפ כאן עדכון נתוני הווידיאו ב DB*/
    public YouTubeAPI()
    {
        MaxVidowToDownload = 5;
    }

    /// <summary>
    /// Connected to youTube Api and get list of new video in specipic channel
    /// </summary>
    /// <param name="ChannelID">Id Channel</param>
    /// <returns> list of new video</returns>
    public static async Task<SearchListResponse> fetchChannelVideosByAPI(string ChannelID)
    {
        SearchListResponse searchResponse = new SearchListResponse();
        try
        {

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyChVkppCJxUXR9UcE_BOo9FxiYUS - Kjnok",/* AppConfig.apiKeyYT,*/
                ApplicationName = AppConfig.apiProjectNameYT
            });
            // Playlist ID data request details
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.ChannelId = ChannelID;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date; // לפי תאריך פרסום
            searchRequest.MaxResults = 5;
            searchRequest.Type = "video";
            SearchListResponse channelResponse = searchRequest.ExecuteAsync().Result;



            return channelResponse;

        }
        catch (GoogleApiException apiEx)
        {
            Console.WriteLine("error in-YouTube API:");
            Console.WriteLine($"code error: {apiEx.Error.Code}");
            Console.WriteLine($"message: {apiEx.Error.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("erro global");
            Console.WriteLine(ex.Message);
        }
        return null;

    }

    public static async Task<eStatus> YouTubeMediaMain(string channelID)
    {
        // eStatus status = eStatus.SUCCESS;
        //need update 
        if (channelID.Contains("@"))
        {
            channelID = await GetChannelIdByNameAsync(channelID);

        }
        if (channelID == "")
            return eStatus.FAILED;

        if (channelID == "false")
            return eStatus.APIQuota;
        SearchListResponse playlistItemListResponse = await fetchChannelVideosByAPI(channelID);
        await DownloadVideoAsAudio(channelID, playlistItemListResponse);
        return eStatus.SUCCESS;
    }
    /// <summary>
    ///  Retrieves the duration of a video in minutes by executing yt-dlp (or youtube-dl) 
    /// </summary>
    /// <param name="videoUrl">The URL of the video from which the duration is to be retrieved</param>
    /// <param name="youtubeDL">>The full path to the yt-dlp (or youtube-dl) executable file</param>
    /// <returns></returns>
    public static async Task<int> durationVideo(string videoUrl, string youtubeDL)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = youtubeDL,
            Arguments = $"--print-json --no-check-certificate --geo-bypass {videoUrl}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string jsonOutput = await reader.ReadToEndAsync();
                Console.WriteLine(jsonOutput);
                JObject videoData = JObject.Parse(jsonOutput);

                // get video duration as second
                return videoData["duration"]?.Value<int>() ?? 0;


            }
        }
        return 0;
    }
    public static  async Task<eStatus> DownloadVideoAsAudio(string channelId, SearchListResponse videoList)
    {
        eStatus status = eStatus.SUCCESS;
        AppConfig.exceptions.Clear();
        if (videoList != null)
        {
            var youtubeDl = new YoutubeDL
            {
                YoutubeDLPath = @"C:\yt-dlgANDffmpeg\yt-dlg\yt-dlp.exe",
                FFmpegPath = @"C:\yt-dlgANDffmpeg\yt-dlg\ffmpeg.exe"
            };
            foreach (SearchResult video in videoList.Items)
            {

                if (video.Id.Kind == "youtube#video") // verify that is video
                {

                    string videoTitle = video.Snippet.Title.Replace(" ", "_");
                    string videoId = video.Id.VideoId;
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";


                    await durationVideo(videoUrl, youtubeDl.YoutubeDLPath);
                    // path folder output
                    string outputPath = Path.Combine(AppConfig.rootURL + "Downloads", $"{videoTitle}.%(ext)s");

                    // Setting download options including disabling SSL
                    var options = new OptionSet
                    {
                        Output = outputPath,
                        ExtractAudio = true,
                        AudioFormat = AudioConversionFormat.Mp3,
                        NoCheckCertificates = true //disabling SSL
                    };

                    // Audio download only
                    var result = youtubeDl.RunWithOptions(videoUrl, options).Result;
                    if (!result.Success)
                    {

                        status = eStatus.NETWORKERROR;
                        return status;
                    }
                     status =  await SaveVideoDetails(video, outputPath);
                }
            }
            return status;
        }

        status = eStatus.FAILED;
        return status;

    }

    /// <summary>
    /// update the following parameter of video: channelID, Duration and call Extension to upload this file
    /// </summary>
    /// <param name="videoUrl">video url inorder to get ID channel</param>
    /// <param name="duration">video duration </param>
    public void InsertVideoDetails(string videoUrl, int duration)
    {
        string channelId = videoUrl.Split("=")[1].Replace("\"", "");
        SQLiteAccess sQLiteAccess = new SQLiteAccess(AppConfig.NameDBFile);

        //Retrieve the channel entity along with its linked call extension information
        List<ChannelExtension> channelExtension = sQLiteAccess.GetDBSet<ChannelExtension>($"WHERE UserID ={channelId}");

        //update DB
        VideoDetails videoDetails = new VideoDetails()
        {
            VideoDetails_VideoID = channelId,
            VideoDetails_Duration = duration,
            VideoDetails_ExtensionMapping = channelExtension[0].ChannelExtension_Long
        };
        // if video lass then 10 minets
        if (duration <= 10)
        {
            ;
            videoDetails.VideoDetails_ExtensionMapping = channelExtension[0].ChannelExtension_Short;
        }
    }
    public static async Task<string> GetChannelIdByNameAsync(string channelName)
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = AppConfig.apiKeyYT,
            ApplicationName = AppConfig.apiProjectNameYT

        });

        try
        {
            var channelRequest = youtubeService.Channels.List("id");
            channelRequest.ForHandle = channelName;  // שם הערוץ בפורמט @Handle
            var channelResponse = channelRequest.ExecuteAsync().Result;
            if (channelResponse.Items.Count > 0)
            {
                var channel = channelResponse.Items[0];
                return channel.Id;
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("quotaExceeded"))
            {
                return "false";
            }
        }
        return "";
    }
    public static async Task<eStatus> DownloadVideoAsText(string channelId)
    {
        eStatus status = eStatus.SUCCESS;
        AppConfig.exceptions.Clear();

        var youtubeDl = new YoutubeDL
        {
            YoutubeDLPath = @"C:\yt-dlgANDffmpeg\yt-dlg\yt-dlp.exe",
            FFmpegPath = @"C:\yt-dlgANDffmpeg\yt-dlg\ffmpeg.exe"
        };



        string videoTitle = "ggg";
        string videoId = "5lGUEB3mkhc";
        string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
        //string videoUrl = $"https://www.youtube.com/watch?v=5lGUEB3mkhc";


        // await durationVideo(videoUrl, youtubeDl.YoutubeDLPath);
        // path folder output
        string outputPath = Path.Combine(AppConfig.rootURL + "Downloads", $"{videoTitle}.%(ext)s");

        // Setting download options including disabling SSL
        var options = new OptionSet
        {
            Output = outputPath,
            ExtractAudio = true,
            AudioFormat = AudioConversionFormat.Mp3,
            NoCheckCertificates = true //disabling SSL
        };

        // Audio download only
        var result = youtubeDl.RunWithOptions(videoUrl, options).Result;
        if (!result.Success)
        {
            //AppConfig.exceptions.Add(new GenericMessage()
            //{
            //    MessageTitle = "נתקל בבעיה בהורדת הסרטון",
            //    MessageContent = "נתיב :" + videoUrl+ " " + result.ErrorOutput,
            //    subMessageMessage =  "אנא וודא שהאינטרנט מחובר או שהסרטון נפתח במחשב"

            //});
            status = eStatus.NETWORKERROR;
            return status;
        }

        return status;

    }

    public static async Task<eStatus> SaveVideoDetails(SearchResult video, string videoPath)
    {
        DBHandler dBHandler = new DBHandler();
        List<ChannelExtension> channel = dBHandler.GetDBSet<ChannelExtension>("* WHERE ChannelExtension_ChannelID = @ID", new { ID = video.Id.VideoId });
        if (channel != null)
        {
            int duration = await durationVideo($"https://www.youtube.com/watch?v={video.Id.VideoId}", AppConfig.YouTubeDLPath);
            VideoDetails videoDetails = new VideoDetails()
            {
                VideoDetails_VideoID = video.Id.VideoId,
                VideoDetails_ExtensionMapping = duration > 10 ? channel[0].ChannelExtension_Long : channel[0].ChannelExtension_Short,
                VideoDetails_videoPath = videoPath

            };

            AppStaticParameter.videoDownLoad.Add(videoDetails);
            return eStatus.SUCCESS; 
        }
        return eStatus.FAILED;



    }
}



