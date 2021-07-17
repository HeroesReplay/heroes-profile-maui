

- [Visual Studio 2022 Preview MSIX Extension](https://marketplace.visualstudio.com/items?itemName=ProjectReunion.MicrosoftSingleProjectMSIXPackagingToolsDev17)

- [.NET SDK 6.0.0 Preview 6](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-6.0.100-preview.6-windows-x64-installer)

- Maui Check tool

    ```
    dotnet tool update -g redth.net.maui.check --version 0.6.1
    maui-check
    ```


- Notable Libraries used:

- Microsoft .NET Maui
- CommunityToolkit for Native Windows integration APIs
- ReactiveUI with Blazor for Auto Updating Views based on ViewModel property changes
- WindowsAppSdk for Native WinUI3 Shell (However, UI is written in Maui Blazor since Html/Blazor development a choice over XAML development with Maui Controls)
- Polly for Resilience (handling HTTP 409 etc)
- Microsoft.Extensions for Configuration, Dependency Injection, Logging
- MediatR for Mediator Pattern with CQRS Core layer
- Heroes.ReplayParser for Parsing Heroes of the Storm files.
- TwitchLib for Auto Predictions
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