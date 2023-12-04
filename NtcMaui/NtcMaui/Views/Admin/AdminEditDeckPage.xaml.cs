using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminEditDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminEditDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        List<Deck> decks = await Constants.GetAllDecks();
        DecksToEdit = decks.Where(d => d.DeckName == SelectedDeck.DeckName).ToList();
        UserDecks = await Constants.GetAllUserDecks();
        UserDecks = UserDecks.Where(d => d.DeckId == SelectedDeck.DeckId || d.Deck.DeckName == SelectedDeck.DeckName).ToList();
        DeckGroupDecks = await Constants.GetAllDeckGroupDecks();
        DeckGroupDecks = DeckGroupDecks.Where(deckgroupdeck => deckgroupdeck.DeckId == SelectedDeck.DeckId).ToList();
        DeckNameEntry.Text = SelectedDeck.DeckName;
        DeckDescriptionEntry.Text = SelectedDeck.DeckDescription;
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = true;
        FinishDeleteBtn.IsVisible = false;
        CancelBtn.IsVisible = false;
        WarningLabel.IsVisible = false;
    }

    private void DeleteBtn_Clicked(object sender, EventArgs e)
    {
        FinishEditingBtn.IsVisible = false;
        FinishDeleteBtn.IsVisible = true;
        CancelBtn.IsVisible = true;
        WarningLabel.Text = $"Warning: You are about to delete {SelectedDeck.DeckName}";
        WarningLabel.IsVisible = true;
    }

    private async void FinishDeleteBtn_Clicked(object sender, EventArgs e)
    {
        //get the userDeckGroups and get the ones where by the name.
        foreach (UserDeck userDeck in UserDecks)
        {
            await DeleteUserDeckAsync(userDeck.UserId, userDeck.DeckId);
        }
        //essentially if there is a deck relating to DeckGroupDecks
        //so if a user is trying to delete a deck in a deckgroup we need to delete the userDeck as well as the deckgroupdeck
        if (DeckGroupDecks.Count > 0)
        {
            foreach (DeckGroupDeck groupDeck in DeckGroupDecks)
            {
                await DeleteDeckGroupDeckAsync(groupDeck.DeckGroupId, groupDeck.DeckId);
            }
        }

        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(AdminDeckPage), navigationParameter);

    }


    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        foreach (Deck deck in DecksToEdit)
        {
            SelectedDeck = deck;
            SelectedDeck.DeckName = DeckNameEntry.Text;
            SelectedDeck.DeckDescription = DeckDescriptionEntry.Text;
//not worrying about public as that should be left up to users themselves. (Could also mess up code later on)
            await PutDeckAsync(SelectedDeck);
        }

        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(AdminDeckPage), navigationParameter);
    }

    private void IsPublicCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            IsPublic = true;
        }
        else
        {
            IsPublic = false;
        }
    }

    /// <summary>
    /// Does a Put command on the deck
    /// </summary>
    /// <param name="deck">the deck you are updating</param>
    /// <returns>updated deck</returns>
    public async Task PutDeckAsync(Deck deck)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck/%7Bid%7D?deckid={SelectedDeck.DeckId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tdeck successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteDeckGroupDeckAsync(int deckGroupId, int deckId)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroupDeck/{deckGroupId}/{deckId}", string.Empty));

        try
        {
            HttpResponseMessage response = await Constants._client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\Item successfully deleted.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteUserDeckAsync(int userId, int deckId)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck/{userId}/{deckId}", string.Empty));

        try
        {
            HttpResponseMessage response = await Constants._client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\Item successfully deleted.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
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

    public bool IsPublic { get; set; }

    public User LoggedInUser { get; set; }

    public Deck SelectedDeck { get; set; }

    //gets a list of all the decks that can be editable (used for shared decks mostly to updated stuff for shared)
    public List<Deck> DecksToEdit { get; set; }

    public List<DeckGroupDeck> DeckGroupDecks { get; set; }

    public List<UserDeck> UserDecks { get; set; }
}