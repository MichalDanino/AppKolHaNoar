using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using MediaProcessor.API;
using static DTO.Enums;

namespace AppKolHaNoar.Presentation.Controller;
public class SubDialogManager
{

        private static ContentDialog subCurrentDialog;
        private static DBHandler updateDB = new DBHandler();
      
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
        public static async Task<ContentDialogResult> MainShowSubDialog(XamlRoot xamlRoot, GenericMessage message, eDialogType dialogType)
        {
            Dialogs dialogClass = new Dialogs();
            timeOut = -1;
            subCurrentDialog = new ContentDialog();
            switch (dialogType)
            {
                case eDialogType.OK:
                    dialogClass.OkDialog(xamlRoot, message);
                    break;
                case eDialogType.ASK:
                    dialogClass.AskDialog(xamlRoot, message);
                    break;
                case eDialogType.MultyButton:
                    dialogClass.MultyButton(xamlRoot, message);
                    break;
                case eDialogType.INPUT:
                    dialogClass.InputDialog(xamlRoot, message);
                    break;
                default:
                    break;
            }
            if (subCurrentDialog != null)
            {
                var result = subCurrentDialog.ShowAsync();
                var cancellationTokenSource = new CancellationTokenSource();

                //var dialogTask = result.AsTask();
                // Wait for either the dialog to complete or the timeout to occur
                Task completedTask = await Task.WhenAny(result.AsTask(), Task.Delay(timeOut == -1 ? Timeout.Infinite : timeOut, cancellationTokenSource.Token));

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
                    subCurrentDialog.Hide();

                }


            }
            return ContentDialogResult.None;


        }


        public void AskDialog(XamlRoot xamlRoot, GenericMessage message)
        {
            subCurrentDialog = new ContentDialog
            {
                Title = message.MessageTitle,
                Content = message.MessageContent,
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
        public void OkDialog(XamlRoot pageXamlRoot, GenericMessage message)
        {
            subCurrentDialog = new ContentDialog
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
            subCurrentDialog = new ContentDialog
            {
                Title = message.MessageTitle,
                Content = new StackPanel
                {
                    Children =
        {
            new TextBlock { Text = message.MessageContent },
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
           //TO DO

        }
        

        public async static Task getNewPassword(XamlRoot xamlRoot)
        {
            bool status = false;
            GenericMessage message = new GenericMessage() { MessageContent = "הכנס את הסיסמה החדשה" };
            await MainShowSubDialog(xamlRoot, message, eDialogType.INPUT);
            // do { 
            //ContentDialog innerDialog = new ContentDialog
            // {
            //     Title = message.MessageTitle,
            //     Content = new StackPanel
            //     {
            //         Children =
            // {
            //     new TextBlock { Text = message.MessageContent },
            //     new TextBox { Name = "InputTextBox", PlaceholderText = "Enter your input here" }
            // }
            //     },
            //     PrimaryButtonText = "Yes",
            //     SecondaryButtonText = "No",
            //     XamlRoot = xamlRoot
            // };
            // var result = await innerDialog.ShowAsync();

            //     if (result == ContentDialogResult.Primary)
            //     {
            //         // קבלת הנתונים מהדיאלוג השני
            //         var panel = innerDialog.Content as StackPanel;
            //         if (panel != null)
            //         {
            //             var textBox = panel.Children.OfType<TextBox>().FirstOrDefault();
            //             if (textBox.Text != string.Empty)
            //             {
            //                 status = true;

            //                 return textBox.Text;


            //             }
            //             message.MessageContent = "לא הוכנסה סיסמה אנא הכנס";

            //         }
            //     } while (status == false) ;

            // return "";
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
