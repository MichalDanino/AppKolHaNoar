using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using DotNetEnv;
using DTO;
using static DTO.Enums;
using Google;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MediaProcessor.API;
using Newtonsoft.Json.Linq;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace MediaProcessor
{
    public class YouTubeMediaHandler
    {
        private static SearchListResponse listResponse = new SearchListResponse();

        /// <summary>
        /// The function check if there are new videos in specific channel
        /// </summary>
        /// <param name="NameChannel">name channel to check</param>
        /// <returns>return true if there are new video otherwish, false</returns>
        public eStatus CheckForNewVideos(string channelID)
        {
            eStatus status = eStatus.FAILED;

            if (!string.IsNullOrEmpty(channelID))
            {
                status = YouTubeAPI.YouTubeMediaMain(channelID).Result;
            
                  return status;
             
            }
                
            return status;
        }

     
     

        /// <summary>
        /// Get last time that channel was update
        /// </summary>
        /// <param name="channelID">channel ID</param>
        /// <returns>last update date</returns>
        public static DateTime GetLastUpdateExtension(string channelID)
        {
            MultiSourceDataService dataService = new MultiSourceDataService();  
           ChannelExtension channel =  dataService.GetDBSet<ChannelExtension>().FirstOrDefault(a=> a.ChannelExtension_ChannelID == channelID);
            if (channel == null)
            { 
                DateTime.TryParse(channel.ChannelExtension_RunningDay, out DateTime dateChannel);
                return dateChannel;
            }
            
            return DateTime.Now.AddMonths(-1);

        }
    }
}
