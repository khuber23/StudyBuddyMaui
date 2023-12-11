using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Edit;

public partial class EditDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditDeckPage()
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
            await Constants.DeleteUserDeckAsync(userDeck.UserId, userDeck.DeckId);
        }
        //essentially if there is a deck relating to DeckGroupDecks
        //so if a user is trying to delete a deck in a deckgroup we need to delete the userDeck as well as the deckgroupdeck
        if (DeckGroupDecks.Count > 0)
        {
            foreach (DeckGroupDeck groupDeck in DeckGroupDecks)
            {
                await Constants.DeleteDeckGroupDeckAsync(groupDeck.DeckGroupId, groupDeck.DeckId);
            }
        }

        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);

    }


    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        foreach(Deck deck in DecksToEdit)
        {
            SelectedDeck = deck;
            SelectedDeck.DeckName = DeckNameEntry.Text;
            SelectedDeck.DeckDescription = DeckDescriptionEntry.Text;
            SelectedDeck.IsPublic = IsPublic;
            //the first item should be the one similiar to the one needing to be edited...so anything after are the shared dekcs.
            if(DecksToEdit.First().DeckId == SelectedDeck.DeckId)
            {
                SelectedDeck.IsPublic = IsPublic;
            }
            else
            {
                SelectedDeck.IsPublic = false;
            }
            await Constants.PutDeckAsync(SelectedDeck);
        }
        
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
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

	public async void LogOut(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(nameof(SignIn));
	}


	public bool IsPublic { get; set; }

    public User LoggedInUser { get; set; }

    public Deck SelectedDeck { get; set; }

    //gets a list of all the decks that can be editable (used for shared decks mostly to updated stuff for shared)
    public List<Deck> DecksToEdit { get; set; }

    public List<DeckGroupDeck> DeckGroupDecks { get; set; }

    public List<UserDeck> UserDecks { get; set; }


}