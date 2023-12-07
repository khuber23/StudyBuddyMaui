using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

//might eventually need to copy this page and make a different one just for dealing with creatingUSerDeckGroup and just creating a UserDeck.
//most definetly since this code deals with DeckGroupDecks and if there is just a UserDeck I won't need this and I can't deal with nulls when applying Query attributes.
public partial class CreateDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        UserDeckGroups = await Constants.GetAllUserDeckGroups();
        Decks = await Constants.GetAllDecks();
        foreach (UserDeckGroup group in UserDeckGroups)
        {
            if (group.DeckGroupId == SelectedDeckGroup.DeckGroupId)
            {
                SameDeckGroupsById.Add(group);
            }
            else if (group.DeckGroupId != SelectedDeckGroup.DeckGroupId && group.DeckGroup.DeckGroupName == SelectedDeckGroup.DeckGroupName)
            {
                NotSameDeckGroupsById.Add(group);
            }
        }

    }

    private async void GoToBuildDeckPage(object sender, EventArgs e)
    {
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
        //maybe split it up?
        //bool for this
        bool deckCreated = false;
           foreach(UserDeckGroup deckgroup in SameDeckGroupsById)
        {
            if (deckCreated == false)
            {
                Deck = new Deck();
                Deck.DeckName = DeckNameEntry.Text;
                Deck.DeckDescription = DeckDescriptionEntry.Text;
                Deck.IsPublic = IsPublic;
                Deck.ReadOnly = false;

                await Constants.SaveDeckAsync(Deck);

                //get all the Decks and re-find the one so we have an ID...since when posting it the Id would be 0.
                List<Deck> decks = await Constants.GetAllDecks();
                Deck = decks.FirstOrDefault(d => d.DeckName == Deck.DeckName && d.DeckDescription == Deck.DeckDescription);
                deckCreated = true;

                //after getting the right Deck With an ID from the database we make it a userDeck and post it to database.
                //need all 3
                UserDeck userDeck = new UserDeck();
                userDeck.DeckId = Deck.DeckId;
                userDeck.UserId = deckgroup.UserId;
                //creates the User Deck
                await Constants.SaveUserDeckAsync(userDeck);

                //need a method to create the DeckGroupDeck part to combine the DeckGroup with the Created Deck.
                DeckGroupDeck deckGroupDeck = new DeckGroupDeck();
                deckGroupDeck.DeckGroupId = deckgroup.DeckGroupId;
                deckGroupDeck.DeckId = Deck.DeckId;
                DirectCopiedDeckGroupDeck = deckGroupDeck;
                await Constants.SaveDeckGroupDeckAsync(deckGroupDeck);
                if (LoggedInUser.UserId == deckgroup.UserId)
                {
                    CurrentDeckGroupDeck = await Constants.GetSpecificDeckGroupDeck(deckGroupDeck.DeckGroupId, deckGroupDeck.DeckId);
                }

            }
            //deckGroupDeck and Deck will already be made once, since direct shared access.
            //we only need to make a userDeck showing each user shared has access to the same deck.
            else
            {
                UserDeck userDeck = new UserDeck();
                userDeck.DeckId = Deck.DeckId;
                userDeck.UserId = deckgroup.UserId;
                //creates the User Deck
                await Constants.SaveUserDeckAsync(userDeck);

                //need a method to create the DeckGroupDeck part to combine the DeckGroup with the Created Deck.
                //already would be created
                DeckGroupDeck deckGroupDeck = new DeckGroupDeck();
                deckGroupDeck.DeckGroupId = deckgroup.DeckGroupId;
                deckGroupDeck.DeckId = Deck.DeckId;
                await Constants.SaveDeckGroupDeckAsync(deckGroupDeck);

                if (LoggedInUser.UserId == deckgroup.UserId)
                {
                    CurrentDeckGroupDeck = await Constants.GetSpecificDeckGroupDeck(DirectCopiedDeckGroupDeck.DeckGroupId, DirectCopiedDeckGroupDeck.DeckId);
                }
            }
            
        }

           foreach(UserDeckGroup deckgroup in NotSameDeckGroupsById)
        {
            Deck = new Deck();
            Deck.DeckName = DeckNameEntry.Text;
            Deck.DeckDescription = DeckDescriptionEntry.Text;
            Deck.IsPublic = false;
            Deck.ReadOnly = true;

            await Constants.SaveDeckAsync(Deck);

            //get all the Decks and re-find the one so we have an ID...since when posting it the Id would be 0.
            List<Deck> decks = await Constants.GetAllDecks();
            List<Deck> sameDecks = decks.Where(d => d.DeckName == Deck.DeckName && d.DeckDescription == Deck.DeckDescription).ToList();
            //gets the newest item/most recently created Deck that will be used for the cloning
            Deck = sameDecks.Last();

            //after getting the right Deck With an ID from the database we make it a userDeck and post it to database.
            //need all 3
            UserDeck userDeck = new UserDeck();
            userDeck.DeckId = Deck.DeckId;
            userDeck.UserId = deckgroup.UserId;
            //creates the User Deck
            await Constants.SaveUserDeckAsync(userDeck);

            //need a method to create the DeckGroupDeck part to combine the DeckGroup with the Created Deck.
            DeckGroupDeck deckGroupDeck = new DeckGroupDeck();
            deckGroupDeck.DeckGroupId = deckgroup.DeckGroupId;
            deckGroupDeck.DeckId = Deck.DeckId;
            await Constants.SaveDeckGroupDeckAsync(deckGroupDeck);
            if (LoggedInUser.UserId == deckgroup.UserId)
            {
                CurrentDeckGroupDeck = await Constants.GetSpecificDeckGroupDeck(deckGroupDeck.DeckGroupId, deckGroupDeck.DeckId);
            }
        }

            

        ////may need to re-find the first DeckGroupDeck to assign it below

        ////need to refind that DeckGroupDeck

        //pass in Deck so then Users can eventually add Flashcards to the deck.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck",  CurrentDeckGroupDeck}
                };
        //Finishing up making a DeckGroup so now it will take the user to Build Deck
        await Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);

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

    public User LoggedInUser { get; set; }
    
    public DeckGroup SelectedDeckGroup { get; set; }

    public Deck Deck { get; set; }

    public UserDeck UserDeck { get; set; }

    public DeckGroupDeck CurrentDeckGroupDeck { get; set; }

    //need this for the deckgroups by same id.
    public DeckGroupDeck DirectCopiedDeckGroupDeck { get; set; }

    
    public List<UserDeckGroup> UserDeckGroups { get; set; }

    public List<UserDeckGroup> SameDeckGroupsById { get; set; } = new List<UserDeckGroup>();

    public List<UserDeckGroup> NotSameDeckGroupsById { get; set; } = new List<UserDeckGroup>();

    public List<Deck> Decks { get; set; }


}