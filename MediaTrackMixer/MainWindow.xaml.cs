using MediaTrackMixer.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaTrackMixer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private MainModel _mainModel;
        public static Window Window;
        public static string FfmpegLocation;
        public MainWindow()
        {
            InitializeComponent();
            Window = this;
            AppWindow.Resize(new SizeInt32(700, AppWindow.Size.Height));
            AppWindow.SetTitleBarIcon("Assets/EncircledPIERound.ico");
            try
            {
                FfmpegLocation = Path.Join(Package.Current.InstalledLocation.Path, "Assets/ffmpeg.exe");
            }
            catch (InvalidOperationException)
            {
                FfmpegLocation = "Assets/ffmpeg.exe";
            }
            WindowFrame.Navigate(typeof(MainPage));
        }
    }
}
