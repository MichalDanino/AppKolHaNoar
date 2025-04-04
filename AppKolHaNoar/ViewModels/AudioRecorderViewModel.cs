using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTO;
using AppKolHaNoar.Services;
using System.Collections;
using System.Runtime.CompilerServices;
using static DTO.Enums;


namespace AppKolHaNoar.ViewModels;
public class AudioRecorderViewModel :MainViewModels
{
    private MediaCapture _mediaCapture;
    private StorageFile _recordedFile;
    private bool _isRecording;
    ServiceUI ServiceUI= new ServiceUI();
    public ICommand StartRecordingCommand { get; }
    public ICommand StopRecordingCommand { get; }
    public ICommand PlayRecordingCommand { get; }

    public AudioRecorderViewModel()
    {
        StartRecordingCommand = new RelayCommand(async () => await StartRecordingAsync());
        StopRecordingCommand = new RelayCommand(async () => await StopRecordingAsync());
        PlayRecordingCommand = new RelayCommand(async () => await PlayRecordingAsync());

    }

    public async Task StartRecordingAsync()
    {
        ServiceUI.ShowMessageByDialog(new GenericMessage(), eDialogType.OK);

        try
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();

            _recordedFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("recordedAudio.m4a", CreationCollisionOption.ReplaceExisting);

            var encodingProfile = MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto);
            await _mediaCapture.StartRecordToStorageFileAsync(encodingProfile, _recordedFile);

            _isRecording = true;
            OnPropertyChanged(nameof(IsRecording));
        }
        catch (Exception ex)
        {
            Console.WriteLine("שגיאה בהקלטה: " + ex.Message);
        }
    }

    public async Task StopRecordingAsync()
    {
        if (_isRecording)
        {
            await _mediaCapture.StopRecordAsync();
            _isRecording = false;
            OnPropertyChanged(nameof(IsRecording));
        }
    }

    public async Task PlayRecordingAsync()
    {
        if (_recordedFile != null)
        {
            var stream = await _recordedFile.OpenAsync(FileAccessMode.Read);
            var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
            mediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromStream(stream, _recordedFile.ContentType);
            mediaPlayer.Play();
        }
    }

    public bool IsRecording => _isRecording;
}
