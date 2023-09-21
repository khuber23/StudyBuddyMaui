using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
	}

    private  void GoToDashboardPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DashboardPage));
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }
    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(FlashcardPage));
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeckPage));
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeckGroupPage));
    }

    public User LoggedInUser { get; set; }
}