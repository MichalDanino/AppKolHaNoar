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



namespace AppKolHaNoar.Presentation.ViewProcess;

public class UIProcess: ContentDialog
{
#if WINDOWS
    public static YouTubeMediaHandler s_MEDIAHANLDER = new YouTubeMediaHandler();
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();
    public static string s_MESSAGEDIALOG = "האם אתה מאשר?";
    public static async Task UpdateExtension(XamlRoot parentXamlRoot, string channelID)
    {
        bool NewVideo = s_MEDIAHANLDER.CheckForNewVideos(channelID);

        if (NewVideo)
        {
            s_MESSAGEDIALOG = "ישנן עידכונים חדשים, האם לעלות אותם לשלוחה?";
            ContentDialogResult result = await ShowDialogAsync(parentXamlRoot);
            if (result == ContentDialogResult.Primary)
            {
                s_MEDIAHANLDER.UpdateNewVideoInExtension(channelID);

            }
            else if (result == ContentDialogResult.Secondary)
            {
            }
            
        }


    }
    public static async Task<ContentDialogResult> ShowDialogAsync(XamlRoot parentXamlRoot)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Custom Dialog",
            Content = s_MESSAGEDIALOG,
            PrimaryButtonText = "אישור",
            SecondaryButtonText = "ביטול",
            XamlRoot = parentXamlRoot // חשוב עבור Uno Platform
        };

        return  await dialog.ShowAsync();
        
    }

#endif
    }
