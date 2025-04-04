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
using Sprache;
using System.Text.RegularExpressions;

namespace MediaProcessor.API;
public class YouTubeAPI
{

    static int MaxVidowToDownload;
    public static MultiSourceDataService bHandler;
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
                ApiKey =  AppConfig.apiKeyYT,
                ApplicationName = AppConfig.apiProjectNameYT
            });
            // Playlist ID data request details
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.ChannelId = ChannelID;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date; // לפי תאריך פרסום
            searchRequest.Type = "video";
            // Get the result from the last date the channel ran until today.
            searchRequest.PublishedAfterDateTimeOffset = DateTime.Parse(YouTubeMediaHandler.GetLastUpdateExtension(ChannelID).ToString()).ToUniversalTime();

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
       
        channelID = await GetChannelIdByNameAsync(channelID);

        
        if (channelID == "")
                return eStatus.FAILED;

        if (channelID == "false")
                return eStatus.APIQuota;

        SearchListResponse playlistItemListResponse = await fetchChannelVideosByAPI(channelID);

        if (playlistItemListResponse.Items != null && playlistItemListResponse.Items.Count > 0)
             { 
                 //filter the video by list of bidden words
                 playlistItemListResponse.Items = playlistItemListResponse.Items
                                                           .Where(item => !AppStaticParameter.forbiddenWords
                                                           .Any(word => item.Snippet.Title
                                                           .Contains(word, StringComparison.OrdinalIgnoreCase)))

                                                           .ToList();
            return await DownloadVideoAsAudio(channelID, playlistItemListResponse);
                 

             }
        return eStatus.NotHaveNews;

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
                string jsonOutput = reader.ReadToEndAsync().Result;
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
                YoutubeDLPath =AppConfig.YouTubeDLPath,
                FFmpegPath = AppConfig.FFmpegPath
            };
            foreach (SearchResult video in videoList.Items)
            {

                if (video.Id.Kind == "youtube#video") // verify that is video
                {
                    //Remove all symbols, numbers, and letters from the title.
                    video.Snippet.Title = Regex.Replace(video.Snippet.Title, @"[\uD800-\uDBFF][\uDC00-\uDFFF]|[\\\/|:*?""<>]", "").Replace(" ", "_");
                  
                    string videoTitle = video.Snippet.Title;
                    string videoId = video.Id.VideoId;
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";


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
                    status =  await InsertVideoDetails(video, outputPath);
                }
            }
            return status;
        }

        status = eStatus.FAILED;
        return status;

    }

   


    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelID"></param>
    /// <returns>Channel ID if the input is validy , otherwise return empty string</returns>
    public static async Task<string> GetChannelIdByNameAsync(string channelID)
    {
        if (channelID.Contains("@"))
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = AppConfig.apiKeyYT,
                ApplicationName = AppConfig.apiProjectNameYT

            });

            try
            {
                var channelRequest = youtubeService.Channels.List("id");
                channelRequest.ForHandle = channelID;  // שם הערוץ בפורמט @Handle
                var channelResponse = channelRequest.ExecuteAsync().Result;

                if (channelResponse.Items != null )
                {
                    if (channelResponse.Items.Any())
                    { 
                    var channel = channelResponse.Items[0];
                    return channel.Id;
                    }
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
        return channelID; 
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

    public static async Task<eStatus> InsertVideoDetails(SearchResult video, string videoPath)
    {
        MultiSourceDataService dBHandler = new MultiSourceDataService();
        List<ChannelExtension> channel = dBHandler.GetDBSet<ChannelExtension>(""," WHERE ChannelExtension_ChannelID = @ID", new { ID = video.Snippet.ChannelId });
        if (channel != null)
        {
            int duration = await durationVideo($"https://www.youtube.com/watch?v={video.Id.VideoId}", AppConfig.YouTubeDLPath);
            //ExtensionMapping
            string ExtensionMapping;
            if(duration > 10) 
                ExtensionMapping = (channel[0].ChannelExtension_Long != "")? channel[0].ChannelExtension_Long: channel[0].ChannelExtension_Short;
            else
                ExtensionMapping = (channel[0].ChannelExtension_Short !="") ? channel[0].ChannelExtension_Short: channel[0].ChannelExtension_Long;

                VideoDetails videoDetails = new VideoDetails()
            {
                VideoDetails_VideoID = video.Id.VideoId,
                VideoDetails_ExtensionMapping = duration > 10 ? channel[0].ChannelExtension_Long : channel[0].ChannelExtension_Short,
                VideoDetails_videoPath = videoPath,
                VideoDetails_Title = video.Snippet.Title
                
            };

            AppStaticParameter.videoDownLoad.Add(videoDetails);
           
            return eStatus.SUCCESS; 
        }
        return eStatus.FAILED;



    }

    
}



