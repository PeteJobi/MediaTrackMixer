using MediaTrackMixer.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaTrackMixer;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProcessingPage : Page
{
    private ProcessingPageModel viewModel;
    private MediaTrackMixer mixer;
    private List<MediaTrackMixer.TrackGroup> mixerTracks;

    public ProcessingPage()
    {
        InitializeComponent();
        mixer = new MediaTrackMixer(MainWindow.FfmpegLocation);
        viewModel = new ProcessingPageModel { Tracks = [] };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        dynamic? obj = e.Parameter;
        viewModel = new ProcessingPageModel
        {
            Tracks = new ObservableCollection<Track>((List<Track>)obj.Tracks)
        };
        mixerTracks = obj.MixerTracks;
        foreach (var track in viewModel.Tracks)
        {
            track.OnSecondPage = true;
        }
        base.OnNavigatedTo(e);
    }

    private (string, PickerLocationId, IEnumerable<string>) GetSaveParameters()
    {
        var containsVideo = false;
        var containsChapters = false;
        var containsAudio = false;
        var containsSubtitles = false;
        foreach (var track in viewModel.Tracks)
        {
            if(track.IsChapter) containsChapters = true;
            else switch (track.Type)
            {
                case MediaTrackMixer.TrackType.Video:
                    containsVideo = true;
                    break;
                case MediaTrackMixer.TrackType.Audio:
                    containsAudio = true;
                    break;
                case MediaTrackMixer.TrackType.Subtitle:
                    containsSubtitles = true;
                    break;
            }
        }

        if (containsChapters || (containsVideo && containsSubtitles) || (containsAudio && containsSubtitles))
        {
            return ("New video", PickerLocationId.VideosLibrary, FileTypeChoices(".mkv"));
        }

        if (containsVideo)
        {
            return ("New video", PickerLocationId.VideosLibrary, FileTypeChoices(".mp4"));
        }

        if (containsAudio && viewModel.Tracks.Count > 1)
        {
            return ("New multi-track audio", PickerLocationId.VideosLibrary, FileTypeChoices(".mp4"));
        }

        if (containsAudio)
        {
            return ("New audio", PickerLocationId.MusicLibrary, FileTypeChoices(".mp3"));
        }

        if (containsSubtitles)
        {
            return ("New subtitles", PickerLocationId.DocumentsLibrary, FileTypeChoices(".srt"));
        }

        return ("New file", PickerLocationId.Downloads, FileTypeChoices(".mp4"));

        IEnumerable<string> FileTypeChoices(string type) => MainPage.AllSupportedTypes.Where(t => t != type).Prepend(type);
    }

    private List<MediaTrackMixer.Map> GetMaps() => viewModel.Tracks.Select(tr => new MediaTrackMixer.Map(
        mixerTracks.FindIndex(tg => tg.Path == tr.FullPath), tr.IsChapter ? 0 : tr.Index, tr.IsChapter, tr.IsChapter ? null : tr.Title)).ToList();

    private async void Save(object sender, RoutedEventArgs e)
    {
        var fileSaver = new FileSavePicker();
        var (suggestedFileName, suggestedStartLocation, fileTypeChoices) = GetSaveParameters();
        fileSaver.SuggestedFileName = suggestedFileName;
        fileSaver.SuggestedStartLocation = suggestedStartLocation;
        fileSaver.FileTypeChoices.Add("Media files", fileTypeChoices.ToList());
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Window);
        WinRT.Interop.InitializeWithWindow.Initialize(fileSaver, hwnd);
        var file = await fileSaver.PickSaveFileAsync();
        if (file == null) return;
        viewModel.OperationVisibility = OperationVisibility.ShowProgress;
        var maps = GetMaps();
        var progress = new Progress<double>(progress =>
        {
            ProgressBar.Value = progress;
            ProgressText.Text = $"{Math.Round(progress, 2)}%";
        });
        bool success;
        try
        {
            await mixer.Mix(mixerTracks, file.Path, maps, progress);
            success = true;
        }
        catch (Exception)
        {
            success = false;
        }
        viewModel.OperationVisibility = OperationVisibility.ShowOnlyBack;
        viewModel.ShowInfoBar = true;
        viewModel.InfoBarSeverity = success ? InfoBarSeverity.Success : InfoBarSeverity.Error;
        viewModel.InfoBarMessage = success ? "Track mix completed successfully" : "Track mix was not successful";
    }

    private void GoBack(object sender, RoutedEventArgs e)
    {
        var transition = new SlideNavigationTransitionInfo();
        transition.Effect = SlideNavigationTransitionEffect.FromLeft;
        Frame.GoBack(transition);
    }
}
