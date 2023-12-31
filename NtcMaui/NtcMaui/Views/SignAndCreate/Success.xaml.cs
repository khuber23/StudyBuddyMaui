using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.SignAndCreate;

public partial class Success : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public Success()
    {
        InitializeComponent();
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
        WelcomeUserMessage.Text = $"Hello {LoggedInUser.Username}, Welcome Buddy!";
    }

    void OnTapRecognized(object sender, TappedEventArgs args)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
    }

    public User LoggedInUser { get; set; }
}