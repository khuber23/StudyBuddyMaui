using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.StudySessionFolder;

public partial class MyStudiesSessionPageOnlyUserDecks : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public MyStudiesSessionPageOnlyUserDecks()
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
        UserDeckGroups = await Constants.GetAllUserDeckGroupByUserId(LoggedInUser.UserId);
        //evemtially add another listView for doing just user Decks without a DeckGroup and deal with it
        UserDecks = await Constants.GetAllUserDecksById(LoggedInUser.UserId);
        //do all this stuff on load to eventually be able to display 1 or the other with the checkbox check
        DecksNotInDeckGroup = new List<UserDeck>();
        List<int> deckIds = new List<int>();
        foreach (UserDeckGroup userdeckgroup in UserDeckGroups)
        {
            //
            userdeckgroup.DeckGroup.DeckGroupDecks = await Constants.GetDeckGroupDecksByDeckGroupId(userdeckgroup.DeckGroupId);
            foreach (DeckGroupDeck deckGroupDeck in userdeckgroup.DeckGroup.DeckGroupDecks)
            {
                deckIds.Add(deckGroupDeck.DeckId);
            }
        }

         UserDecks.Where(userdeck =>  deckIds.Contains(userdeck.DeckId)).ToList().ForEach(userdeck => UserDecks.Remove(userdeck));
        DecksNotInDeckGroup = UserDecks;
        //basically sets the items source to be all the Decks not found in the UserDeckGroups related to a user
        UserDecksOnlyListView.ItemsSource = DecksNotInDeckGroup;
    }

    private void GoToStudyPriorityPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
    }

    private void GoToMyStudiesSessionPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }

	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
	}

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}

	private void GoToMyStudiesSessionPageNoDeckGroups(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPageOnlyUserDecks), navigationParameter);
    }

    //will assign a user Deck when a user clicks on it.
    private void UserDeckOnlyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            ChosenUserDeck = e.SelectedItem as UserDeck;
            ChosenDeckLabel.IsVisible = true;
            ChosenDeckLabel.Text = $"Current Deck Chosen: {ChosenUserDeck.Deck.DeckName}";
        }

    }


    //going to have to re-do this part to deal with probably just user decks.
    //Eventually this code will be used for if a user choosen A UserDeckGroup and can probably do all of the Decks within.
    private void BeginSession(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        if (ChosenUserDeck != null)
        {
            if (ChosenUserDeck.Deck.DeckFlashCards.Count > 0) 
            {
                var navigationParameter = new Dictionary<string, object>
            {
            { "Current User", LoggedInUser },
            {"Chosen Deck", ChosenUserDeck }
            };
                //seperate page for StudyingPagewith just Decks
                Shell.Current.GoToAsync(nameof(StudyingPageNoDeckGroups), navigationParameter);
            }
            else
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Deck has no flashcards to study. Please add some or choose a new deck to study";
            }

        }
        else
        {
            ErrorLabel.IsVisible = true;
            ErrorLabel.Text = "Select a Deck you wish to Study";
        }
    }

    public User LoggedInUser { get; set; }

    public List<UserDeck> UserDecks { get; set; }

    public List<UserDeck> DecksNotInDeckGroup { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }

    public UserDeck ChosenUserDeck { get; set; }

    public List<DeckFlashCard> DeckFlashCards { get; set; }

	private void styleSwitch_Toggled(object sender, ToggledEventArgs e)
	{
		if (e.Value == true)
		{
			var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser },
				};
			Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);

		}
		else if (e.Value == false)
		{
			var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser },
				};
			Shell.Current.GoToAsync(nameof(MyStudiesSessionPageOnlyUserDecks), navigationParameter);
		}
	}
}