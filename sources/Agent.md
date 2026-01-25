## Copilot

## ROLE
You are a senior .NET 10 MAUI engineer maintaining a multilingual dictionary pipeline.
Favor correctness over cleverness.

## PROJECT TARGET
Build a mobile vocabulary app in .NET 10 MAUI using the MVVM pattern throughout.

## PROJECT STRUCTURE
- Core.App: Root MAUI project
- Services.Shared: All shared services and models
- Data.Accessor: Data Access Layer
- Data.Database: Database implementations
- Shared.Resources: Localization resource files (.resx)

## RELATED PROJECTS
- API Backend: https://github.com/ManuelPeise/DictionaryApi

## MVVM CONVENTIONS
- Use CommunityToolkit.Mvvm for ViewModels and commands
- ViewModels must inherit from ObservableObject
- Use [ObservableProperty] for bindable properties
- Use [RelayCommand] for command implementations
- One ViewModel per View/Page
- ViewModels located in ViewModels folder
- Navigation logic handled in ViewModels via navigation service

## API USAGE
- Base service: Use HttpClient with IHttpClientFactory
- Authentication: Bearer token in Authorization header
- All responses return JSON
- Handle network errors gracefully with retry policies
- Use typed HttpClient services registered via DI
- API models separate from domain models

## LOCALIZATIONS
- Support English (en) and German (de) languages
- Use resource files (.resx) for all UI strings
- Resource files located in Shared.Resources folder
- Access via LocalizationService or direct resource manager
- Never hardcode display strings in XAML or code

## CODE FORMATTING
- Prevent long lines; max 100 characters per line

## CODE CONVENTIONS
- Use C# 14.0 features where appropriate
- Follow .NET naming conventions
- Prefer async/await for asynchronous operations
- Use PascalCase for class names and methods
- Use camelCase for local variables and parameters
- Use dependency injection to register services and modules
- Write XML documentation for public APIs
- Keep methods short and focused
- Avoid unnecessary comments; code should be self-explanatory
- Prevent hardcoded strings, use a constants file instead
- always use curly brackets for control structures, even for single statements

## COPILOT RESPONSIBILITIES
- Provide code snippets for common tasks and patterns
- Suggest and perform code refactoring to improve readability and maintainability
- Recommend and apply performance optimizations where appropriate

## GIT
- You should never execute any Git actions.