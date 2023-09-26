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

    //after building a deckgroup they need to go back and possibly choose 1 to use?
    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        //in this code then might need to create a deckgroup or make a method to create a deck group and post it before moving on?
        //I'll get the method ready
        _deckGroup.DeckGroupName = DeckGroupNameEntry.Text;
        _deckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
        DeckGroup = _deckGroup;
        SaveDeckGroupAsync(DeckGroup);
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
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

    public DeckGroup DeckGroup { get; set; }
}