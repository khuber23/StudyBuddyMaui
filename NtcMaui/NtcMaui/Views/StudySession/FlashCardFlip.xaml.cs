using System.Diagnostics.Metrics;

namespace NtcMaui.Views.StudySession;

public partial class FlashCardFlip : ContentPage
{
	public FlashCardFlip()
	{
		InitializeComponent();
    }

    
   
  async void OnTapRecognized(object sender, TappedEventArgs args)
    {
        if (FlashcardText.Text == "Here is some filler text")
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = "text has changed";
        }
        else if (FlashcardText.Text == "text has changed")
        {
            await Flashcard.RotateXTo(180, 500);
            Flashcard.RotationX = 0;
            FlashcardText.Text = "Here is some filler text";
        }
    }
}