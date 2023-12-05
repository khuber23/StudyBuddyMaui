using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Share;

public partial class ShareDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckPage()
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
        UserDecks = await Constants.GetAllUserDecksById(LoggedInUser.UserId);
        List<UserDeck> DecksNotInDeckGroup = new List<UserDeck>();
        List<int> deckIds = new List<int>();
        foreach (UserDeckGroup userdeckgroup in UserDeckGroups)
        {
            //test
            userdeckgroup.DeckGroup.DeckGroupDecks = await Constants.GetDeckGroupDecksByDeckGroupId(userdeckgroup.DeckGroupId);
                foreach (DeckGroupDeck deckGroupDeck in userdeckgroup.DeckGroup.DeckGroupDecks)
                {
                    deckIds.Add(deckGroupDeck.DeckId);
                }
        
        }

        UserDecks.Where(userdeck => deckIds.Contains(userdeck.DeckId)).ToList().ForEach(userdeck => UserDecks.Remove(userdeck));
        DecksNotInDeckGroup = UserDecks;
        //basically sets the items source to be all the Decks not found in the UserDeckGroups related to a user
        if (DecksNotInDeckGroup.Count == 0)
        {
            //throw an error saying you have no decks not in a deck group or if they just don't have decks.
            ErrorLabel.Text = "You don't have any Decks NOT in a Deck Group or You have NOT created any Decks.";
            ErrorLabel2.Text = "Please create a Deck or share a Deck Group";

			ErrorLabel.IsVisible = true;
			ErrorLabel2.IsVisible = true;
		}
        DeckPicker.ItemsSource = DecksNotInDeckGroup;
    }

    //gets the Deckname when a user chooses an option
    private void DeckPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this deckname to eventually find a deck in the next btn.
            ErrorLabel.IsVisible = false;
            DeckName = picker.Items[selectedIndex];
        }
    }

    //will take them to the next page dealing with Copying a deck to a user
    private async void CopyBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        Deck selectedDeck = await Constants.GetDeckByDeckName(DeckName);
        if (selectedDeck == null || selectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeck = selectedDeck;
            string chosenShareType = "Copy";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current Deck" , SelectedDeck},
                {"Share Type", chosenShareType }
                };
            await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
        }
    }

    //will take them to the next page dealing with cloning a deck to a user
    private async void CloneBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        Deck selectedDeck = await Constants.GetDeckByDeckName(DeckName);
        if (selectedDeck == null || selectedDeck.DeckId == 0)
        {
            ErrorLabel.Text = "Please Select a deck from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeck = selectedDeck;
            string chosenShareType = "Clone";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current Deck" , SelectedDeck},
                {"Share Type", chosenShareType }
                };
            await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
        }
    }

	private void GoToHomePage(object sender, EventArgs e)
	{
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(HomePage), navigationParameter);
	}

	private void GoToDashboardPage(object sender, EventArgs e)
	{
		//eventually make this the dashboard page and also send the user through to this page.
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
	}

	private void GoToFlashcardPage(object sender, EventArgs e)
	{
		//eventually make this the dashboard page and also send the user through to this page.
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(FlashcardPage), navigationParameter);
	}
	private void GoToDeckPage(object sender, EventArgs e)
	{
		//eventually make this the dashboard page and also send the user through to this page.
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
	}

	private void GoToDeckGroupPage(object sender, EventArgs e)
	{
		//eventually make this the dashboard page and also send the user through to this page.
		var navigationParameter = new Dictionary<string, object>
				{
					{ "Current User", LoggedInUser }
				};
		Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
	}


	public User LoggedInUser { get; set; }

    public string DeckName { get; set; }

    public Deck SelectedDeck { get; set; }

    public List<UserDeck> UserDecks { get; set; }

    public List<UserDeckGroup> UserDeckGroups { get; set; }


}