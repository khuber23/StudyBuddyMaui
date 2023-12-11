using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using Microsoft.Maui.Controls.PlatformConfiguration;
using NtcMaui.Views.SignAndCreate;

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

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        Decks = await Constants.GetAllDecks();
        DecksToUpdate = Decks.Where(d => d.DeckName == SelectedDeck.DeckName && d.DeckDescription == SelectedDeck.DeckDescription).ToList();
        FlashCards = await Constants.GetAllFlashCards();
    }



    //So when a user saves a FlashCard it will end up in a list until a User decides to Finish making the Deck.
    //eventually handle making images but for now i can handle if flashcard is public.
    private void SaveFlashCard(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var foundFlashCard = FlashCards.FirstOrDefault(x => x.FlashCardQuestion == FlashcardQuestionEntry.Text);
        if (foundFlashCard != null)
        {
            if (foundFlashCard.FlashCardQuestion == FlashcardQuestionEntry.Text)
            {
                ErrorLabel.IsVisible = true;
                ErrorLabel.Text = "FlashCard already exists. Please use a different question.";
            }
        }

        else
        {
            FlashCard flashCard = new FlashCard();
            flashCard.FlashCardQuestion = FlashcardQuestionEntry.Text;
            flashCard.FlashCardAnswer = FlashcardAnswerEntry.Text;
            flashCard.FlashCardQuestionImage = FlashcardQuestionImageEntry.Text;
            flashCard.FlashCardAnswerImage = FlashcardAnswerImageEntry.Text;
            flashCard.IsPublic = IsPublic;

            //contreversial but Readonly will be set to false...I am not too worried about the editing for flashcards.
            //if a user would import a public flashcard it would clone a copy for that user to use and even eventually delete from it and use as they wish.

            flashCard.ReadOnly = false;
            UsermadeFlashCards.Add(flashCard);
            //added this for later use of dealing with getting all the flashcards by question that have been created (mostly re-get proper Id's)
            FlashCardQuestions.Add(flashCard.FlashCardQuestion);
            FlashcardQuestionEntry.Text = String.Empty;
            FlashcardAnswerEntry.Text = String.Empty;
            FlashcardAnswerImageEntry.Text = String.Empty;
            FlashcardQuestionImageEntry.Text = String.Empty;
            IsPublicCheckBox.IsChecked = false;
            if (UsermadeFlashCards.Count > 0)
            {
                FinishBtn.IsVisible = true;
                CancelBtn.IsVisible = false;
                FinishLabel.IsVisible = true;
            }
            else
            {
                FinishBtn.IsVisible = false;
                FinishLabel.IsVisible = false;
            }
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
        List<FlashCard> flashcards = await Constants.GetAllFlashCards();
        UsermadeFlashCards = flashcards.Where(item => FlashCardQuestions.Contains(item.FlashCardQuestion)).ToList();
        foreach (Deck deck in DecksToUpdate)
        {
            foreach (FlashCard userMadeFlashCard in UsermadeFlashCards)
            {
                DeckFlashCard deckFlashCard = new DeckFlashCard();
                deckFlashCard.FlashCardId = userMadeFlashCard.FlashCardId;
                deckFlashCard.DeckId = deck.DeckId;
                await SaveDeckFlashcardAsync(deckFlashCard);
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
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
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

    private async void UploadQuestionImageBtn_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images
        });

        FlashcardQuestionImageEntry.Text = result.FullPath;
    }

    private async void UploadAnswerImageBtn_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images
        });

        FlashcardAnswerImageEntry.Text = result.FullPath;
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
	private void GoToDeckPageTab(object sender, EventArgs e)
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

	public List<FlashCard> UsermadeFlashCards { get; set; } = new List<FlashCard>();

    public User LoggedInUser { get; set; }

    public Deck SelectedDeck { get; set; }

    public bool IsPublic { get; set; }

    public List<Deck> Decks { get; set; }

    public List<Deck> DecksToUpdate { get; set; }

    public List<string> FlashCardQuestions { get; set;} = new List<string>();

    public List<FlashCard> FlashCards { get; set; }
}