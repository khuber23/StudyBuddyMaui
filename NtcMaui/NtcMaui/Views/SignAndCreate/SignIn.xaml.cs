using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiStudyBuddy.Models;
using Microsoft.Maui.ApplicationModel;
using System.Windows;
using Microsoft.AspNetCore.Identity;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;

namespace NtcMaui.Views.SignAndCreate;

public partial class SignIn : ContentPage
{
     PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
    public SignIn()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        GoBack = false;
    }
    //code that just is there to check if a user clicked the back button.
    protected override bool OnBackButtonPressed()
    {
            return false;
    }
    public void CompleteSignIn(object sender, EventArgs e)
    {
        //add method before this with other tasks that check stuff like if user exists in database and the password they typed is re-typed again correctly.
        this.ValidateUser();
    }

    public async void GoToAccountCreation(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateAccount));
    }

    

    public async void ValidateUser()
    {
        //This endpoint I need to at least include the stuff from user to pass into other areas maybe?
        //should honestly just make this with a new endpoint to return a user based on username and password hash and if not null return.
        List<User> users = await Constants.GetAllUsers();
        foreach (User user in users)
        {
            if (user.Username == UserNameEntry.Text)
            {
                PasswordVerificationResult passwordVerificationResult =
                passwordHasher.VerifyHashedPassword(null, user.PasswordHash, PasswordEntry.Text);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    Error.Text = string.Empty;
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "Current User", user }
                };
                    await Shell.Current.GoToAsync(nameof(Success), navigationParameter);
                    break;
                }
                else
                {
                    Error.TextColor = Colors.Red;
                    Error.Text = "Invalid password";
                }

                break;
            }
            else
            {
                Error.TextColor = Colors.Red;
                Error.Text = "Invalid User Name";
            }
        }
    }

    private async void ForgotPassword_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ForgotPassword));
    }

    //bool set up on load to essentially stop user from going bck if they would delete their profile.
    public bool GoBack { get; set; }
}