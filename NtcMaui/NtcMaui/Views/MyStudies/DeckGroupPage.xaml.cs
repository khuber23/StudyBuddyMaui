using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.MyStudies;

public partial class DeckGroupPage : ContentPage
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

    //private async void TestClick(object sender, EventArgs e)
    //{
    //    DeckGroupListView.ItemsSource = await GetAllDeckGroups();
    //}

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

    //public Task<List<DeckGroup>> DeckGroups { get; set; }
}