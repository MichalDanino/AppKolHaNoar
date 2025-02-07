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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppKolHaNoar.Presentation.SubPage;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Channel : Page
{
    public Channel()
    {
        this.InitializeComponent();
    }

    private void SaveChannel(object sender, RoutedEventArgs e)
    {
        List  <ChannelExtension> channelExtension = new List<ChannelExtension>(); 

    }
}
