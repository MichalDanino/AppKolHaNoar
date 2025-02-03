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



namespace AppKolHaNoar.Presentation.ViewProcess;

public partial class UIProcess: ContentDialog
{
//#if WINDOWS
    public static YouTubeMediaHandler s_MEDIAHANLDER = new YouTubeMediaHandler();
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();
    public static string s_MESSAGEDIALOG = "האם אתה מאשר?";
    public static async Task UpdateExtension(XamlRoot parentXamlRoot, string channelID)
    {
        s_YEMOTHAMASHICH.UplaodFiles();

        bool status = false; 
        status = s_MEDIAHANLDER.CheckForNewVideos(channelID);

        if (status)
        {

            bool result = await ShowDialog(parentXamlRoot);
            //if update the extension with new video
            if (result == true)
            {
                status = s_MEDIAHANLDER.DownLoadVideoAsAudio(channelID);
                if (status = true)
                {
                    s_YEMOTHAMASHICH.UplaodFiles();
                }
            }


        }


    }

    private static async Task<bool> ShowDialog(XamlRoot parentXamlRoot)
    {
        s_MESSAGEDIALOG = "ישנן עידכונים חדשים, האם לעלות אותם לשלוחה?";
        ContentDialogResult result = await Dialogs.ShowAskDialog(parentXamlRoot,s_MESSAGEDIALOG);

        // if update extension wuth new video
        if (result == ContentDialogResult.Primary)
        {
            return true;
        }
        return false;
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
