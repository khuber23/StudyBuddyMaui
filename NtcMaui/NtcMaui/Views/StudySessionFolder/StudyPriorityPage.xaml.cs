using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ApiStudyBuddy.Models;
using Microsoft.Maui.Controls;

namespace NtcMaui.Views.StudySessionFolder;

public partial class StudyPriorityPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
	public StudyPriorityPage()
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
        IncorrectCards = await GetIncorrectStudySessionsById(LoggedInUser.UserId);
        StudySessionFlashCardsListView.ItemsSource = IncorrectCards;
        //ok so it appears but now i need a way to seperate them so that only return the flashcard question where 
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

    public async Task<List<StudySession>> GetFullStudySessionsById(int userId)
    {
        List<StudySession> studySessions = new List<StudySession>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySession/full/{userId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                studySessions = JsonSerializer.Deserialize<List<StudySession>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return studySessions;
    }

    public async Task<List<StudySessionFlashCard>> GetIncorrectStudySessionsById(int userId)
    {
        List<StudySessionFlashCard> studySessionFlashCards = new List<StudySessionFlashCard>();

        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/StudySessionFlashCard/incorrect/{userId}", string.Empty));
        try
        {
            HttpResponseMessage response = await Constants._client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                studySessionFlashCards = JsonSerializer.Deserialize<List<StudySessionFlashCard>>(content, Constants._serializerOptions);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
        return studySessionFlashCards;
    }

    public User LoggedInUser { get; set; }

    public List<StudySession> StudySessions { get; set; }

    public List<StudySessionFlashCard> IncorrectCards { get; set; }
}