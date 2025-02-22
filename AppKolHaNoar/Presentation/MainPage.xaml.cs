

using AppKolHaNoar.Presentation.ViewProcess;
using DTO;
using MediaProcessor;
using MediaProcessor.API;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
//using Uno.UI.Controls;
using static DTO.Enums;


namespace AppKolHaNoar.Presentation;



public sealed partial class MainPage : Page
{
    public YouTubeMediaHandler YoutubeAPI;
    public ActionService process;
    public List<ChannelExtension> channelExtensions;
    public ViewModels.MainViewModels ViewModel { get; } = new ViewModels.MainViewModels();

    public MainPage()
    {
        process = new ActionService();
      
        
        this.InitializeComponent();
        process.GetMainPageXamlRoot(this.XamlRoot);   
        comboBoxChannel.DataContext = ViewModel;
        BBb.DataContext = ViewModel;
        // FillComboBox(); 


        YoutubeAPI = new YouTubeMediaHandler();  

    }
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();

    private async void UpdateExtension(object sender, RoutedEventArgs e)
    {
        //Get channel id from ComboBox
        ChannelExtension channelToCheck = comboBoxChannel.SelectedItem as ChannelExtension;
        if (channelToCheck != null)
        {
            await process.UpdateExtension( channelToCheck.ChannelExtension_ID);
        }
        else 
        {
            GenericMessage message = new GenericMessage() { MessageContent = "לא נבחר ערוץ לבצע עליו בדיקה ועדכון השלוחה" };
            await process.ShowMessageByDialog(message, eDialogType.OK);
        }



      //// bool NewVideo =  YoutubeAPI.CheckForNewVideos("gh");
      // if (!NewVideo)
      // { 
      // var dialog = new Controller.CustomDialog("");
      // dialog.XamlRoot= this.Frame.XamlRoot;
      // var f =  await dialog.ShowAsync();
      //     int y = 0;
      //     //להוסיף כאן עדכון שלוחה במקרה שהלקוח רוצה

        // }

    }

    private void AddChannel(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(SubPage.Channel));

    }




    private async Task ShowCustomDialog()
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Custom Dialog",
            Content = "This is a simple dialog example in Uno Platform.",
            CloseButtonText = "Close",
            XamlRoot = this.XamlRoot // חשוב עבור Uno Platform
        };

        await dialog.ShowAsync();
    }

    internal void FillComboBox()
    {
       comboBoxChannel.ItemsSource = ViewModel.Channels;
        comboBoxChannel.DisplayMemberPath = "ChannelExtension_Name";
     comboBoxCampaign.ItemsSource = ViewModel.Campaigns;  
        comboBoxCampaign.DisplayMemberPath = "Campaign_Number";
    }
    private void GetList(object sender, RoutedEventArgs e)
    {

    }

    private void RunCampaign(object sender, RoutedEventArgs e)
    {
       // var r = ViewModel.
        Campaign channelToCheck = comboBoxCampaign.SelectedItem as Campaign;
        process.RunCampaign(channelToCheck,this.XamlRoot);
    }

    //    channelExtensions =new List<ChannelExtension>() {
    //            new ChannelExtension()
    //    {
    //        ChannelExtensionCampaign = "ff",
    //                ChannelExtension_ID = "2342",
    //                ChannelExtension_Name = "הרב יגאל כהן"
    //            },
    //            new ChannelExtension()
    //    {
    //        ChannelExtensionCampaign = "dfvc",
    //                ChannelExtension_ID = "423",
    //                ChannelExtension_Name = "הרב znhr כהן"
    //            }
    //};
}
