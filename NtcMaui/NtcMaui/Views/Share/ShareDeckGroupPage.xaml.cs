using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;

namespace NtcMaui.Views.Share;

public partial class ShareDeckGroupPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public ShareDeckGroupPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        LoggedInUser = query["Current User"] as User;
        OnPropertyChanged("Current User");
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        DeckGroupPicker.ItemsSource = await GetAllUserDeckGroups();
    }

    //picks the deckgroups based on the user DeckGroups
    private void DeckGroupPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            //use this deckname to eventually find a deck in the next btn.
            ErrorLabel.IsVisible = false;
            DeckGroupName = picker.Items[selectedIndex];
        }
    }

    //will take them to the next page dealing with Copying a deck to a user
    private async void CopyBtn_Clicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        DeckGroup selectedDeckGroup = await GetDeckGroupByDeckGroupName(DeckGroupName);
        if (selectedDeckGroup == null || selectedDeckGroup.DeckGroupId == 0)
        {
            ErrorLabel.Text = "Please Select a deck group from the drop-down";
            ErrorLabel.IsVisible = true;
        }
        else
        {
            SelectedDeckGroup = selectedDeckGroup;
            string chosenShareType = "Copy";
            //then go to the ShareDeckWithUserPage
            //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //have so many seperate pages for the shareDeckWithUserPage
            var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", LoggedInUser },
                {"Current DeckGroup" , SelectedDeckGroup},
                {"Share Type", chosenShareType }
                };
            //make this page next
            //await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
        }
    }

        //will take them to the next page dealing with cloning a deck to a user
        private async void CloneBtn_Clicked(object sender, EventArgs e)
        {
            //ErrorLabel.IsVisible = false;
            //Deck selectedDeck = await GetDeckByDeckName(DeckName);
            //if (selectedDeck == null || selectedDeck.DeckId == 0)
            //{
            //    ErrorLabel.Text = "Please Select a deck from the drop-down";
            //    ErrorLabel.IsVisible = true;
            //}
            //else
            //{
            //    SelectedDeck = selectedDeck;
            //    string chosenShareType = "Clone";
            //    //then go to the ShareDeckWithUserPage
            //    //just to also make it easier I might make a string to easily identify which option they chose for sharing, just to not
            //    //have so many seperate pages for the shareDeckWithUserPage
            //    var navigationParameter = new Dictionary<string, object>
            //        {
            //            { "Current User", LoggedInUser },
            //        {"Current Deck" , SelectedDeck},
            //        {"Share Type", chosenShareType }
            //        };
            //    await Shell.Current.GoToAsync(nameof(ShareDeckWithUserPage), navigationParameter);
            //}
        }

        public async Task<List<UserDeckGroup>> GetAllUserDeckGroups()
        {
            List<UserDeckGroup> deckGroups = new List<UserDeckGroup>();

            //originally
            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/UserDeckGroup/maui/deckgroup/{LoggedInUser.UserId}", string.Empty));
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

        //might be an issue later but...i think it's fine
        /// <summary>
        /// gets the deck based on deck name
        /// </summary>
        /// <returns>a deck</returns>
        public async Task<DeckGroup> GetDeckGroupByDeckGroupName(string DeckGroupName)
        {
            DeckGroup deckGroup = new DeckGroup();

            Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/DeckGroup/deckgroup/{DeckGroupName}", string.Empty));
            try
            {
                HttpResponseMessage response = await Constants._client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    deckGroup = JsonSerializer.Deserialize<DeckGroup>(content, Constants._serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return deckGroup;
        } 

    public DeckGroup SelectedDeckGroup { get; set; }

    public string DeckGroupName { get; set; }

    public User LoggedInUser { get; set; }

}