using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppKolHaNoar.Presentation.Controller;
public class Dialogs
{
    public static async Task<ContentDialogResult> ShowAskDialog(XamlRoot xamlRoot, string message)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Dialog 2",
            Content = message ,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            XamlRoot = xamlRoot 
        };


        return await dialog.ShowAsync();
    }
    public static async Task<ContentDialogResult> ShowOkDialog(XamlRoot xamlRoot, string message)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Dialog 1",
            Content = message,
            PrimaryButtonText = "OK"
        };


        return await dialog.ShowAsync();
    }
}
