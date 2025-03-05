
using AppKolHaNoar.Services;

using DTO;
using MediaProcessor;
using MediaProcessor.API;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
//using Uno.UI.Controls;
using static DTO.Enums;
using static DTO.Campaign;
using AppKolHaNoar.ViewModels;


namespace AppKolHaNoar.Presentation;



public sealed partial class MainPage : Page
{
    public YouTubeMediaHandler YoutubeAPI;
    public ServiceUI process;
    public List<ChannelExtension> channelExtensions;
    public ViewModels.MainViewModels ViewModel;
    public MainPage()
    {
        process = new ServiceUI();
        List<Campaign> dd = new List<Campaign>()
       { new Campaign()
        {
            Campaign_Number ="812"
        }};
        process.InsertData<Campaign>(dd);

        this.InitializeComponent();
        ViewModel = new ViewModels.MainViewModels();    
        this.Loaded += MainPage_Loaded;
        //process.GetMainPageXamlRoot(this.XamlRoot);   
       // comboBoxChannel.DataContext = ViewModel;
       // BBb.DataContext = ViewModel;
       // ChangeDB.DataContext = ViewModel;
        FillComboBox();

        YoutubeAPI = new YouTubeMediaHandler();  

    }
    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        process.GetMainPageXamlRoot(this.XamlRoot);
        autoSuggestBox.DataContext = ViewModel;
        BBb.DataContext = ViewModel;
        ChangeDB.DataContext = ViewModel;
    }
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();

    private async void UpdateExtension(object sender, RoutedEventArgs e)
    {
        //Get channel id from ComboBox
        ChannelExtension channelToCheck = comboBoxChannel.SelectedItem as ChannelExtension;
        if (channelToCheck != null)
        {
            await process.UpdateExtension( channelToCheck.ChannelExtension_ChannelID);
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
        



        // this.Frame.Navigate(typeof(SubPage.Channel));

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

     //comboBoxCampaign.ItemsSource = ViewModel.Campaigns;  
       // comboBoxCampaign.DisplayMemberPath = "Campaign_Number";
    }
   

    private void RunCampaign(object sender, RoutedEventArgs e)
    {
        // var r = ViewModel.
        Campaign channelToCheck = null;// comboBoxCampaign.SelectedItem as Campaign;
        process.RunCampaign(channelToCheck,this.XamlRoot);
    }


    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.AutoSuggestVM.GetFilteredItems(sender.Text);
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        ViewModel.AutoSuggestVM.SelectedText = args.SelectedItem.ToString();
    }

    private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.AutoSuggestVM.ValidateSelection();
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
//channelExtensions = new List<ChannelExtension>() {
//                    new ChannelExtension()
//{
//    ChannelExtension_Campaign = "ff",
//                        ChannelExtension_ChannelID = "2342",
//                        ChannelExtension_Name = "הרב יגאל כהן"
//                    },
//                    new ChannelExtension()
//{
//    ChannelExtension_Campaign = "dfvc",
//                        ChannelExtension_ChannelID = "423",
//                        ChannelExtension_Name = "הרב znhr כהן"
//                    }
//        };
