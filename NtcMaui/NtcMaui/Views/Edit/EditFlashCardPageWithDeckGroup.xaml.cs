using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Edit;

public partial class EditFlashCardPageWithDeckGroup : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    //leave a note tonight about also adding an Iseditable or something to flashcards incase a user adds in a flashcard from public
	public EditFlashCardPageWithDeckGroup()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedFlashCard = query["Current FlashCard"] as FlashCard;
        SelectedDeckGroupDeck = query["Current Deck"] as DeckGroupDeck;
        OnPropertyChanged("Current User");
    }

    
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        FlashCardQuestionEntry.Text = SelectedFlashCard.FlashCardQuestion;
        FlashCardAnswerEntry.Text = SelectedFlashCard.FlashCardAnswer;
        IsPublicCheckBox.IsChecked = SelectedFlashCard.IsPublic;
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

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        //change to deal with flashcard
        SelectedFlashCard.FlashCardQuestion = FlashCardQuestionEntry.Text;
        SelectedFlashCard.FlashCardAnswer = FlashCardAnswerEntry.Text;
        //eventually deal with images here as well.

        await PutFlashCardAsync(SelectedFlashCard);
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Current Deck", SelectedDeckGroupDeck }
                };
        //goes right back to BuildDeckPage incase users wanted to edit more flashcards, which is why we needed to pass in Current deck.
        await Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);
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

    public DeckGroupDeck SelectedDeckGroupDeck { get; set; }
    public User LoggedInUser { get; set; }

    public FlashCard SelectedFlashCard { get; set; }

    public bool IsPublic { get; set; }
}