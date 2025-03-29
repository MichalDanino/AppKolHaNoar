using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor.API;

namespace AppKolHaNoar.Services;
public class BackgroundTask
{
    List<DateTime> list = new List<DateTime>();
    List<ChannelExtension> channelExtensions = new List<ChannelExtension>();
    List<Campaign> campaigns = new List<Campaign>();
    public static MultiSourceDataService DB = new MultiSourceDataService();
    ServiceUI ServiceUI = new ServiceUI();
    public  bool run()
    {
       
        List<ChannelExtension> channel = DB.GetDBSet<ChannelExtension>().FindAll(a => a.ChannelExtension_RunningTime != null);
        List<Campaign> campin = DB.GetDBSet<Campaign>().FindAll(a => a.Campaign_RunningTime != null);
        ChannelExtension sss = new ChannelExtension()
        {
            ChannelExtension_ChannelID = "fff"
        };
        ServiceUI.UpdateExtension(sss.ChannelExtension_ChannelID);

       

        //foreach (var channelExtension in channel)
        //{
        //    DateTime.TryParse(channelExtension.ChannelExtension_RunningTime, out DateTime dateTime);
        //    if (dateTime.Hour == DateTime.Now.Hour)
        //       ServiceUI.UpdateExtension(channelExtension.ChannelExtension_ChannelID);

        //}

        //foreach (var campain1 in campin)
        //{
        //    DateTime.TryParse(campain1.Campaign_RunningTime, out DateTime dateTime);
        //    if (dateTime.Hour == DateTime.Now.Hour)
        //        campaigns.Add(campain1);

        //}
        //foreach (var item in campaigns)
        //{
        //    RunCampaign(item);
        //    campaigns.Remove(item);
        //}
        //foreach (var item in channelExtensions)
        //{
        //}
        return true;
    }
}
