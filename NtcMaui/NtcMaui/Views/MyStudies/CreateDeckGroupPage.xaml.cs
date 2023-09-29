using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private DeckGroup _deckGroup = new DeckGroup();
    public CreateDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private async void GoToDeckGroupPage(object sender, EventArgs e)
    {
        //in this code then might need to create a deckgroup or make a method to create a deck group and post it before moving on?
        //I'll get the method ready
        //_deckGroup.DeckGroupName = DeckGroupNameEntry.Text;
        //_deckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
        //DeckGroup = _deckGroup;
        // //alright so basically we need to save it to a deckgroup then basically assign it to the User Deck after.
        //await SaveDeckGroupAsync(DeckGroup);

        // //get a list of the deckgroups and find the one matching the Description of current Deckgroup.
        // //this allows us to reassign the DeckGroup
        // List<DeckGroup> deckGroups = await GetAllDeckGroups();
        // foreach (DeckGroup deckGroup in deckGroups)
        // {
        //     if (deckGroup.DeckGroupName == DeckGroup.DeckGroupName && deckGroup.DeckGroupDescription == DeckGroup.DeckGroupDescription)
        //     {
        //         DeckGroup = deckGroup;
        //         break;
        //     }
        // }
        //balls to the wall
        //manual test to see why stuff isn't working
        //create userDeckGroup
        _deckGroup.DeckGroupId = 4;
        _deckGroup.DeckGroupName = "Pirateology";
        _deckGroup.DeckGroupDescription = "Deck group relating to the study of pirates";
        DeckGroup = _deckGroup;
        UserDeckGroup userDeckGroup = new UserDeckGroup();
        userDeckGroup.UserId = LoggedInUser.UserId;
        userDeckGroup.DeckGroupId = DeckGroup.DeckGroupId;

        await SaveUserDeckGroupAsync(userDeckGroup);

        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
       await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    public async Task SaveDeckGroupAsync(DeckGroup deckGroup)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        //wait to see what they want from it and explain what you are thinking/what he envisions. you could have been right in the beginning 
        //with the idea of creating new ones from there and saving them or just choosing 1 like your new idea. --past Brody
        Uri uri = new Uri(string.Format($"{Constants.LocalApiUrl}/api/DeckGroup", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroup>(deckGroup, Constants._serializerOptions);
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

    //remember the 1 to 1 relationship later on. Once a user makes a userDeck he owns that deck and nobody else can use it for the moment.
    public async Task SaveUserDeckGroupAsync(UserDeckGroup userDeckGroup)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        //wait to see what they want from it and explain what you are thinking/what he envisions. you could have been right in the beginning 
        //with the idea of creating new ones from there and saving them or just choosing 1 like your new idea. --past Brody
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<UserDeckGroup>(userDeckGroup, Constants._serializerOptions);
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

    //is going to get
    public async Task<List<DeckGroup>> GetAllDeckGroups()
    {
        List<DeckGroup> deckGroups = new List<DeckGroup>();


        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                deckGroups = JsonSerializer.Deserialize<List<DeckGroup>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }

        return deckGroups;
    }

    public User LoggedInUser { get; set; }

    public DeckGroup DeckGroup { get; set; }
}