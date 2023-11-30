using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using ApiStudyBuddy.Models;
using Microsoft.AspNetCore.Identity;

namespace NtcMaui.Views.SignAndCreate;

public partial class CreateAccount : ContentPage
{

    PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
    public CreateAccount()
    {
        InitializeComponent();
    }
    public async void CompleteCreation(object sender, EventArgs e)
    {
        //eventually add some checks/add to database before going back to sign in.
        User user = this.MakeUser();
        await SaveUserAsync(user);
        await Shell.Current.GoToAsync(nameof(SignIn));
    }

    public async Task SaveUserAsync(User user)
    {
        Uri uri = new Uri(string.Format($"{Constants.TestUrl}/api/User", string.Empty));

        try
        {
            string json = JsonSerializer.Serialize<User>(user, Constants._serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await Constants._client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tUser successfully saved.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }

    public User MakeUser()
    {
        User user = new User();
        user.Username = UserNameEntry.Text;
        //when we implement password hash
        user.PasswordHash = passwordHasher.HashPassword(null, PasswordEntry.Text);
        user.Email = EmailEntry.Text;
        user.FirstName = FirstNameEntry.Text;
        user.LastName = LastNameEntry.Text;
        //eventually add profile picture stuff here
        user.IsAdmin = false;
        user.ProfilePicture = UserImageEntry.Text;
        if (UserImageEntry.Text == string.Empty || UserImageEntry.Text == null)
        {
            if (ImagePath == string.Empty || ImagePath == null)
            {
                ImagePath = "stockprofileimage.png";
                UserImageEntry.Text = ImagePath;
            }
            else
            {
                user.ProfilePicture = UserImageEntry.Text;
            }
        }
        
        
        return user;
    }

    private async void UploadImageBtn_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images
        });

        ImagePath = result.FullPath;
        ProfileImage.Source = ImagePath;
        UserImageEntry.Text = ImagePath;
    }

    public string ImagePath { get; set; }
}