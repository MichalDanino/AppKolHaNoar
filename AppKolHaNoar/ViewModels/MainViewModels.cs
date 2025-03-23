using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DTO;
using AppKolHaNoar.Services;
using System.Diagnostics;

namespace AppKolHaNoar.ViewModels;
public class MainViewModels : INotifyPropertyChanged
{
    public AutoSuggestViewModels AutoSuggestVM { get; set; }
    public DataTimeViewModel DataTimeVM { get; set; }
    public ObservableCollection<ChannelExtension> Channels { get; set; }
    public ObservableCollection<Campaign> Campaigns { get; set; }

    private bool _isCalculating;
    public bool IsCalculating
    {
        get => _isCalculating;
        set { _isCalculating = value; OnPropertyChanged();
            OnPropertyChanged(nameof(LoadingVisibility)); // עדכון Visibility
        }
    }
    private string _progressText = "מתבצע חישוב...";

    public string ProgressText
    {
        get => _progressText;
        set
        {
            _progressText = value;
            OnPropertyChanged();
        }
    }
    public Visibility LoadingVisibility => IsCalculating ? Visibility.Visible : Visibility.Collapsed;

    // field seelection
    private ServiceUI actionService;
    private ChannelExtension _selectedChannel;

    public ICommand SendSelectedChannelCommand { get; }
    public ICommand SendSelectedinfoCommand { get; }
    public ICommand SendSelectedCampainCommand { get; }
    public ICommand ShowDialogToChangePasswordCommand { get; }

    public DateTime Date { get; set; } = DateTime.Now;
    public TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;

    public void Submit()
    {
        DateTime selectedDateTime = Date.Add(Time);
        //Debug.Log("Selected DateTime: " + selectedDateTime);
    }
    //  private Campaign _selectedChannel;
    private Campaign _selectedCampaign;
    public ChannelExtension SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            if (_selectedChannel != value)  // בדוק אם הערך השתנה
            {
                _selectedChannel = value;
                OnPropertyChanged();
            }
        }
    }
    public Campaign SelectedCampaign
    {
        get => _selectedCampaign;
        set
        {
            _selectedCampaign = value;
            OnPropertyChanged();
        }
    }

    public MainViewModels()
    {
        AutoSuggestVM = new AutoSuggestViewModels();
        actionService = new ServiceUI();
        DataTimeVM = new DataTimeViewModel();

        LoadChannels();
        SendSelectedChannelCommand = new RelayCommand(
         () => SendSelectedChannelToBackend()

     );
        SendSelectedinfoCommand = new RelayCommand(
            () => SendSelectedinfoToBackend()
            );
        SendSelectedCampainCommand = new RelayCommand(
            () => SendSelectedCampainToBackend() 
            );
        ShowDialogToChangePasswordCommand = new RelayCommand(
           () => ShowDialogToChangePassword()
           );

    }
    

    /// <summary>
    /// 
    /// </summary>
    private async Task SendSelectedChannelToBackend()
    {
        IsCalculating = true;
        // actionService.dddd();
        //If a channel is selected 
        if (SelectedChannel != null)
        {
           await  actionService.UpdateExtension(SelectedChannel.ChannelExtension_ChannelID);
        
        }
        else
        {
            GenericMessage message = new GenericMessage() { MessageContent = " לא נבחר ערוץ לבצע עליו את ההרצה" };
           await actionService.ShowMessageByDialog(message, Enums.eDialogType.OK);
        }
        ProgressText = "ערוץ";
      //  IsCalculating = false;
    }

    private void SendSelectedCampainToBackend()
    {
        // actionService.dddd();
       
            actionService.RunCampaign(AutoSuggestVM.SelectedText);

     
    }

    private async void ShowDialogToChangePassword()
    {
        GenericMessage message = new GenericMessage() { MessageTitle = "שינוי סיסמאות" };
        await actionService.ShowMessageByDialog(message, Enums.eDialogType.MultyButton);
    }

    private async void SendSelectedinfoToBackend()
    {
        IsCalculating = true;
        bool f = await actionService.ChangeDB(); // קריאה לפונקציה בשירות
        UpdateDB();
        IsCalculating = false;


    }

    // פונקציה שמוודאת שהכפתור יהיה פעיל רק אם נבחר ערוץ
    private bool CanSend() => SelectedChannel != null;

    private void LoadChannels()
    {

        Channels = new ObservableCollection<ChannelExtension>(actionService.GetDBSet<ChannelExtension>().ToList());

    }
    private void UpdateDB()
    {
        Channels.Clear();
        // טוען את הערוצים מחדש מה-DB
        var y = new ObservableCollection<ChannelExtension>(actionService.GetDBSet<ChannelExtension>().ToList());
        foreach (var channel in y)
        {
            Channels.Add(channel);
        }

    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
