using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Admin;

public partial class AdminEditFlashCardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public AdminEditFlashCardPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedFlashCard = query["Current FlashCard"] as FlashCard;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        FlashCardQuestionEntry.Text = SelectedFlashCard.FlashCardQuestion;
        FlashCardAnswerEntry.Text = SelectedFlashCard.FlashCardAnswer;
        FlashcardQuestionImageEntry.Text = SelectedFlashCard.FlashCardQuestionImage;
        FlashcardQuestionImageEntry.Text = SelectedFlashCard.FlashCardAnswerImage;
        DeckFlashcards = await Constants.GetAllDeckFlashCards();
        DeckFlashcards = DeckFlashcards.Where(deckflashcard => deckflashcard.FlashCardId == SelectedFlashCard.FlashCardId || deckflashcard.FlashCard.FlashCardQuestion == SelectedFlashCard.FlashCardQuestion).ToList();
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
        WarningLabel.Text = $"Warning: You are about to delete {SelectedFlashCard.FlashCardQuestion}. Hitting finish will delete yours and your shared users' flashcards from the deck.";
        WarningLabel.IsVisible = true;
    }


    private async void FinishDeleteBtn_Clicked(object sender, EventArgs e)
    {
        foreach (DeckFlashCard deckFlashCard in DeckFlashcards)
        {
            await DeleteDeckFlashCardAsync(deckFlashCard.DeckId, deckFlashCard.FlashCardId);
        }
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        //don't know where to take them so take them to the deck page lol
        await Shell.Current.GoToAsync(nameof(AdminFlashCardPage), navigationParameter);

    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        //change to deal with flashcard
        SelectedFlashCard.FlashCardQuestion = FlashCardQuestionEntry.Text;
        SelectedFlashCard.FlashCardAnswer = FlashCardAnswerEntry.Text;
        SelectedFlashCard.FlashCardAnswerImage = FlashcardAnswerImageEntry.Text;
        SelectedFlashCard.FlashCardQuestionImage = FlashcardQuestionImageEntry.Text;
        //eventually deal with images here as well.

        await PutFlashCardAsync(SelectedFlashCard);
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        //goes right back to BuildDeckPage incase users wanted to edit more flashcards, which is why we needed to pass in Current deck.
        await Shell.Current.GoToAsync(nameof(AdminFlashCardPage), navigationParameter);
    }

    /// <summary>
    /// Does a Put command on the flashcard
    /// </summary>
    /// <param name="flashcard">the flashcard you are updating</param>
    /// <returns>updated flashcard</returns>
    public async Task PutFlashCardAsync(FlashCard flashcard)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/FlashCard/%7Bid%7D?flashcardid={SelectedFlashCard.FlashCardId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<FlashCard>(flashcard, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public async Task DeleteDeckFlashCardAsync(int deckId, int flashCardId)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckFlashCard/{deckId}/{flashCardId}", string.Empty));

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

    public User LoggedInUser { get; set; }

    public FlashCard SelectedFlashCard { get; set; }

    public bool IsPublic { get; set; }

    public List<DeckFlashCard> DeckFlashcards { get; set; }
}