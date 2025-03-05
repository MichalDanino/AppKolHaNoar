using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor.API;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using static DTO.Enums;

namespace AppKolHaNoar.Presentation.Controller;
public class Dialogs
{
 
    private static ContentDialog currentDialog;
    private static DBHandler updateDB = new DBHandler();
   private static List<string> nameTable = new List<string>() { "עדכון הקמפיינים", "עדכון שלוחות", "עדכון ערוצים", "אישור" };
    private static int timeOut;
    private TaskCompletionSource<string> dialogResultSource;

    /// <summary>
    /// Displays a content dialog with a specified timeout. If the dialog is not interacted with before the timeout,
    /// it automatically closes and returns a <see cref="ContentDialogResult.None"/>. Otherwise, it returns the result
    /// of the dialog interaction (e.g., Primary, Secondary, or None).
    /// </summary>
    /// <param name="xamlRoot">current page xaml root </param>
    /// <param name="message">message dialog</param>
    /// <param name="dialogType">Specifies the type of dialog to display<see cref="E_dialogType"/></param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the <see cref="ContentDialogResult"/>
    /// that indicates how the dialog was closed (either by user interaction or timeout). 
    /// </returns>
    public static async Task<ContentDialogResult> MainShowDialog(XamlRoot xamlRoot,GenericMessage message , eDialogType dialogType)
    {
        Dialogs dialogClass = new Dialogs();
        timeOut = -1;
        switch (dialogType)
        {
            case eDialogType.OK:
                dialogClass.OkDialog(xamlRoot, message);
                break;
            case eDialogType.ASK:
                dialogClass.AskDialog(xamlRoot, message);
                break;
            case eDialogType.MultyButton:
               dialogClass.DataChangeDialog(xamlRoot,message);
                break;  
            default:
                break;
        }
        if (currentDialog != null)
        {
            var result = currentDialog.ShowAsync();
            var cancellationTokenSource = new CancellationTokenSource();

            //var dialogTask = result.AsTask();
            // Wait for either the dialog to complete or the timeout to occur
            Task completedTask = await Task.WhenAny(result.AsTask(), Task.Delay(timeOut== -1? Timeout.Infinite : timeOut, cancellationTokenSource.Token));
           
            // If the dialog operation completes, return the result
            if (completedTask == result.AsTask())
            {
                return result.GetResults();

                

            }
            else
            {
                // If the dialog operation completes, return the result
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return result.GetResults();
                }
                // Timeout occurred, close the dialog and return None
                currentDialog.Hide();

            }
           

        }
        return ContentDialogResult.None;


    }


    public  void AskDialog(XamlRoot xamlRoot, GenericMessage message)
    {
        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content = message.MessageContent ,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            XamlRoot = xamlRoot 
        };
        


       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageXamlRoot"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public  void OkDialog(XamlRoot pageXamlRoot, GenericMessage message)
    {
        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content = message.MessageContent,
            PrimaryButtonText = "OK",
            XamlRoot = pageXamlRoot 

        };
        timeOut = 300000;


    }

    public void DataChangeDialog(XamlRoot pageXamlRoot, GenericMessage message)
    {
        StackPanel stackPanel = new StackPanel();
        //dialogResultSource = new TaskCompletionSource<string>();

        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content = stackPanel,
            XamlRoot = pageXamlRoot,
            PrimaryButtonText = "אישור", // מונע סגירה אוטומטית
            CloseButtonText = ""
        };

        foreach (var buttonText in nameTable)
        {
            var button = new Button
            {
                Content = buttonText,
                Margin = new Thickness(5)
            };
            if (buttonText == "אישור")
            {
                button.Click += (sender, e) =>
                {
                     dialogResultSource.SetResult(buttonText);
                    currentDialog.Hide();
                };
            }
            else { 
            button.Click += (sender, e) => updateDB.ChangeDataInDB(buttonText);
            }

            stackPanel.Children.Add(button);
        }


    }

    public static void ShowlistDialog(XamlRoot pageXamlRoot, List<string> message, string title = "")
    {

        var listView = new ListView
        {
            SelectionMode = ListViewSelectionMode.None, // מונע בחירה רגילה
            ItemsSource = title
        };
        // שימוש ב-ItemTemplate כדי להוסיף CheckBox לכל פריט
        //listView.ItemTemplate = new DataTemplate(() =>
        //{
        //    var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        //    var checkBox = new CheckBox();
        //    var textBlock = new TextBlock();
        //    textBlock.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("Name") });

        //    stackPanel.Children.Add(checkBox);
        //    stackPanel.Children.Add(textBlock);

        //    var container = new ListViewItem { Content = stackPanel };
        //    return container;
        //});

        // יצירת הדיאלוג
        ContentDialog dialog = new ContentDialog
        {
            Title = "בחר פריטים",
            Content = listView,
            PrimaryButtonText = "אישור",
            CloseButtonText = "ביטול",
            XamlRoot = pageXamlRoot
        };


        //return await dialog.ShowAsync();
    }
}
