

using AppKolHaNoar.Presentation.ViewProcess;
using MediaProcessor;
using MediaProcessor.API;
using Microsoft.UI.Xaml.Controls;
namespace AppKolHaNoar.Presentation;


public sealed partial class MainPage : Page
{
    public YouTubeMediaHandler YoutubeAPI;
    public MainPage()
    {
        YoutubeAPI = new YouTubeMediaHandler();  
        this.InitializeComponent();
    }
    public static YemotHamashichAPI s_YEMOTHAMASHICH = new YemotHamashichAPI();

    private async void UpdateExtension(object sender, RoutedEventArgs e)
    {
        s_YEMOTHAMASHICH.UplaodFiles();
    // this.Frame.Navigate(typeof(Controller.ListPage));

   // await UIProcess.UpdateExtension(this.XamlRoot, " ");


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

#if WINDOWS
    private async Task ShowCustomDialog()
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Custom Dialog",
            Content = "This is a simple dialog example in Uno Platform.",
            CloseButtonText = "Close",
            XamlRoot = this.Content.XamlRoot // חשוב עבור Uno Platform
        };

        await dialog.ShowAsync();
    }
#endif
}
