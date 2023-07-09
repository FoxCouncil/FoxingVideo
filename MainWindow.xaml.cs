using System.Windows;

namespace FoxingVideo;

public partial class MainWindow : Window
{
    public FFmpegProfile CurrentProfile
    {
        get { return (FFmpegProfile)GetValue(CurrentProfileProperty); }
        set { SetValue(CurrentProfileProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentProfile.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentProfileProperty =
        DependencyProperty.Register("CurrentProfile", typeof(FFmpegProfile), typeof(MainWindow), new PropertyMetadata());

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        var profileWindow = new ProfileWindow(this);

        profileWindow.ShowDialog();
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        foreach (var file in files)
        {
            var task = new FFmpegTask(file, "b.mp4", CurrentProfile.Arguments);

            await task.Run();
        }
    }
}
