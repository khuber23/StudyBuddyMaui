using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ApiStudyBuddy.Models;
using NtcMaui.Views.MyStudies;

namespace NtcMaui.Views.Edit;

public partial class EditDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public EditDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        SelectedDeckGroup = query["Current DeckGroup"] as DeckGroup;
        OnPropertyChanged("Current User");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        DeckGroupNameEntry.Text = SelectedDeckGroup.DeckGroupName;
        DeckGroupDescriptionEntry.Text = SelectedDeckGroup.DeckGroupDescription;
    }

    private async void FinishEditingBtn_Clicked(object sender, EventArgs e)
    {
        SelectedDeckGroup.DeckGroupName = DeckGroupNameEntry.Text;
        SelectedDeckGroup.DeckGroupDescription = DeckGroupDescriptionEntry.Text;
        await PutDeckGroupAsync(SelectedDeckGroup);
        //then navigate back to DeckGroupPage
        var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser }
                };
       await Shell.Current.GoToAsync(nameof(DeckGroupPage), navigationParameter);
    }

    /// <summary>
    /// Does a Put command on the deckgroup
    /// </summary>
    /// <param name="deckGroup">the deckgroup you are updating</param>
    /// <returns>updated deckgroup</returns>
    public async Task PutDeckGroupAsync(DeckGroup deckGroup)
    {
        //note to self. You need to have the %7Bid%7D?deckgroupid={} since that is what the endpoint is looking for
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup/%7Bid%7D?deckgroupid={SelectedDeckGroup.DeckGroupId}", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<DeckGroup>(deckGroup, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PutAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tTodoItem successfully updated.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }


    public User LoggedInUser { get; set; }

    public DeckGroup SelectedDeckGroup { get; set; }
}