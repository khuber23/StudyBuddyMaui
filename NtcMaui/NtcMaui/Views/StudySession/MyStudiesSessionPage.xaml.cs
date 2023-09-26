using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySession;

public partial class MyStudiesSessionPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public MyStudiesSessionPage()
	{
		InitializeComponent();
	}

    //when i eventually start using this page i can use the item selected thingy of the list view to get the item that was selected
    //in this case being a deckgroup/deck. Then i can use it to eventually find all the flashcards in the thing and make a list of flashcards.
    //to get them to display we might need a mehtod/way of tracking the index of the cards in a list. with [0] and maybe like a counter.
    //then when it's done or when the counter == the last index of the cards add a button to finish.
    //also need a way of using the swipe methods for left and right

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        MyStudiesListView.ItemsSource = await GetAllDecks();
    }

    public async Task<List<UserDeck>> GetAllDecks()
    {
        List<UserDeck> decks = new List<UserDeck>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeck/user/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                decks = JsonSerializer.Deserialize<List<UserDeck>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return decks;
    }

    public User LoggedInUser { get; set; }
}