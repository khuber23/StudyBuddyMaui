using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using CommunityToolkit.Maui.Views;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class FlashcardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public FlashcardPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        FlashcardListView.ItemsSource = await Constants.GetAllFlashCards();
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



    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
       List<FlashCard> flashCards = await Constants.GetAllFlashCards();

        if (flashCards != null)
        {
            if (e != null)
            {
                FlashcardListView.ItemsSource = null;
                FlashcardListView.ItemsSource = flashCards.Where(f => f.FlashCardQuestion.Contains(e.NewTextValue));
            }
            else
            {
                OnAppearing();
            }
        }
       
    }

    private void FlashcardListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if(e.SelectedItem != null)
        {
            SelectedFlashcard = e.SelectedItem as FlashCard;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Selected FlashCard", SelectedFlashcard }
                };
            Shell.Current.GoToAsync(nameof(AddFlashcardToDeckPage), navigationParameter);
        }
    }

    public User LoggedInUser { get; set; }

    public FlashCard SelectedFlashcard { get; set; }
}