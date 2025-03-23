
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
        List<ChannelExtension> dd = new List<ChannelExtension>()
       { 
            new ChannelExtension()
        {
            ChannelExtension_ChannelID ="@Kol-Halashon",
            ChannelExtension_Name = "fff",
            ChannelExtension_Short = "content",
            ChannelExtension_Long= "content",
            ChannelExtension_Campaign = "1418810",
            ChannelExtension_RunningTime = "11:34",
            ChannelExtension_RunningDay = "שלישי"


        },
            new ChannelExtension()
        {
            ChannelExtension_ChannelID ="@Kol-Halashon",
            ChannelExtension_Name = "fff",
            ChannelExtension_Short = "content",
            ChannelExtension_Long= "content",
            ChannelExtension_Campaign = "1418810",
            ChannelExtension_RunningTime = "11:34",
            ChannelExtension_RunningDay = "רביעי"


        }};
        process.InsertData<ChannelExtension>(dd);

        this.InitializeComponent();
        ViewModel = new ViewModels.MainViewModels();    
        this.Loaded += MainPage_Loaded;
      // FillComboBox();

        YoutubeAPI = new YouTubeMediaHandler();  

    }
    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        process.GetMainPageXamlRoot(this.XamlRoot);
        autoSuggestBox.DataContext = ViewModel;
        RunUpdating.DataContext = ViewModel;
        ChangeDB.DataContext = ViewModel;
        //  RanCampaing.DataContext = ViewModel;
        ComboBoxDay.DataContext = ViewModel;
        TextDay.DataContext = ViewModel;
        Password.DataContext = ViewModel;
        ViewModel.AutoSuggestVM.isGetDataFromService();
        comboBoxChannel.DataContext = ViewModel;
        progressBar.DataContext = ViewModel;
        WaitText.DataContext = ViewModel;   

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
      // comboBoxChannel.ItemsSource = ViewModel.Channels;
       // comboBoxChannel.DisplayMemberPath = "ChannelExtension_Name";
       // autoSuggestBox.ItemsSource = ViewModel.AutoSuggestVM.Items;
       
       // autoSuggestBox.DisplayMemberPath = "Campaign_Name";
    }
   




    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
      if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.AutoSuggestVM.GetFilteredItems(sender.Text);
            if (ViewModel.AutoSuggestVM.Items.Count > 1)
            {
                sender.IsSuggestionListOpen = ViewModel.AutoSuggestVM.Items.Any();
            }
            else
            {
                autoSuggestBox.IsSuggestionListOpen = false;

            }

        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is Campaign selectedCampaign)
        {
           ViewModel.AutoSuggestVM.SelectedText.Campaign_Name = selectedCampaign.Campaign_Name;
           ViewModel.AutoSuggestVM.SelectedText.Campaign_Number = selectedCampaign.Campaign_Number;
            autoSuggestBox.IsSuggestionListOpen = false;

        }
    }

    private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.AutoSuggestVM.ValidateSelection();
        //autoSuggestBox.IsSuggestionListOpen = false;
        autoSuggestBox.Text = ViewModel.AutoSuggestVM.SelectedText.Campaign_Name;

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
