using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using static DTO.Enums;
using static AppKolHaNoar.Models.AppConfig;
using MediaProcessor;

namespace AppKolHaNoar.Presentation.Controller;
public class Dialogs
{

    private static ContentDialog currentDialog;
    private static ProgressBar progressBar;
    private static MultiSourceDataService updateDB = new MultiSourceDataService();
    private static List<string> nameTable = new List<string>() { "מילים לסינון","עדכון הקמפיינים", "עדכון שלוחות", "עדכון ערוצים" };
   
    private static int timeOut;
    private TaskCompletionSource<string> dialogResultSource = new TaskCompletionSource<string>();

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
         currentDialog = new ContentDialog();
        progressBar = new ProgressBar();
        ProgressBarDialog();
        switch (dialogType)
        {
            case eDialogType.OK:
                dialogClass.OkDialog(xamlRoot, message);
                break;
            case eDialogType.ASK:
                dialogClass.AskDialog(xamlRoot, message);
                break;
            case eDialogType.MultyButton:
               dialogClass.MultyButton(xamlRoot,message);
                

                break;
            case eDialogType.INPUT:
                dialogClass.InputDialog(xamlRoot, message);
                break;
            case eDialogType.list:
                dialogClass.Showlistpassword(xamlRoot);
                break;

            default:
                break;
        }

