using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class CreateFlashcardPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public CreateFlashcardPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    public async Task SaveFlashcardAsync(FlashCard flashCard)
    {
        //either will be api/userDeckgroup or maybe just Deckgroup?
        //for now i won't run anything but will just keep deckgroup.
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Flashcard", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<FlashCard>(flashCard, Constants._serializerOptions);
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
}