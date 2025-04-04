
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.UI.Xaml.Documents;


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
            ChannelExtension_ChannelID ="@Kodfgbfdsxcdd",
            ChannelExtension_Name = "מס' שלוחה שגוי",
            ChannelExtension_Short = "content",
            ChannelExtension_Long= "contentmichalyafalala",
            ChannelExtension_Campaign = "1418810",
            ChannelExtension_RunningTime = "11:34",
            ChannelExtension_RunningDay = "שלישי"


        },
       };
       process.InsertData<ChannelExtension>(dd);

        this.InitializeComponent();
        ViewModel = new ViewModels.MainViewModels();    
        this.Loaded += MainPage_Loaded;
      
          YoutubeAPI = new YouTubeMediaHandler();  

    }
    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        process.GetMainPageXamlRoot(this.XamlRoot);
        autoSuggestBox.DataContext = ViewModel;
        RunUpdating.DataContext = ViewModel;
        ChangeDB.DataContext = ViewModel;
        RunCampain.DataContext = ViewModel;
        Password.DataContext = ViewModel;
        ViewModel.AutoSuggestVM.isGetDataFromService();
        comboBoxChannel.DataContext = ViewModel;
        progressBar.DataContext = ViewModel;
        WaitText.DataContext = ViewModel;
        Profile.DataContext = ViewModel;    

    }
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();

   




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
