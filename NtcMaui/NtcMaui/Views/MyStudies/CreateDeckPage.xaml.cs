using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private Deck _deck = new Deck();
	public CreateDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private void GoToBuildDeckPage(object sender, EventArgs e)
    {
        _deck.DeckName = DeckNameEntry.Text;
        _deck.DeckDescription = DeckDescriptionEntry.Text;
        Deck = _deck;
        SaveDeckAsync(Deck);
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);
    }

    public async Task SaveDeckAsync(Deck deck)
    {
        //either will be api/userDeck or maybe just Deck?
        //for now i won't run anything but will just keep deck. (won't do a post essentially just comment it out)
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            //response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public User LoggedInUser { get; set; }

    public Deck Deck { get; set; }
}