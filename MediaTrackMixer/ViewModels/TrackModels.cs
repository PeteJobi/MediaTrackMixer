using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaTrackMixer.ViewModels;

public class Track : INotifyPropertyChanged
{
    public bool OnSecondPage { get; set; }
    public string? FileName { get; set; }
    public string FullPath { get; set; }
    public MediaTrackMixer.TrackType Type { get; set; }
    public string Colour { get; set; }
    public int Index { get; set; }
    public string Codec { get; set; }
    public bool IsChapter { get; set; }
    private string _Title;
    public string Title
    {
        get => _Title;
        set
        {
            _Title = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasTitle));
            OnPropertyChanged(nameof(HasNoTitle));
        }
    }
    private bool _ineditmode;
    public bool InEditMode
    {
        get => _ineditmode;
        set
        {
            _ineditmode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NotInEditMode));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string IndexString => IsChapter ? string.Empty : (Index + 1).ToString();
    public string? FileNameString => OnSecondPage ? FileName : null;
    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);
    public bool HasNoTitle => !HasTitle;
    public bool NotInEditMode => !InEditMode;
    public string Icon => IsChapter ? "\uE8F1" : Type switch
    {
        MediaTrackMixer.TrackType.Video => "\uE714",
        MediaTrackMixer.TrackType.Audio => "\uE8D6",
        MediaTrackMixer.TrackType.Subtitle => "\uED1E",
        _ => "\uE9CE"
    };
    public string ToolTip => IsChapter ? "Chapters" : Type switch
    {
        MediaTrackMixer.TrackType.Video => "Video",
        MediaTrackMixer.TrackType.Audio => "Audio",
        MediaTrackMixer.TrackType.Subtitle => "Subtitle",
        _ => "Unknown"
    };
    public bool NotOnSecondPage => !OnSecondPage;
    public bool IsNotChapter => !IsChapter;
}

public class TrackGroup : List<Track>, INotifyPropertyChanged
{
    public TrackGroup(IEnumerable<Track> items) : base(items)
    {
    }
    private bool? _checked;
    public bool? Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            OnPropertyChanged();
        }
    }
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public string Colour { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}