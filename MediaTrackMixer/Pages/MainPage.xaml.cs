using MediaTrackMixer.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace MediaTrackMixer;

public sealed partial class MainPage : Page
{
    private MainModel _mainModel;
    private MediaTrackMixer mixer;
    private List<MediaTrackMixer.TrackGroup> mixerTracks;
    public static List<string> AllSupportedTypes = [ ".mkv", ".mp4", ".mp3", ".wav", ".srt" ];
    (string, bool)[] colours =
    [
        ("Magenta", true),
        ("Yellow", true),
        ("Cyan", true),
        ("Green", false),
        ("Blue", false),
        ("DarkCyan", false),
        ("Red", false),
        ("DarkMagenta", false),
    ];
    public MainPage()
    {
        InitializeComponent();
        mixer = new MediaTrackMixer(MainWindow.FfmpegLocation);
        _mainModel = new MainModel{ TrackGroups = new ObservableCollection<TrackGroup>() };
        //AddMedia([
        //    @"C:\Users\Peter Egunjobi\Documents\Shared Folder\The.Walking.Dead.S09.1080p.BluRay.x265-YAWNiX\The.Walking.Dead.S09E02.1080p.BluRay.x265-YAWNiX.mkv",
        //    @"C:\Users\Peter Egunjobi\Downloads\TorrentDownloads\Dexter Original Sin (2024) S01 (1080p AMZN WEB-DL x265 10bit EAC3 5.1 Silence)\Dexter Original Sin (2024) - S01E07 - The Big Bad Body Problem (1080p AMZN WEB-DL x265 Silence).mkv",
        //]);
    }

    private async void ShowFilePicker(object sender, RoutedEventArgs e)
    {
        var filePicker = new FileOpenPicker();
        filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
        AllSupportedTypes.ForEach(t => filePicker.FileTypeFilter.Add(t));
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Window);
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
        var files = await filePicker.PickMultipleFilesAsync();
        await AddMedia(files.Select(f => f.Path).ToArray());
    }

    private async Task AddMedia(string[] inputs)
    {
        if (!File.Exists(MainWindow.FfmpegLocation))
        {
            await ErrorDialog.ShowAsync();
            return;
        }
        mixerTracks = await mixer.GetTracks(inputs);
        var i = 0;
        _mainModel.TrackGroups = new ObservableCollection<TrackGroup>(mixerTracks.Select(t =>
        {
            var (background, hasBlackForeground) = colours[i++ % colours.Length];
            var colour = new Colour { Background = background, HasBlackForeground = hasBlackForeground };
            var tracks = t.Tracks.Select(s => new Track
            {
                FullPath = t.Path,
                Title = s.Title,
                Codec = s.Codec,
                Index = s.Index,
                Type = s.Type,
                Colour = colour,
                FileName = Path.GetFileName(t.Path)
            }).ToList();
            if(t.Chapters.Any()) tracks.Add(new Track
            {
                IsChapter = true, 
                Title = $"{t.Chapters.Count} chapters",
                Colour = colour,
                FileName = Path.GetFileName(t.Path),
                FullPath = t.Path
            });
            return new TrackGroup(tracks)
            {
                FileName = Path.GetFileName(t.Path),
                FullPath = t.Path,
                Colour = tracks.First().Colour,
                Checked = false
            };
        }));
        CollectionViewSourcee.Source = _mainModel.TrackGroups;
        ListVieww.ItemsSource = CollectionViewSourcee.View;

    }

    private void TrackSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var trackGroup in _mainModel.TrackGroups)
        {
            var checkedd = false;
            var notAll = false;
            foreach (var track in trackGroup)
            {
                if (ListVieww.SelectedItems.Contains(track)) checkedd = true;
                else notAll = true;
            }

            trackGroup.Checked = checkedd switch
            {
                true when notAll => null,
                false when notAll => false,
                _ => true
            };
        }

        _mainModel.HasSelectedTracks = ListVieww.SelectedItems.Any();
    }

    private int GetTrackGroupIndex(TrackGroup trackGroup)
    {
        return _mainModel.TrackGroups.TakeWhile(tg => tg != trackGroup).Sum(tg => tg.Count);
    }

    private void TrackGroupChecked(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as CheckBox;
        if (checkbox == null) return;
        var trackGroup = checkbox.DataContext as TrackGroup;
        if (trackGroup == null) return;
        var index = GetTrackGroupIndex(trackGroup);
        if (checkbox.IsChecked == true)
        {
            ListVieww.SelectRange(new ItemIndexRange(index, (uint)trackGroup.Count));
        }
        else
        {
            ListVieww.DeselectRange(new ItemIndexRange(index, (uint)trackGroup.Count));
        }
    }

    private void TrackGroupDeleted(object sender, RoutedEventArgs e)
    {
        _mainModel.TrackGroups.Remove((sender as MenuFlyoutItem).DataContext as TrackGroup);
    }

    private async void MainPage_OnDrop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            await AddMedia(_mainModel.TrackGroups.Select(t => t.FullPath).Concat(items.Select(i => i.Path)).ToArray());
        }
    }

    private void MainPage_OnDragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private void SelectVideoAndAudio(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as MenuFlyoutItem;
        if (checkbox == null) return;
        var trackGroup = checkbox.DataContext as TrackGroup;
        if (trackGroup == null) return;
        trackGroup.Checked = false;
        var index = GetTrackGroupIndex(trackGroup);
        var count = trackGroup.Count(t => t.Type is MediaTrackMixer.TrackType.Video or MediaTrackMixer.TrackType.Audio);
        ListVieww.SelectRange(new ItemIndexRange(index, (uint)count));
    }

    private void SelectSubtitles(object sender, RoutedEventArgs e)
    {
        var checkbox = sender as MenuFlyoutItem;
        if (checkbox == null) return;
        var trackGroup = checkbox.DataContext as TrackGroup;
        if (trackGroup == null) return;
        trackGroup.Checked = false;
        var index = GetTrackGroupIndex(trackGroup);
        index += trackGroup.Count(t => t.Type is MediaTrackMixer.TrackType.Video or MediaTrackMixer.TrackType.Audio);
        var count = trackGroup.Count(t => t.Type == MediaTrackMixer.TrackType.Subtitle);
        ListVieww.SelectRange(new ItemIndexRange(index, (uint)count));
    }

    private void GoToNextPage(object sender, RoutedEventArgs e)
    {
        var transition = new SlideNavigationTransitionInfo();
        transition.Effect = SlideNavigationTransitionEffect.FromRight;
        var selectedTracks = ListVieww.SelectedItems.Cast<Track>().ToList();
        Frame.Navigate(typeof(ProcessingPage), new
        {
            Tracks = selectedTracks.Where(t => t.Type != MediaTrackMixer.TrackType.Other).ToList(),
            MixerTracks = mixerTracks.Where(mt => selectedTracks.Any(t => t.FullPath == mt.Path)).ToList()
        }, transition);
    }

    private void RemoveAllMedia(object sender, RoutedEventArgs e)
    {
        _mainModel.TrackGroups.Clear();
    }
}
