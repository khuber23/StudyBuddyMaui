using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.StudySessionFolder;

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
        MyStudiesListView.ItemsSource = await GetAllUserDeckGroups();
    }

    public async Task<List<UserDeckGroup>> GetAllUserDeckGroups()
    {
        List<UserDeckGroup> userDeckGroups = new List<UserDeckGroup>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/user/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                userDeckGroups = JsonSerializer.Deserialize<List<UserDeckGroup>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return userDeckGroups;
    }

    public User LoggedInUser { get; set; }

    public UserDeckGroup ChosenUserDeckGroup { get; set; }

    private void BeginSession(object sender, EventArgs e)
    {
        ChosenUserDeckGroup = MyStudiesListView.SelectedItem as UserDeckGroup;
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                    {"ChosenStudy", ChosenUserDeckGroup }
                };
        Shell.Current.GoToAsync(nameof(StudyingPage), navigationParameter);
    }

    private void GoToStudyPriorityPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        Shell.Current.GoToAsync(nameof(StudyPriorityPage), navigationParameter);
    }

    private void GoToMyStudiesSessionPage(object sender, EventArgs e)
    {
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                };
        Shell.Current.GoToAsync(nameof(MyStudiesSessionPage), navigationParameter);
    }
}