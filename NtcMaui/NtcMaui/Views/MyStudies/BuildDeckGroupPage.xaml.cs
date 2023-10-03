using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class BuildDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public BuildDeckGroupPage()
	{
		InitializeComponent();
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        DeckListView.ItemsSource = await GetAllDecks();
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

    private void GoToCreateDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(CreateDeckPage), navigationParameter);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedUserDeckGroup = query["Selected UserDeckGroup"] as UserDeckGroup;
        OnPropertyChanged("Current User");
    }



    public User LoggedInUser { get; set; }

    public UserDeckGroup SelectedUserDeckGroup { get; set; }

    public UserDeck SelectedDeck { get; set; }

    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            SelectedDeck = e.SelectedItem as UserDeck;
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"Selected UserDeckGroup", SelectedUserDeckGroup },
                    {"Selected UserDeck", SelectedDeck }
                };
            Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);

        }
    }
}