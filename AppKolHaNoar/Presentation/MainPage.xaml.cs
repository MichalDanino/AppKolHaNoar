
using MediaProcessor;
using MediaProcessor.API;
namespace AppKolHaNoar.Presentation;

public sealed partial class MainPage : Page
{
    public YouTubeMediaHandler YoutubeAPI;
    public MainPage()
    {
        YoutubeAPI = new YouTubeMediaHandler();  
        this.InitializeComponent();
    }

    private void UpdateExtension(object sender, RoutedEventArgs e)
    {
        YoutubeAPI.CheckForNewVideos("gh");
    }
}
