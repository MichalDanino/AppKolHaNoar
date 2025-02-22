using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DTO;
using AppKolHaNoar.Presentation.ViewProcess;
using CommunityToolkit.Mvvm.Input;

namespace AppKolHaNoar.ViewModels;
public class MainViewModels : INotifyPropertyChanged
{
    public ObservableCollection<ChannelExtension> Channels { get; set; }
    public ObservableCollection<Campaign> Campaigns { get; set; }


    // field seelection
    private ActionService actionService; 
    private ChannelExtension _selectedChannel;

    public ICommand SendSelectedChannelCommand { get; }

    //  private Campaign _selectedChannel;
    private Campaign _selectedCampaign;
    public ChannelExtension SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            _selectedChannel = value;
            OnPropertyChanged();
        }
    }
    //public Campaign SelectedCampaign
    //{
    //    get => _selectedCampaign;
    //    set
    //    {
    //        _selectedCampaign = value;
    //        OnPropertyChanged();
    //    }
    //}

    public MainViewModels()
    {
        actionService = new ActionService();
        
         LoadChannels();
        SendSelectedChannelCommand = new RelayCommand(
         () =>  SendSelectedChannelToBackend() // פונקציה שתשלח את הנתונים
        /* CanSend*/ // פונקציה שבודקת האם ניתן לשלוח
     );

    }
    private void SendSelectedChannelToBackend()
    {
        if (SelectedChannel != null) // בודק אם יש ערוץ נבחר
        {
             actionService.UpdateExtension(SelectedChannel.ChannelExtension_ID); // קריאה לפונקציה בשירות
        }
    }

    // פונקציה שמוודאת שהכפתור יהיה פעיל רק אם נבחר ערוץ
    private bool CanSend() => SelectedChannel != null;

    private void LoadChannels()
    {
        
        Channels = new ObservableCollection<ChannelExtension>( actionService.GetDBSet<ChannelExtension>().ToList());
        Campaigns = new ObservableCollection<Campaign>(actionService.GetDBSet<Campaign>().ToList());

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
