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
using Windows.Services.Store;
using AppKolHaNoar.Services;
using static DTO.Enums;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppKolHaNoar.Presentation.SubPage;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Channel : Page
{
    static ServiceUI UI;
    public Channel()
    {
        this.InitializeComponent();

    }

    private async void SaveChannel(object sender, RoutedEventArgs e)
    {
        UI = new ServiceUI();
       List< ChannelExtension> channelExtension = new List<ChannelExtension>()
       {
           new ChannelExtension()
           {
                ChannelExtension_ChannelID = ChannelURL.Text,
                ChannelExtension_Long = longVideo.Text,
                ChannelExtension_Short = shortVideo.Text,
                ChannelExtension_Campaign = Campaign.Text
           }
       }; 
       

       bool isAdd =  UI.InsertData< ChannelExtension>(channelExtension);
        if (isAdd)
        {
            GenericMessage message = new GenericMessage() { MessageContent = " המידע נשמר בהצלחה" };
           await UI.ShowMessageByDialog(message, eDialogType.OK);
        }
        this.Frame.Navigate(typeof(MainPage));

    }



}
