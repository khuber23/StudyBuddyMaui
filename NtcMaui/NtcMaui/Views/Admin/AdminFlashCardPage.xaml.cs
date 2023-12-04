using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminFlashCardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminFlashCardPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        FlashcardListView.ItemsSource = await Constants.GetAllFlashCards();
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<FlashCard> flashCards = await Constants.GetAllFlashCards();

        if (flashCards != null)
        {
            if (e != null)
            {
                FlashcardListView.ItemsSource = null;
                FlashcardListView.ItemsSource = flashCards.Where(f => f.FlashCardQuestion.Contains(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
        }

    }

    private void FlashcardListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedFlashcard = e.SelectedItem as FlashCard;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current FlashCard", SelectedFlashcard }
                };
            Shell.Current.GoToAsync(nameof(AdminEditFlashCardPage), navigationParameter);
        }
    }

//tabs
private void GoToHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
    }

    private void GoToAdminHomePage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminHomePage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminFlashCardPage), navigationParameter);
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminDeckPage), navigationParameter);

    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminDeckGroupPage), navigationParameter);
    }

    private void GoToUsersPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(AdminUsersPage), navigationParameter);
    }

    public User LoggedInUser { get; set; }

    public FlashCard SelectedFlashcard { get; set; }
}