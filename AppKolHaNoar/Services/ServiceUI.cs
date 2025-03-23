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
namespace AppKolHaNoar.Services;
public class ServiceUI : ContentDialog
{
//#if WINDOWS

public static YouTubeMediaHandler youTubeMediaHandler = new YouTubeMediaHandler();
public static YemotHamashichAPI yemotHamashichAPI = new YemotHamashichAPI();
public static MultiSourceDataService DB = new MultiSourceDataService();
public static string GlobalMessageDialog = "האם אתה מאשר?";
public static XamlRoot MainPageXamlRoot;
GenericMessage message = new GenericMessage();

    /// <summary>
    /// Checks if a new video has been uploaded to a specific YouTube channel.
    /// If a new video is found and the user wants to update, the function downloads the video, 
    /// extracts the audio, and uploads it to the phone extension system.
    /// </summary>
    /// <param name="parentXamlRoot">The ID of the YouTube channel to check </param>
    /// <param name="channelID"></param>
    /// <returns></returns>
    public async Task UpdateExtension(string channelID)
{
    eStatus status = eStatus.SUCCESS;
    status = youTubeMediaHandler.CheckForNewVideos(channelID);

    if (status == eStatus.SUCCESS)
    {
       // status = youTubeMediaHandler.DownLoadVideoAsAudio(channelID);

        if (status == eStatus.SUCCESS)
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
        if (status == eStatus.NETWORKERROR)
        {
            message = new GenericMessage() { MessageContent = "אין סרטונים חדשים בערוץ" };
        }
        else if (status == eStatus.FAILED)
        {
            message = new GenericMessage() { MessageContent = "אין סרטונים חדשים בערוץ" };
        }
        else
        {
            message = new GenericMessage() { MessageContent = "הערוץ עודכן בסרטונים החדשים" };

        }
        ShowMessageByDialog(message, eDialogType.OK);
    }
}








public async Task<ContentDialogResult> ShowMessageByDialog(GenericMessage exception, eDialogType dialogType)
{
    return await Dialogs.MainShowDialog(MainPageXamlRoot, exception, dialogType);
}


public static async Task ShowException(GenericMessage exception)
{
    await ExceptionMessage.ShowErrorDialog(MainPageXamlRoot, exception);
}

public static async Task ShowExceptionWithList()
{
    await ExceptionMessage.ShowExceptionWithList(MainPageXamlRoot);
}

public bool RunCampaign(Campaign campaign)
{

    if (campaign != null&& campaign.Campaign_Name!="")
    {
        yemotHamashichAPI.RunCampaign(campaign.Campaign_Number);
    }
    else
    {
        message = new GenericMessage() { MessageTitle = "הפעלת קמפיין", MessageContent = "לא נבחר קמפיין להרצה" };
        Dialogs.MainShowDialog(MainPageXamlRoot, message, eDialogType.OK);
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
    return DB.GetDBSet<T>("","",parameters);
}

    public eStatus UpdatePassword(string namePassword, string password)
    {
       return DB.UpdatePassword(namePassword,password);
    }

public async Task<bool> ChangeDB()
{
    message = new GenericMessage() { MessageTitle = "עדכון/הוספת נתונים", MessageContent = "בעת לחיצה על הכפתור יפתח אקסל, אנא מלא את הנתונים, שמור את האקסל ולחץ על אישור לעדכון הנתונים" };

    ContentDialogResult result = await Dialogs.MainShowDialog(MainPageXamlRoot, message, eDialogType.MultyButton);

    if (result == ContentDialogResult.Primary)
    {
        if (DB.ControlDataSync() == eStatus.SUCCESS)
        {

            message = new GenericMessage() { MessageContent = " העדכון בוצע בהצלחה" };
        }
        else
        {
            message = new GenericMessage() { MessageContent = "משהו השתבש, אנא נסה שוב" };

        }
        await Dialogs.MainShowDialog(MainPageXamlRoot, message, eDialogType.OK);

    }
    return true;

}

public List<Campaign> GetCampaignsTable()
    {
       return   yemotHamashichAPI.GetCampaing().Result;
    }


public void GetMainPageXamlRoot(XamlRoot xamlRoot)
{
    MainPageXamlRoot = xamlRoot;
}
    public void dddd()
    {
      // YouTubeAPI youTubeAPI= new YouTubeAPI();
        YouTubeAPI.DownloadVideoAsText("xamlRoot");
    }
    //#endif
}
