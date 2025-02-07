using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using DotNetEnv;
using DTO;
using Google;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MediaProcessor.API;
using Newtonsoft.Json.Linq;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;



namespace MediaProcessor
{
    public class YouTubeMediaHandler
    {
        private static SearchListResponse listResponse = new SearchListResponse();

        /// <summary>
        /// The function Check if there are new videos in specific channel
        /// </summary>
        /// <param name="NameChannel">name channel to check</param>
        /// <returns>return true if there are new video otherwish, false</returns>
        public bool CheckForNewVideos(string NameChannel)
        {
            if (!string.IsNullOrEmpty(NameChannel))
            {
                string channelID = "UC-Dheb1tLoTo962VC2of1bw"; /*"GetChannelIdByName(NameChannel);*/
                DateTime lastUpdateExtension = GetLastUpdateExtension(channelID);
                // get list of last video in channel
                listResponse = YouTubeAPI.fetchChannelVideosByAPI(channelID).GetAwaiter().GetResult();
               
               var newVideo = listResponse.Items.Where(a => a.Id.Kind == "youtube#video" && a.Snippet.PublishedAt > lastUpdateExtension);
               
                //if there are new video
              if(newVideo.Any())
              {
                  return true;
              }
            }
                
            return false;
        }

        /// <summary>
        /// download video as audio 
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns>return true if successful ,otherwish false</returns>
        public bool DownLoadVideoAsAudio(string channelID)
        {
            return YouTubeAPI.DownloadVideoAsAudio(channelID, listResponse).Result;
            
        }
        /// <summary>
        /// Get channel ID according by name channel
        /// </summary>
        /// <param name="NameChannel">Name of channel</param>
        /// <returns>channel ID</returns>
        public string GetChannelIdByName(string NameChannel)
        {
            //To Do
            return "";
        }

        /// <summary>
        /// Get last time that channel was update
        /// </summary>
        /// <param name="channelID">channel ID</param>
        /// <returns>last update date</returns>
        private DateTime GetLastUpdateExtension(string channelID)
        {
            //To Do
            return DateTime.Now;    

        }
    }
}
