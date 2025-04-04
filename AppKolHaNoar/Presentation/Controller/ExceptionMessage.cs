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
            Text = message.MessageTitle,
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
            Text = message.MessageContent,
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
    
            Content = stackPanel,
            
            PrimaryButtonText = "אישור",
            
            XamlRoot = xamlRoot,
            Background = new SolidColorBrush(Windows.UI.Color.FromArgb(225, 220, 37, 5) ),
            CornerRadius = new CornerRadius(12)
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
        exception.subMessage = string.Join(Environment.NewLine, MediaProcessor.AppConfig.listExceptions.Select(item => item.MessageContent));
        await ShowErrorDialog(xamlRoot, exception);
        MediaProcessor.AppConfig.listExceptions.Clear();    
    }
//#endif
}
