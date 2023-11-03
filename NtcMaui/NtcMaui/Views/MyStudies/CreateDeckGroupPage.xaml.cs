using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using System.Collections.ObjectModel;

namespace NtcMaui.Views.MyStudies;

public partial class CreateDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
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
        DeckGroup = new DeckGroup();
        DeckGroup.DeckGroupName = DeckGroupNameEntry.Text;
        DeckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
        //alright so basically we need to save it to a deckgroup then basically assign it to the User DeckGroup after.
        await SaveDeckGroupAsync(DeckGroup);

        //get a list of the deckgroups and find the one matching the Description of current Deckgroup.
        //this allows us to reassign the DeckGroup
        List<DeckGroup> deckGroups = await GetAllDeckGroups();
        foreach (DeckGroup deckGroup in deckGroups)
        {
            if (deckGroup.DeckGroupName == DeckGroup.DeckGroupName && deckGroup.DeckGroupDescription == DeckGroup.DeckGroupDescription)
            {
                DeckGroup = deckGroup;
                break;
            }
        }
        //create userDeckGroup
        UserDeckGroup userDeckGroup = new UserDeckGroup();
        userDeckGroup.UserId = LoggedInUser.UserId;
        userDeckGroup.DeckGroupId = DeckGroup.DeckGroupId;

        //so now UserDeckGroup is at least made and if there was an issue a user can at least later go into it to view it/edit
        await SaveUserDeckGroupAsync(userDeckGroup);

        //So this method is basically going towards the Full Creation of a UserDeckGroup. So after they create one, immedietly takes them to BuildDeckGroup.
        //might eventually need a different page/folder for editing.

        //pass on the DeckGroup that they made for their UserDeckGroup to the next page.
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    //this is the current DeckGroup that belongs to the user, easier to deal with this than the userDeckGroup itself.
                    {"Current DeckGroup", DeckGroup }
                };
       await Shell.Current.GoToAsync(nameof(BuildDeckGroupPage), navigationParameter);
    }

    public async Task SaveDeckGroupAsync(DeckGroup deckGroup)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        //wait to see what they want from it and explain what you are thinking/what he envisions. you could have been right in the beginning 
        //with the idea of creating new ones from there and saving them or just choosing 1 like your new idea. --past Brody
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup", string.Empty));

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

    //is going to get all of the Deck Groups
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

    private void IsPublicCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            IsPublic = true;
            ReadOnlyStack.IsVisible = true;
        }
        else
        {
            IsPublic = false;
            ReadOnlyStack.IsVisible = false;
            //re-set this to false if it was checked after unsetting is public
            ReadOnlyCheckBox.IsChecked = false;
        }
    }

    private void ReadOnlyCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value == true)
        {
            ReadOnly = true;
        }
        else
        {
            ReadOnly = false;
        }
    }

    public bool ReadOnly { get; set; }

    public bool IsPublic { get; set; }

    public User LoggedInUser { get; set; }

    public DeckGroup DeckGroup { get; set; }


}