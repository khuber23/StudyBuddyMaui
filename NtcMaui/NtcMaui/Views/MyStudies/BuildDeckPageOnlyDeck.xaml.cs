using System.ComponentModel;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class BuildDeckPageOnlyDeck : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public BuildDeckPageOnlyDeck()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as UserDeck;

        OnPropertyChanged("Current User");
    }

    //button Click For CreateFlashcardPage
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    { "Current Deck", SelectedDeck.Deck}
                };
        Shell.Current.GoToAsync(nameof(CreateFlashcardPage), navigationParameter);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //this I want to eventually be all the Deck Flashcards. Work on that later.
        //FlashcardListView.ItemsSource = await GetAllFlashcards();
        FlashcardListView.ItemsSource = SelectedDeck.Deck.DeckFlashCards;
        BuildDeckNameLabel.Text = $"Building: {SelectedDeck.Deck.DeckName}";
    }

    public User LoggedInUser { get; set; }

    public UserDeck SelectedDeck { get; set; }
}