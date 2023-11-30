using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.Admin;

public partial class AdminHomePage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminHomePage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    public User LoggedInUser { get; set; }
}