using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor.API;
using static DTO.Enums;

namespace MediaProcessor;
public class ProcessingWorkflow
{
    YouTubeMediaHandler youTube = new YouTubeMediaHandler();
    YemotHamashichAPI API_YH = new YemotHamashichAPI();
    MultiSourceDataService dataService = new MultiSourceDataService();
    public async Task<eStatus> UpdateExtension(string channelID)
    {
        eStatus status = eStatus.SUCCESS;
        status = youTube.CheckForNewVideos(channelID);
        if (status == eStatus.SUCCESS)
        {
            await API_YH.UplaodFiles();
            await SetDataLastUpdate(channelID);

        }
        return status;

    }
     
    public void BackgroundRunning()
    {
        List<DateTime> list = new List<DateTime>();
      
      //TO DO
    }
    

    private async Task SetDataLastUpdate(string channelID)
    {
        List<ChannelUpdateInfo> channelUpdateInfo = AppStaticParameter.channelUpdateInfoList.Where(a => a.ChannelUpdateInfo_ChannelID == channelID).ToList();
        if (channelUpdateInfo.Any())
        {
            channelUpdateInfo[0].ChannelUpdateInfo_lastDataUpdate = DateTime.Now.ToString();
            await dataService.UpdateTable(channelUpdateInfo, "ChannelUpdateInfo_ChannelID");
        }
        else
        {
            ChannelUpdateInfo newChannelUpdateInfo = new ChannelUpdateInfo() { ChannelUpdateInfo_ChannelID = channelID, ChannelUpdateInfo_lastDataUpdate = DateTime.Now.ToString() };
            channelUpdateInfo.Add(newChannelUpdateInfo);
            AppStaticParameter.channelUpdateInfoList.Add(newChannelUpdateInfo);
            dataService.AddSet(channelUpdateInfo);
        }


    }
     public async Task BackgroundRunChannel(string channelID)
    {
        
            await UpdateExtension(channelID);
        

    }

    public async Task BackgroundRunCanpaing(string campaignID)
    {
       await API_YH.RunCampaign(campaignID);
    
    }
}
