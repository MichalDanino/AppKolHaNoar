using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor;
using MediaProcessor.API;

namespace AppKolHaNoar.Services;
public class BackgroundTask
{
    List<DateTime> list = new List<DateTime>();
    List<ChannelExtension> channelExtensions = new List<ChannelExtension>();
    List<Campaign> campaigns = new List<Campaign>();
    public static MultiSourceDataService DB = new MultiSourceDataService();
    ProcessingWorkflow ServiceUI = new ProcessingWorkflow();
    public async Task<bool> run(string campaignID, string channelID)
    {
        if (campaignID != null)
            await ServiceUI.BackgroundRunCanpaing(channelID);
        if(channelID != null) { }
           // ServiceUI.BackgroundRunChannel()
           //TO DO
           return true;
    }
}
