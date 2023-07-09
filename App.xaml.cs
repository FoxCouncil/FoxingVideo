using System.Windows;

namespace FoxingVideo;

public partial class App : Application
{
    public App()
    {
        FFmpegProfile.Initialize();
    }
}
