using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateFlashcardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
   
	public CreateFlashcardPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;
        OnPropertyChanged("Current User");
    }

    //So when a user saves a FlashCard it will end up in a list until a User decides to Finish making the Deck.
    //eventually handle making images but for now i can handle if flashcard is public.
    private void SaveFlashCard(object sender, EventArgs e)
    {
        FlashCard flashCard = new FlashCard();
        flashCard.FlashCardQuestion = FlashcardQuestionEntry.Text;
        flashCard.FlashCardAnswer = FlashcardAnswerEntry.Text;
        flashCard.IsPublic = IsFlashCardPublic;
        UsermadeFlashCards.Add(flashCard);
        FlashcardQuestionEntry.Text = String.Empty;
        FlashcardAnswerEntry.Text = String.Empty;
        IsPublicCheckBox.IsChecked = false;
        if (UsermadeFlashCards.Count > 0)
        {
            FinishBtn.IsVisible = true;
            FinishLabel.IsVisible = true;
        }
        else
        {
            FinishBtn.IsVisible = false;
            FinishLabel.IsVisible = false;
        }
    }

    //this will finish the creation of flashcards.
    private async void FinishCreation(object sender, EventArgs e)
    {
        //first we take each flashcard inside the list and add it to the Database.
        foreach (FlashCard userMadeFlashCard in UsermadeFlashCards)
        {
            await SaveFlashcardAsync(userMadeFlashCard);
        }
        //after that we neeed to make retrieve the right flashcards with the Id in the database and then make them into DeckFlashcards.
        List<FlashCard> flashcards = await GetAllFlashCards();
        foreach(FlashCard flashCard in flashcards)
        {
            foreach (FlashCard userMadeFlashCard in UsermadeFlashCards)
            {
                if (flashCard.FlashCardQuestion == userMadeFlashCard.FlashCardQuestion && flashCard.FlashCardAnswer == userMadeFlashCard.FlashCardAnswer)
                {
                    DeckFlashCard deckFlashCard = new DeckFlashCard();
                    deckFlashCard.FlashCardId = flashCard.FlashCardId;
                    deckFlashCard.DeckId = SelectedDeck.DeckId;
                    await SaveDeckFlashcardAsync(deckFlashCard);
                    //we break out here so that it doesn't keep looping through all of the userMade Flashcards. Since we will need to keep looping
                    //through the flashcard in the database anyway.
                    break;
                }
            }
        }
        //then we can probably go back to the DecksPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    //navigates to DeckPage
    private void GoToDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    //Posts a flashcard
    public async Task SaveFlashcardAsync(FlashCard flashCard)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/FlashCard", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<FlashCard>(flashCard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //Posts a Deckflashcard
    public async Task SaveDeckFlashcardAsync(DeckFlashCard deckFlashCard)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckFlashCard", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckFlashCard>(deckFlashCard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\deckFlashCard successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    //gets all the flashCards
    public async Task<List<FlashCard>> GetAllFlashCards()
    {
        List<FlashCard> flashCards = new List<FlashCard>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/FlashCard", string.Empty));
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

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            IsFlashCardPublic = true;
        }
        else
        {
            IsFlashCardPublic = false;
        }
    }

    public List<FlashCard> UsermadeFlashCards { get; set; } = new List<FlashCard>();

    public User LoggedInUser { get; set; }

    public Deck SelectedDeck { get; set; }

    public bool IsFlashCardPublic { get; set; }
}