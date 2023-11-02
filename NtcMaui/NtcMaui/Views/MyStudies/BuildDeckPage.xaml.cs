using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;

namespace NtcMaui.Views.MyStudies;

//eventually need to make a view flashcard page to see the details and eventually edit like images and stuff.
public partial class BuildDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public BuildDeckPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroupDeck = query["Current Deck"] as DeckGroupDeck;

        OnPropertyChanged("Current User");
    }

    //button Click For CreateFlashcardPage
    private void GoToCreateFlashcardPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    { "Current Deck", SelectedDeckGroupDeck.Deck},
                };
        Shell.Current.GoToAsync(nameof(CreateFlashcardPage), navigationParameter);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //this I want to eventually be all the Deck Flashcards. Work on that later.
        //FlashcardListView.ItemsSource = await GetAllFlashcards();
        FlashcardListView.ItemsSource = SelectedDeckGroupDeck.Deck.DeckFlashCards;
        BuildDeckNameLabel.Text = $"Building: {SelectedDeckGroupDeck.Deck.DeckName}";
    }

    //might straight up get rid of this and just display all of the DeckFlashCards of the deck/deckGroupDeck
    public async Task<List<FlashCard>> GetAllFlashcards()
    {
        List<FlashCard> flashCards = new List<FlashCard>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Flashcard", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                flashCards = JsonSerializer.Deserialize<List<FlashCard>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return flashCards;
    }

    private void FlashcardListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        { 
           if (EditingCheckBox.IsChecked == true)
            {
                SelectedDeckFlashCard = e.SelectedItem as DeckFlashCard;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current FlashCard", SelectedDeckFlashCard.FlashCard},
                    //need this to be able to go back to this page after editing flashcard
                    {"Current Deck", SelectedDeckGroupDeck }
                };
                Shell.Current.GoToAsync(nameof(EditFlashCardPageWithDeckGroup), navigationParameter);
            }
            else
            {
                FlashcardListView.SelectedItem = null;
            }
        }
    }

    public User LoggedInUser { get; set; }

    public DeckGroupDeck SelectedDeckGroupDeck { get; set; }

    public DeckFlashCard SelectedDeckFlashCard { get; set; }


}