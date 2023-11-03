using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Edit;

public partial class EditDeckPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditDeckPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeck = query["Current Deck"] as Deck;
        OnPropertyChanged("Current User");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        DeckNameEntry.Text = SelectedDeck.DeckName;
        DeckDescriptionEntry.Text = SelectedDeck.DeckDescription;
    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        SelectedDeck.DeckName = DeckNameEntry.Text;
        SelectedDeck.DeckDescription = DeckDescriptionEntry.Text;
        await PutDeckAsync(SelectedDeck);
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
        await Shell.Current.GoToAsync(nameof(DeckPage), navigationParameter);
    }

    /// <summary>
    /// Does a Put command on the deck
    /// </summary>
    /// <param name="deck">the deck you are updating</param>
    /// <returns>updated deck</returns>
    public async Task PutDeckAsync(Deck deck)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/Deck/%7Bid%7D?deckid={SelectedDeck.DeckId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<Deck>(deck, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tdeck successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
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

    public Deck SelectedDeck { get; set; }


}