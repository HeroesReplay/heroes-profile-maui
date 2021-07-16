

- [Visual Studio 2022 Preview MSIX Extension](https://marketplace.visualstudio.com/items?itemName=ProjectReunion.MicrosoftSingleProjectMSIXPackagingToolsDev17)

- [.NET SDK 6.0.0 Preview 6](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-6.0.100-preview.6-windows-x64-installer)

- Maui Check tool

    ```
    dotnet tool update -g redth.net.maui.check --version 0.6.1
    maui-check
    ```


- Code smells and thoughts that need reviewing

    - Views without ViewModels
    - ViewModels wihtout ReactiveUI
    - Mediator being used inside Handlers (This is handlers depending on handlers...not good?)
    - Handlers should be used for outward facing Commands and Queries only?
    - Replace any mediator logic inside handlers with the respective services/repositories?
    - Who calls the Publish if its not from a Handler?
    - Where do UI background tasks really go?