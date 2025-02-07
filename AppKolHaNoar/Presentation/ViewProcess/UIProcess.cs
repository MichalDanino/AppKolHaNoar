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


namespace AppKolHaNoar.Presentation.ViewProcess;

public partial class UIProcess : ContentDialog
{
    //#if WINDOWS
    public static YouTubeMediaHandler youTubeMediaHandler = new YouTubeMediaHandler();
    public static YemotHamashichAPI yemotHamashichAPI = new YemotHamashichAPI();
    public static DBHandler DB = new DBHandler();    
    public static string GlobalMessageDialog = "האם אתה מאשר?";
    static XamlRoot xamlRoot = null;


    /// <summary>
    /// Checks if a new video has been uploaded to a specific YouTube channel.
    /// If a new video is found and the user wants to update, the function downloads the video, 
    /// extracts the audio, and uploads it to the phone extension system.
    /// </summary>
    /// <param name="parentXamlRoot">The ID of the YouTube channel to check </param>
    /// <param name="channelID"></param>
    /// <returns></returns>
    public static async Task UpdateExtension(XamlRoot parentXamlRoot, string channelID)
    {
        xamlRoot = parentXamlRoot;
        GenericException exception = null;
        bool status = false;
        status = youTubeMediaHandler.CheckForNewVideos(channelID);

        if (status)
        {

            bool result = await ShowDialog();
            //if update the extension with new video
            if (result == true)
            {
                status = youTubeMediaHandler.DownLoadVideoAsAudio(channelID);

                if (status == true)
                {
                    await yemotHamashichAPI.UplaodFiles();

                    if (MediaProcessor.AppConfig.listExceptions.Any())
                    {
                        await ShowExceptionWithList();

                    } {
                    }
                }
            }


        }


    }

    private static async Task<bool> ShowDialog()
    {
        GlobalMessageDialog = "ישנן עידכונים חדשים, האם לעלות אותם לשלוחה?";
        ContentDialogResult result = await Dialogs.ShowAskDialog(xamlRoot, GlobalMessageDialog);

        // if update extension wuth new video
        if (result == ContentDialogResult.Primary)
        {
            return true;
        }
        return false;
    }

    public static async Task ShowException(GenericException exception)
    {
        await ExceptionPopUp.ShowErrorDialog(xamlRoot, exception);
    }

    public static async Task ShowExceptionWithList()
    {
        await ExceptionPopUp.ShowExceptionWithList(xamlRoot);
    }



    public void  InsertData<T>(List<T> entityList) where T : class 
        {
            DB.AddSet<T>(entityList, "");
        }
    //public static async Task<ContentDialogResult> ShowDialogAsync(XamlRoot parentXamlRoot)
    //{
    //    ContentDialog dialog = new ContentDialog
    //    {
    //        Title = "Custom Dialog",
    //        Content = s_MESSAGEDIALOG,
    //        PrimaryButtonText = "אישור",
    //        SecondaryButtonText = "ביטול",
    //        XamlRoot = parentXamlRoot // חשוב עבור Uno Platform
    //    };

    //    return  await dialog.ShowAsync();

    //}

    //#endif
}
