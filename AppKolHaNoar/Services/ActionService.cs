using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using MediaProcessor.API;
using MediaProcessor;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Popups;
using AppKolHaNoar.Presentation.Controller;
using System.Runtime.CompilerServices;
using DTO;
using MediaProcessor;
using Microsoft.UI.Xaml;
using static DTO.Enums;


namespace AppKolHaNoar.Presentation.ViewProcess;

public partial class ActionService : ContentDialog
{
    //#if WINDOWS
    public static YouTubeMediaHandler youTubeMediaHandler = new YouTubeMediaHandler();
    public static YemotHamashichAPI yemotHamashichAPI = new YemotHamashichAPI();
    public static DBHandler DB = new DBHandler();    
    public static string GlobalMessageDialog = "האם אתה מאשר?";
    public static XamlRoot MainPageXamlRoot;

    /// <summary>
    /// Checks if a new video has been uploaded to a specific YouTube channel.
    /// If a new video is found and the user wants to update, the function downloads the video, 
    /// extracts the audio, and uploads it to the phone extension system.
    /// </summary>
    /// <param name="parentXamlRoot">The ID of the YouTube channel to check </param>
    /// <param name="channelID"></param>
    /// <returns></returns>
    public  async Task UpdateExtension( string channelID)
    {
        GenericMessage messgeToUser = null;
        bool status = false;
        status = youTubeMediaHandler.CheckForNewVideos(channelID);

        if (status)
        {
                status = youTubeMediaHandler.DownLoadVideoAsAudio(channelID);

                if (status == true)
                {
                    await yemotHamashichAPI.UplaodFiles();

                    if (MediaProcessor.AppConfig.listExceptions.Any())
                    {
                        await ShowExceptionWithList();

                    } 
                }
            }
        else
            {
            messgeToUser = new GenericMessage() { MessageContent = "אין סרטונים חדשים בערוץ" };
            ShowMessageByDialog(messgeToUser, eDialogType.OK);
            }


        


    }

      public async Task<ContentDialogResult> ShowMessageByDialog( GenericMessage exception, eDialogType dialogType)
    {
       return await Dialogs.MainShowDialog(MainPageXamlRoot, exception,dialogType);
    }


    public static async Task ShowException(GenericMessage exception)
    {
        await ExceptionMessage.ShowErrorDialog(MainPageXamlRoot, exception);
    }

    public static async Task ShowExceptionWithList()
    {
        await ExceptionMessage.ShowExceptionWithList(MainPageXamlRoot);
    }

   public bool RunCampaign(Campaign campaign, XamlRoot xamlRoot)
    {
         
        if (campaign != null)
        {
            yemotHamashichAPI.RunCampaign("1418810");
        }
        else
        {
            GenericMessage message = new GenericMessage() { MessageTitle = "הפעלת קמפיין", MessageContent = "לא נבחר קמפיין להרצה" };
            Dialogs.MainShowDialog( xamlRoot,message,eDialogType.OK);
            return false;
        }
        return true;    
    }

    public bool InsertData<T>(List<T> entityList) where T : class 
        {
           return DB.AddSet<T>(entityList, "jkj");
        }

    public List<T> GetDBSet<T>(object parameters = null)
    {
       return DB.GetDBSet<T>(parameters);
    }
  
    public void GetMainPageXamlRoot(XamlRoot xamlRoot)
    {
        MainPageXamlRoot = xamlRoot;
    }

    //#endif
}
