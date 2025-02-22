using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI;
using DTO;
//using AndroidX.ConstraintLayout.Core.Motion.Utils;


namespace AppKolHaNoar.Presentation.Controller;
public static class ExceptionMessage
{
//#if WINDOWS
    public static async Task<ContentDialogResult> ShowErrorDialog(XamlRoot xamlRoot, GenericMessage message)
    {

        var stackPanel = new StackPanel { Spacing = 10 };

        // כותרת מעוצבת
        var titleBlock = new TextBlock
        {
            Text = "New Update Available!",
            FontSize = 20,
            //FontWeight = Windows.UI.Text.FontWeight,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 220, 37, 5)),
            HorizontalAlignment = HorizontalAlignment.Center
        };

        // אייקון והתראה
        var iconStack = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        var icon = new FontIcon { Glyph = "🔔", FontSize = 32, Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(30,220,37,5)) };
        var messageBlock = new TextBlock
        {
            Text = "A new version is available. Would you like to update now?",
            FontSize = 16,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(225,0,0,0)),
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        iconStack.Children.Add(icon);
        iconStack.Children.Add(messageBlock);

        // הוספת הרכיבים לדיאלוג
        stackPanel.Children.Add(titleBlock);
        stackPanel.Children.Add(iconStack);




        // יצירת הדיאלוג
        ContentDialog dialog = new ContentDialog
        {


            Title = "שגיאה",
            //Content = new StackPanel
            //{
            //    Children =
            //{
            //    new SymbolIcon
            //    {
            //        Symbol = Symbol.Stop, // אייקון שגיאה
            //        //Foreground =Microsoft.UI.Xaml.Windows.UI.Color.FromArgb(20,220,37,5),
            //        Height = 50,
            //        Width = 50
            //    },
            //    new TextBlock
            //    {
            //        Text = message.exceptionMessage,
            //        TextWrapping = TextWrapping.Wrap,
            //        FontSize = 16,
            //       // Margin = new Windows.UI.Xaml.Thickness(10)
            //    }
            //}
            //},
            Content = stackPanel,
            
            PrimaryButtonText = "אישור",
            XamlRoot = xamlRoot,
            Background = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 220, 37, 5) ),
            CornerRadius = new CornerRadius(12)
            //RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark // ניתן לשנות לפי העדפות העיצוב
        };



        return await dialog.ShowAsync();
    }
    /// <summary>
    ///  Merges all error messages into one string.
    ///  Each error message is placed on a separate line, 
    ///  making it easier to read in logs.
    /// </summary>
    /// <param name="xamlRoot">xamlroot of main page - need to show dialog</param>
    /// <param name="ListExceptions">list of error to display</param>
    /// <returns>None</returns>

    public static async Task ShowExceptionWithList(XamlRoot xamlRoot)
    {
        GenericMessage exception = new GenericMessage();
        /* 
        * Merges all error messages into one string.
        * Each error message is placed on a separate line, 
        * making it easier to read in logs.
        */
        exception.subMessageMessage = string.Join(Environment.NewLine, MediaProcessor.AppConfig.listExceptions.Select(item => item.MessageContent));
        await ShowErrorDialog(xamlRoot, exception);
        MediaProcessor.AppConfig.listExceptions.Clear();    
    }
//#endif
}
