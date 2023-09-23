using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private void GoToBuildDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(BuildDeckGroupPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }
}