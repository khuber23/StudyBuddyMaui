using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateFlashcardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateFlashcardPage()
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