        if (currentDialog != null)
        {
            var result =  currentDialog.ShowAsync();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageXamlRoot"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public void InputDialog(XamlRoot pageXamlRoot, GenericMessage message)
    {
        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content = new StackPanel
            {
                Children =
        {
             new TextBlock { Text = message.subMessage },

            new TextBlock { Text = message.MessageContent },
            new TextBlock { 
                Text = message.subMessage , Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red)},
             

            new TextBox { Name = "InputTextBox", PlaceholderText = "Enter your input here" }
        }
            },
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            XamlRoot = pageXamlRoot
        };
        


    }

    public void MultyButton(XamlRoot pageXamlRoot, GenericMessage message)
    {
        if(message.MessageTitle.Contains("נתונים"))
            DataChangeDialog(pageXamlRoot, message);
        if(message.MessageTitle.Contains("סיסמאות"))
            passwordChangeDialog(pageXamlRoot,message);

    }
    public void DataChangeDialog(XamlRoot pageXamlRoot, GenericMessage message)
    {
        
        StackPanel stackPanel = new StackPanel();
        //dialogResultSource = new TaskCompletionSource<string>();
        TextBlock textBlock = new TextBlock()
        {
            Text = message.MessageContent
        };
        var scrollViewer = new ScrollViewer
        {
            Content = stackPanel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, // גלילה אוטומטית אם צריך
            MaxHeight = 400
        };
        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content =scrollViewer,
            XamlRoot = pageXamlRoot,
            PrimaryButtonText = "אישור", // מונע סגירה אוטומטית
            CloseButtonText = ""
        };
        stackPanel.Children.Add(progressBar);
        stackPanel.Children.Add(textBlock);
      //  stackPanel.Children.Add(scrollViewer);
        foreach (var buttonText in nameTable)
        {
            var button = new Button
            {
                Content = buttonText,
                Margin = new Thickness(5)
            };
         
               
           
                button.Click += async (sender, e) =>
                {

                    progressBar.Visibility = Visibility.Visible;
                    await Task.Run(() =>
                    {
                        updateDB.ChangeDataInDB(buttonText);
                    });
                    progressBar.Visibility = Visibility.Collapsed;


                };
          

            stackPanel.Children.Add(button);
        }


    }
    public void passwordChangeDialog(XamlRoot pageXamlRoot, GenericMessage message)
    {
            StackPanel stackPanel = new StackPanel();
        var scrollViewer = new ScrollViewer
        {
            Content = stackPanel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, // גלילה אוטומטית אם צריך
            MaxHeight = 400
        };
        currentDialog = new ContentDialog
        {
            Title = message.MessageTitle,
            Content = scrollViewer,
            XamlRoot = pageXamlRoot,
            PrimaryButtonText = "אישור", // מונע סגירה אוטומטית
            CloseButtonText = ""
        };

        foreach (var buttonText in TranslationTable.passwords)
        {
            var button = new Button
            {
                Content = buttonText.Key,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center

            };



            button.Click += async (sender, e) =>
            {

                GenericMessage message1 = new GenericMessage() { MessageContent = "הכנס ססיסמה ל" + buttonText.Key };
                ChangePassword(currentDialog.XamlRoot, message1, buttonText.Value);
            };
            

            stackPanel.Children.Add(button);
        }

        TextBlock textBlock = new TextBlock()
        {
            Text = "צפה בסיסמאות הנוכחיות",
            TextDecorations = Windows.UI.Text.TextDecorations.Underline,


        };
        


        }


    private async void ChangePassword(XamlRoot pageXamlRoot,GenericMessage message,string nameProperty)
    {
        currentDialog.Hide();
        bool getNewPassword = false;
        TextBox textBox = new TextBox();
        do
        {
            ContentDialogResult result = await MainShowDialog(pageXamlRoot, message, eDialogType.INPUT);
            if (result == ContentDialogResult.Primary)
            {
                StackPanel stackPanel = (StackPanel)currentDialog.Content;
                textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

                if (textBox != null && textBox.Text != "")
                {
                    getNewPassword = true;
                }
                message.subMessage = "לא התקבלה סיסמה";
            }
            else
                break;
        } while (!getNewPassword);
        updateDB.UpdatePassword(nameProperty, textBox.Text);
    }


    public static async Task<bool> DisplayPasswords(XamlRoot pageXamlRoot, GenericMessage message)
    {
        
        bool getNewPassword = false;
        TextBox textBox = new TextBox();
        do
        {
            ContentDialogResult result = await MainShowDialog(pageXamlRoot, message, eDialogType.INPUT);
            if (result == ContentDialogResult.Primary)
            {
                StackPanel stackPanel = (StackPanel)currentDialog.Content;
                textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

                if (textBox != null)
                {
                    if (textBox.Text == MediaProcessor.AppConfig.ManagerPassword)
                    {
                        getNewPassword = true;
                    }


                    else if (textBox.Text == "")
                    {
                        message.subMessage = "לא התקבלה סיסמה";

                    }
                    else
                         message.subMessage = "הסיסמה שגויה";



                }
                } 
            else
                break;
        } while (!getNewPassword  );

        return getNewPassword;
    }

    public void Showlistpassword(XamlRoot pageXamlRoot)
    {

        var listView = new ListView
        {

            SelectionMode = ListViewSelectionMode.None, // מונע בחירה רגילה
            ItemsSource = updateDB.ShowPasswort()
        };

        currentDialog = new ContentDialog
        {
            Title = "הסיסמאות",
            Content = listView,
            PrimaryButtonText = "אישור",
            SecondaryButtonText = "עדכן סיסמה",
            CloseButtonText = "ביטול",
            XamlRoot = pageXamlRoot
        };
        currentDialog.SecondaryButtonClick += async (sender, args) =>
        {
            currentDialog.Hide();
            // מחליפים ליעד הרצוי
            GenericMessage message = new GenericMessage() { MessageTitle = "שינוי סיסמאות" };
            MainShowDialog(pageXamlRoot, message, eDialogType.MultyButton);

        };

    }
    public static void ProgressBarDialog()
    {
         progressBar = new ProgressBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 0,
             Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 220, 37, 5)),
             Background = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 225, 225, 225)),
            IsIndeterminate = true, // הצגת ProgressBar ללא צורך במעקב אחר ערכים
            Visibility = Visibility.Collapsed // בהתחלה נסתר
        };
    }

    
}
