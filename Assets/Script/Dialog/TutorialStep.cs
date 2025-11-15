// TutorialStep.cs
public enum TutorialStep
{
    None = 0,
    IntroStory,           // 1. First-time story

    // Branch point: wait for first interaction
    ChooseInteraction,    // 2. "Click on the laptop or door to start"

    // Laptop path
    UseLaptop,            // 3a. "Now open the email app!"
    CheckEmail,           // 4a. Done

    // Door path
    UseDoor,              // 3b. "Go outside and explore!"
    GoOutside,            // 4b. Done

    Completed
}