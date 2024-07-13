
- [.NET 8 SDK](https://dotnet.microsoft.com/download/)

- Notable Libraries used:

- Microsoft .NET Maui
- ReactiveUI with Blazor for Auto Updating Views based on ViewModel property changes
- WindowsAppSdk for Native WinUI3 Shell (However, UI is written in Maui Blazor since Html/Blazor development a choice over XAML development with Maui Controls)
- Polly for Resilience (handling HTTP 409 etc)
- Microsoft.Extensions for Configuration, Dependency Injection, Logging
- MediatR for Mediator Pattern with CQRS Core layer
- Heroes.StormReplayParser for Parsing Heroes of the Storm files.
- Blazorise for Blazor Components styled to Bootstrap
- FluentValidation for User input

- Code smells and thoughts that need reviewing

    - Views without ViewModels
    - ViewModels without ReactiveUI
    - Mediator being used inside Handlers (This is handlers depending on handlers...not good?)
    - Handlers should be used for outward facing Commands and Queries only?
    - Replace any mediator logic inside handlers with the respective services/repositories?
    - Who calls the Publish if its not from a Handler?
    - The right "entry point / location" for UI App initialization logic and background Tasks.