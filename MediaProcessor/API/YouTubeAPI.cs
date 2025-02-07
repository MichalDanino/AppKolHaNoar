using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using DataAccess;
using DotNetEnv;
using DTO;
using Google;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

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
                ApiKey = AppConfig.apiKeyYT,
                ApplicationName = AppConfig.apiProjectNameYT
            });
            // Playlist ID data request details
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.ChannelId = ChannelID;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date; // לפי תאריך פרסום
            searchRequest.MaxResults = MaxVidowToDownload;
            SearchListResponse channelResponse = await searchRequest.ExecuteAsync();



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

    public async void YouTubeMediaMain(string channelID)
    {
        //need update 


        SearchListResponse playlistItemListResponse = await fetchChannelVideosByAPI(channelID);
        await DownloadVideoAsAudio(channelID, playlistItemListResponse);
    }

    public static async Task durationVideo(string videoUrl, string youtubeDL)
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
                int duration = videoData["duration"]?.Value<int>() ?? 0;

              
            }
        }
    }
    public static async Task<bool> DownloadVideoAsAudio(string channelId, SearchListResponse videoList)
    {
        AppConfig.exceptions.Clear();
        if (videoList != null)
        {
            var youtubeDl = new YoutubeDL
            {
                YoutubeDLPath = @"C:\yt-dlgANDffmpeg\yt-dlg\yt-dlp.exe",
                FFmpegPath = @"C:\yt-dlgANDffmpeg\yt-dlg\ffmpeg.exe"
            };
            foreach (var video in videoList.Items)
            {

                if (video.Id.Kind == "youtube#video") // verify that is video
                {
                    string videoTitle = video.Snippet.Title.Replace(" ", "_");
                    string videoId = video.Id.VideoId;
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                    //string videoUrl = $"https://www.youtube.com/watch?v=5lGUEB3mkhc";


                    await durationVideo(videoUrl, youtubeDl.YoutubeDLPath);
                    // path folder output
                    string outputPath = Path.Combine(AppConfig.rootURL +"Downloads", $"{videoTitle}.%(ext)s");

                    // Setting download options including disabling SSL
                    var options = new OptionSet
                    {
                        Output = outputPath,
                        ExtractAudio = true,
                        AudioFormat = AudioConversionFormat.Mp3,
                        NoCheckCertificates = true //disabling SSL
                    };

                    // Audio download only
                    var result = await youtubeDl.RunWithOptions(videoUrl, options);
                    if (!result.Success)
                    { 
                        AppConfig.exceptions.Add(new GenericException()
                        {
                            exceptionTitle = "נתקל בבעיה בהורדת הסרטון",
                            exceptionMessage = "נתיב :" + videoUrl+ " " + result.ErrorOutput,
                            subExceptionMessage =  "אנא וודא שהאינטרנט מחובר או שהסרטון נפתח במחשב"

                        });
                    }
                }
            }
            return true;
        }


        return false;

    }

    /// <summary>
    /// update the following parameter of video: channelID, Duration and call Extension to upload this file
    /// </summary>
    /// <param name="videoUrl">video url inorder to get ID channel</param>
    /// <param name="duration">video duration </param>
    public void InsertVideoDetails(string videoUrl, int duration)
    {
        string channelId = videoUrl.Split("=")[1].Replace("\"", "");
        SQLiteAccess sQLiteAccess = new SQLiteAccess(AppConfig.rootURL);

        //Retrieve the channel entity along with its linked call extension information
        List<ChannelExtension> channelExtension = sQLiteAccess.GetDBSet<ChannelExtension>($"WHERE UserID ={channelId}");
       
        //update DB
        VideoDetails videoDetails = new VideoDetails()
        {
            VideoDetails_VideoID = channelId,
            VideoDetails_Duration = duration,
            VideoDetails_ExtensionMapping = channelExtension[0].ChannelExtensionLong
        };
        // if video lass then 10 minets
        if(duration<=10)
        {;
            videoDetails.VideoDetails_ExtensionMapping = channelExtension[0].ChannelExtensionShort;
         }
    }



}

