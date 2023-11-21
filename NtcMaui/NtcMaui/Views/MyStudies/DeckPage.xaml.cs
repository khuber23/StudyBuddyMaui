using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.Edit;
using NtcMaui.Views.Share;
using NtcMaui.Views.SignAndCreate;

namespace NtcMaui.Views.MyStudies;

public partial class DeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public DeckPage()
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

        DeckListView.ItemsSource = await Constants.GetAllDeckByUserId(LoggedInUser.UserId);
    }

    //this will be slightly different than the other go to Create Deck page.
    //Reason being the other CreateDeckPage and stuff deals with DeckGroup and I can't deal with nulls when dealing with query attributes.
    //So we will just be making a different CreateDeckPage and BuildDeckPage
    private void GoToCreateDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(CreateDeckPageNoDeckGroup), navigationParameter);
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
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DashboardPage), navigationParameter);
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
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

    private void GoToShareDeckPage(object sender, EventArgs e)
    {
        //eventually make this the dashboard page and also send the user through to this page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(ShareDeckPage), navigationParameter);
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

    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedDeck = e.SelectedItem as UserDeck;

            //if checked go to edit Deck Page
            if (EditingCheckBox.IsChecked == true)
            {
                if (SelectedDeck.Deck.ReadOnly == true)
                {
                    ErrorLabel.Text = $"{SelectedDeck.Deck.DeckName} isn't editable";
                    ErrorLabel.IsVisible = true;
                    //it's weird but this is the work around for dealing with re-selecting options for the item.
                    SelectedDeck = null;
                    DeckListView.SelectedItem = null;
                }
                else
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current Deck", SelectedDeck.Deck}
                };
                    Shell.Current.GoToAsync(nameof(EditDeckPage), navigationParameter);
                }
                
            }
            else
            //go to view/build screen
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //added this to go to the BuildDeckGroupPage, might eventually just make an edit/viewing page?  
                    {"Current Deck", SelectedDeck.Deck}
                };
                Shell.Current.GoToAsync(nameof(BuildDeckPageOnlyDeck), navigationParameter);
            }

        }
    }

    public User LoggedInUser { get; set; }

    public UserDeck SelectedDeck { get; set; }

}