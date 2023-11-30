using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Admin;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.StudySessionFolder;

namespace NtcMaui.Views.SignAndCreate;

public partial class HomePage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public HomePage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
        UserNameText.Text = LoggedInUser.Username;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (LoggedInUser.ProfilePicture == null || LoggedInUser.ProfilePicture == string.Empty)
        {
            UserImage.Source = "stockprofileimage.png";
        }
        else
        {
            UserImage.Source = LoggedInUser.ProfilePicture;
        }
    }

    public async void GoToMyStudies(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    public async void GoToStudySession(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }
    public async void GoToAdminHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(AdminHomePage), navigationParameter);
    }
    public User LoggedInUser { get; set; }
}