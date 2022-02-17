[![Nuget](https://img.shields.io/nuget/v/haf)](https://www.nuget.org/packages/HAF/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/kriho/haf/publish%20to%20nuget)](https://github.com/kriho/haf/actions)

# HAF X.X.X

Official documentation for the ~~**h**ornung~~ **h**olistic **a**pplication **f**ramework for WPF based MVVM applications.

This framework aims to make the most of the MVVM pattern by
- providing a service based approach to implement the application logic
- facilitationg systematic service dependencies and states
- using MEF (Managed Extensibility Framework) for service injection to allow complete application extensibility
- improving the design time experience by declaring design time data on a service level
- implementing most missing MVVM related classes such as the RelayCommand and the ObservableObject
 ## Concepts
 It is important to understand the [concepts](articles/service-infrastructure.md) of this framework before diving into the API documentation. These concepts have evolved over a duration of 15 years and some of them have become quite complex (but also very flexible). They are explained in a series of articles.

 ## Available application logic
 The framework provides a set of services to handle common application logic. This will streamline application development and has a huge impact on the implementation effort of the application as a whole. Provided functionality includes (among other things):
 - global application settings that can be filtered, are extendable by plugins, have a scope and default drawers
 - service configuration management that simplifies storage of data (and settings) on a service level
 - project management that creates scoped application settings and scoped service configuration
 - application startup and exit routines wich can be used to schedule actions
 - theme support with live switching, default themes and an in-app theme editor
 - window layout management for windows with multiple panes
 - window management for window related settings and granting access to controls
 - a flexible input validation system
 - filterable patch notes with categories
 - observable tasks that can be used to visualize asynchronous operations and integrate with async relay commands
 - observable task pools that facilitate scheduling of tasks according to configurable restrictions
 - extension management with in-app extension publishing
 - global localization with multiple source provides (among them PO files) with live switching
 - inter application communication using WCF
 - global hotkeys
 - several custom controls, especially for creating forms
 - a proper chrome window that supports UI elements in the title bar
 - dozens of converters for WPF bindings
 - dozens of attached behaviors that help with common problems
 - many useful utils