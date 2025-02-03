using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DTO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AppKolHaNoar.Presentation.Controller;
public sealed partial class Dialog : UserControl
{
    public Dialog()
    {
        this.InitializeComponent();
    }
    public async Task<ContentDialogResult> ShowOKDialog(XamlRoot xamlRoot, string message)
    {
        OkDialog.XamlRoot = xamlRoot;
        //OkDialogText.Text = message;    
       return await OkDialog.ShowAsync();

       
    }

    public async Task<ContentDialogResult> ShowErrorDialog(XamlRoot xamlRoot, GenericException message)
    {
        ErrorDialog.XamlRoot = xamlRoot;
        ErrorDialog.Title = message.exceptionTitle;
        ErrorText.Text = message.exceptionMessage;
        ErrorSubText.Text = message.subExceptionMessage;
        return await ErrorDialog.ShowAsync();


    }
}
