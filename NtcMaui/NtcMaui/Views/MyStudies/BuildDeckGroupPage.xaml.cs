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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }



    //button click event for Building off of DeckGroup
    private void GoToCreateDeckPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //needs to be DeckGroup for later for connecting Everything.
            {"Current DeckGroup", SelectedDeckGroup }
                };
        Shell.Current.GoToAsync(nameof(CreateDeckPage), navigationParameter);
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

    public DeckGroup SelectedDeckGroup { get; set; }

    public UserDeck SelectedDeck { get; set; }

    private void DeckListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        //eventually change this to be for editing or just button clicks later, for now not worrying about it
        //if (e.SelectedItem != null)
        //{
        //    SelectedDeck = e.SelectedItem as UserDeck;
        //    var navigationParameter = new Dictionary<string, object>
        //        {
        //            { "Current User", LoggedInUser },
        //            {"Selected UserDeckGroup", SelectedUserDeckGroup },
        //            {"Selected UserDeck", SelectedDeck }
        //        };
        //    Shell.Current.GoToAsync(nameof(BuildDeckPage), navigationParameter);

        //}
    }
}