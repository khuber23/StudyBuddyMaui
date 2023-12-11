using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

//very similiar to the CreateDeckPage, just doesn't deal with DeckGroups and just focuses on User and user Deck. Basically doesn't deal with DeckGroupDeck
public partial class CreateDeckPageNoDeckGroup : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckPageNoDeckGroup()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private async void GoToBuildDeckPage(object sender, EventArgs e)
    {
        Decks = await Constants.GetAllDecks();
        ErrorLabel.IsVisible = false;
        var foundDeck = Decks.FirstOrDefault(x => x.DeckName == DeckNameEntry.Text);
        if (foundDeck != null)
        {
            if (foundDeck.DeckName == DeckNameEntry.Text)
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "Deck already exists. Please choose a different name.";
            }
        }
        else
        {
            //Creates the Deck that the user will make
            Deck = new Deck();
            Deck.DeckName = DeckNameEntry.Text;
            Deck.DeckDescription = DeckDescriptionEntry.Text;
            Deck.ReadOnly = false;
            Deck.IsPublic = IsPublic;
            await Constants.SaveDeckAsync(Deck);

            //get all the Decks and re-find the one so we have an ID...since when posting it the Id would be 0.
            List<Deck> decks = await Constants.GetAllDecks();
            foreach (Deck deck in decks)
            {
                if (deck.DeckName == Deck.DeckName && deck.DeckDescription == Deck.DeckDescription)
                {
                    Deck = deck;
                    break;
                }
            }
            //after getting the right Deck With an ID from the database we make it a userDeck and post it to database.
            UserDeck userDeck = new UserDeck();
            userDeck.DeckId = Deck.DeckId;
            userDeck.UserId = LoggedInUser.UserId;
            //creates the User Deck
            await Constants.SaveUserDeckAsync(userDeck);

            //pass in Deck so then Users can eventually add Flashcards to the deck.
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", Deck }
                };
            //Finishing up making a DeckGroup so now it will take the user to Build Deck
            await Shell.Current.GoToAsync(nameof(BuildDeckPageOnlyDeck), navigationParameter);
        }
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

	public bool IsPublic { get; set; }

    public Deck Deck { get; set; }

    public UserDeck UserDeck { get; set; }

    public User LoggedInUser { get; set; }

    public List<Deck> Decks { get; set; }
}