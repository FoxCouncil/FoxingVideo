using Ookii.Dialogs.Wpf;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;

namespace FoxingVideo;

public partial class MainWindow : Window
{
    const string DbConnectionString = "Data Source=foxingvideo.db;Version=3;";

    public FFmpegProfile CurrentProfile
    {
        get { return (FFmpegProfile)GetValue(CurrentProfileProperty); }
        set { SetValue(CurrentProfileProperty, value); }
    }

    public static readonly DependencyProperty CurrentProfileProperty = DependencyProperty.Register("CurrentProfile", typeof(FFmpegProfile), typeof(MainWindow), new PropertyMetadata());

    public string CurrentOutputDirectory
    {
        get { return (string)GetValue(CurrentOutputDirectoryProperty); }
        set { SetValue(CurrentOutputDirectoryProperty, value); }
    }

    public static readonly DependencyProperty CurrentOutputDirectoryProperty = DependencyProperty.Register("CurrentOutputDirectory", typeof(string), typeof(MainWindow), new PropertyMetadata());


    public MainWindow()
    {
        InitializeComponent();

        InitializeDb();

        CurrentOutputDirectory = RetrieveCurrentDirectory();

        CurrentProfile = FFmpegProfile.ListProfiles().First();
    }

    void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        var profileWindow = new ProfileWindow(this);

        profileWindow.ShowDialog();
    }

    async void Window_Drop(object sender, DragEventArgs e)
    {
        if (CurrentOutputDirectory == null)
        {
            MessageBox.Show("Uh, output directory not set, wtf...");
            return;
        }

        if (CurrentProfile == null)
        {
            MessageBox.Show("Uh, profile not set, wtf...");
            return;
        }

        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        foreach (var file in files)
        {
            var task = new FFmpegTask(CurrentProfile, file, CurrentOutputDirectory);

            await task.Run();
        }
    }

    static void InitializeDb()
    {
        using var conn = new SQLiteConnection(DbConnectionString);

        conn.Open();

        string sql = @"
        CREATE TABLE IF NOT EXISTS Config
        (
            Id INTEGER PRIMARY KEY,
            CurrentDirectory TEXT
        );";

        using var cmd = new SQLiteCommand(sql, conn);

        cmd.ExecuteNonQuery();
    }

    static void StoreCurrentDirectory(string directoryPath)
    {
        using var conn = new SQLiteConnection(DbConnectionString);

        conn.Open();

        var sql = "INSERT INTO Config (CurrentDirectory) VALUES (@dirPath);";

        using var cmd = new SQLiteCommand(sql, conn);

        cmd.Parameters.AddWithValue("@dirPath", directoryPath);

        cmd.ExecuteNonQuery();
    }

    static string RetrieveCurrentDirectory()
    {
        using var conn = new SQLiteConnection(DbConnectionString);

        conn.Open();

        var sql = "SELECT CurrentDirectory FROM Config ORDER BY Id DESC LIMIT 1;";

        using var cmd = new SQLiteCommand(sql, conn);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return reader["CurrentDirectory"].ToString();
        }

        // If no directory was found in the database, get and store the current directory.
        var currentDirectory = Directory.GetCurrentDirectory();

        StoreCurrentDirectory(currentDirectory);

        return currentDirectory;
    }

    private void OutputDirectoryButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = "Please select a folder.";
        dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

        if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
        {
            MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
        }

        if ((bool)dialog.ShowDialog(this))
        {
            MessageBox.Show(this, $"The selected folder was:{Environment.NewLine}{dialog.SelectedPath}", "Sample folder browser dialog");
        }

    }
}
