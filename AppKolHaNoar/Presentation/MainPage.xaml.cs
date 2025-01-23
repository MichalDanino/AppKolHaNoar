

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

    private async void UpdateExtension(object sender, RoutedEventArgs e)
    {

        bool NewVideo =  YoutubeAPI.CheckForNewVideos("gh");
        if(NewVideo)
        { 
        var dialog = new Controller.CustomDialog("");
        dialog.XamlRoot= this.Frame.XamlRoot;
        await dialog.ShowAsync();
            //להוסיף כאן עדכון שלוחה במקרה שהלקוח רוצה

        }

    }
}
