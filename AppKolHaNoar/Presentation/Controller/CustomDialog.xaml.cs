using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Reactive;
using MediaProcessor.API;
//using Android.App;
//using Android.App;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AppKolHaNoar.Presentation.Controller;
public partial class CustomDialog : UserControl
{
    public CustomDialog()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Shows the ContentDialog.
    /// </summary>
    public async Task<bool> ShowContentDialog(string message)
    {
        

        // OkDialog.XamlRoot =  this..XamlRoot;
        // var dialog = new CustomDialog();

        //ContentDialogResult result = await this.ddd.ShowAsync();
        //if (result == ContentDialogResult.Primary)
        //{
        //    return true;    
        //}
        //else if (result == ContentDialogResult.Secondary)
        //{      
        //    return false;

    
        return false;
    }


    
}
