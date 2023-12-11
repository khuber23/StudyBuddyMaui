using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.Edit;

public partial class EditFlashCardOnlyDeck : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditFlashCardOnlyDeck()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedFlashCard = query["Current FlashCard"] as FlashCard;
        SelectedDeck = query["Current Deck"] as Deck;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        FlashCardQuestionEntry.Text = SelectedFlashCard.FlashCardQuestion;
        FlashCardAnswerEntry.Text = SelectedFlashCard.FlashCardAnswer;
        IsPublicCheckBox.IsChecked = SelectedFlashCard.IsPublic;
        FlashcardQuestionImageEntry.Text = SelectedFlashCard.FlashCardQuestionImage;
        FlashcardAnswerImageEntry.Text = SelectedFlashCard.FlashCardAnswerImage;
        DeckFlashcards = await Constants.GetAllDeckFlashCards();
        DeckFlashcards = DeckFlashcards.Where(deckflashcard => deckflashcard.DeckId == SelectedDeck.DeckId || deckflashcard.Deck.DeckName == SelectedDeck.DeckName).ToList();
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

    private async void FinishDeleteBtn_Clicked(object sender, EventArgs e)
    {
        foreach(DeckFlashCard deckFlashCard in DeckFlashcards)
        {
            await Constants.DeleteDeckFlashCardAsync(deckFlashCard.DeckId, deckFlashCard.FlashCardId);
        }
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        //don't know where to take them so take them to the deck page lol
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);

    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        //change to deal with flashcard
        SelectedFlashCard.FlashCardQuestion = FlashCardQuestionEntry.Text;
        SelectedFlashCard.FlashCardAnswer = FlashCardAnswerEntry.Text;
        SelectedFlashCard.FlashCardAnswerImage = FlashcardAnswerImageEntry.Text;
        SelectedFlashCard.FlashCardQuestionImage = FlashcardQuestionImageEntry.Text;
        //eventually deal with images here as well.

        await Constants.PutFlashCardAsync(SelectedFlashCard);
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", SelectedDeck }
                };
        //goes right back to BuildDeckPage incase users wanted to edit more flashcards, which is why we needed to pass in Current deck.
        await Shell.Current.GoToAsync(nameof(BuildDeckPageOnlyDeck), navigationParameter);
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


	public User LoggedInUser { get; set; }

    public FlashCard SelectedFlashCard { get; set; }

    public Deck SelectedDeck { get; set; }

    public bool IsPublic { get; set; }

    public List<DeckFlashCard> DeckFlashcards { get; set; }
}