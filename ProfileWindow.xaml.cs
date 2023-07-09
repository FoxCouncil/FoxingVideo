using System.Collections.Generic;
using System.Windows;

namespace FoxingVideo;

public partial class ProfileWindow : Window
{
    public MainWindow mainWin;

    public ProfileWindow(MainWindow mainWin)
    {
        InitializeComponent();

        this.mainWin = mainWin;

        ProfileList.SelectionChanged += ProfileList_SelectionChanged;

        RefreshData();
    }

    private void ProfileList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        mainWin.CurrentProfile = ProfileList.SelectedItem as FFmpegProfile;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var response = FFmpegProfile.StoreProfile(new FFmpegProfile(keyTextBox.Text, argsTextBox.Text));

        MessageBox.Show(response);

        RefreshData();

        keyTextBox.Text = argsTextBox.Text = string.Empty;
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        FFmpegProfile.DeleteProfile(ProfileList.SelectedItem?.ToString() ?? "");

        RefreshData();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RefreshData()
    {
        ProfileList.ItemsSource = FFmpegProfile.ListProfiles();
    }
}
