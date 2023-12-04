using System.ComponentModel;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminDeckPage()
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

        DeckListView.ItemsSource = await Constants.GetAllUserDecks();
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<UserDeck> userDecks = await Constants.GetAllUserDecks();

        if (userDecks != null)
        {
            if (e != null)
            {
                DeckListView.ItemsSource = null;
                DeckListView.ItemsSource = userDecks.Where(ud => ud.Deck.DeckName.Contains(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
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

    //different from deckpage. Only will deal with going directly to edit page.
    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedDeck = e.SelectedItem as UserDeck;
                
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser},
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current Deck", SelectedDeck.Deck}
                };
                    Shell.Current.GoToAsync(nameof(AdminEditDeckPage), navigationParameter);
                }

            }

    public UserDeck SelectedDeck { get; set; }

    public User LoggedInUser { get; set; }
}
