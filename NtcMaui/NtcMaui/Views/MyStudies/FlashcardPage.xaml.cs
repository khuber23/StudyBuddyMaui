namespace NtcMaui.Views.MyStudies;

public partial class FlashcardPage : ContentPage
{
	public FlashcardPage()
	{
		InitializeComponent();
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
}