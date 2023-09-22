using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class DeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    public DeckGroupPage()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        DeckGroupListView.ItemsSource = await GetAllDeckGroups();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    private void GoToDashboardPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DashboardPage));
    }

    private void GoToFlashcardPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(FlashcardPage));
    }
    private void GoToDeckPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeckPage));
    }

    private void GoToDeckGroupPage(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeckGroupPage));
    }


    public async Task<List<UserDeckGroup>> GetAllDeckGroups()
    {
        List<UserDeckGroup> deckGroups = new List<UserDeckGroup>();


        //Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup", string.Empty));
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/user/{LoggedInUser.UserId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                deckGroups = JsonSerializer.Deserialize<List<UserDeckGroup>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return deckGroups;
    }

    public User LoggedInUser { get; set; }
}