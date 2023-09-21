using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

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

    public async void GoToMyStudies(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }
    public User LoggedInUser { get; set; }
}