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
            FfmpegLocation = Path.Join(Package.Current.InstalledLocation.Path, "Assets/ffmpeg.exe");
            //SetIconFromEmbeddedResource($"{nameof(MediaTrackMixer)}.Assets.EncircledPIERound.ico");
            WindowFrame.Navigate(typeof(MainPage));
        }

        ///// <summary>
        ///// Set the Icon for this <see cref="Window"/> out from an EmbeddedResource. If no <see cref="Assembly"/> is specified, the current loaded <see cref="Assembly"/> is used for
        ///// </summary>
        ///// <param name="window"></param>
        ///// <param name="resourceName">The name of the resource</param>
        ///// <param name="assembly">Location of the resource</param>
        //public void SetIconFromEmbeddedResource(string resourceName, Assembly? assembly = null)
        //{                
        //    // https://github.com/microsoft/microsoft-ui-xaml/issues/7782#issuecomment-1266928339
        //    if (assembly == null) assembly = Assembly.GetEntryAssembly();

        //    var rName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));
        //    var icon = new Icon(assembly.GetManifestResourceStream(rName));

        //    AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(icon.Handle));
        //}
    }
}
