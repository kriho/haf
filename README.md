[![Nuget](https://img.shields.io/nuget/v/haf)](https://www.nuget.org/packages/HAF/)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/kriho/haf/publish%20to%20nuget)](https://github.com/kriho/haf/actions)

A **h**olistic **a**pplication **f**ramework for WPF based MVVM applications.

This framework aims to make the most of the MVVM pattern by
- providing a service based approach to implement the application logic
- allowing a systematic description of service dependencies and service states
- using MEF (Managed Extensibility Framework) for service injection
- providing full application extensibility
- improving the design time experience by declaring design time data on a service level
- already providing a large set of services for default application logic
- implementing most missing MVVM related classes such as the RelayCommand and the ObservableObject

### Naming conventions
Enitity | Prefix | Examples
-|-|-
event | On | OnConnected
command | Do | DoConnect
state | Is, Can | IsConnected, CanConnect
dependency | May | MayConnect

## Service infrastructure
Services act as the main interface to the functionality of an application. If the application logic is split into fine grained services, it is easy to maintain, extend and test. The abstract `Service` class should be used for all services and provides the service configuration behaviour.
All services can contribute to and retrive information from a global settings file using the `LoadConfiguration()`and `SaveConfiguration()` functions wich are used to access the `ServiceConfiguration` instance. This class allows easy manipulation of the XML based storage.
Service dependencies are declared using the `LinkedDependency` and `LinkedState` classes. These allow a unified expression of service relations and have some handy integrated features that makes them usable in bindings and commands. Service events are issued using the `LinkedEvent` classes. All "linked" classes provide a visualization of the logic flow in the console (in debug builds).

## Localization
The framework provides a localization suite that provides everything that is needed to localize real-world applications while supporting a live switching of cultures.
The `ILocalizationService` provides a [gettext](https://www.gnu.org/software/gettext/) interface for resolving a translated text. The service is used to register `ILocalizationProvider` instances that provide translations. The `SystemLocalizationProvider` simply returns the original ids and thus provides the english translations. Use the included `PoFileLocalizationProvider` to provide translations via po files.
```
this.Description = localizationService.GetText("Select folder that contains movies");
```
Using the `LocalizeExtension` binding, it is simple to place translated texts in the UI. 
```
<TextBlock Text="{haf:Localize 'Add from folder'}"/>
```
Dynamically created objects that contain translated texts and persist and must be updated when the selected culture changes. This can be achieved using the `LocalizedText`. It has multiple constructory that mimic the gettext interface. The object is implicitly converted to string and has a `Value` member that can be used in XAML bindings and will update when appropriate.
```
theme.Name = new LocalizedText("Dark");
```
The haf.tools CLI has a verb "localize" that scans a project for used strings and generates a pot file.

Notable mentions: [adams85](https://github.com/adams85) for a great way to extract texts from the source files, [Christina Mosers](https://www.wpftutorial.net/LocalizeMarkupExtension.html) for a great overall concept

## Themes
Applications that use the HAF theme infrastructure support live swithing of themes. The `IThemesService` provides default light and dark themes and allows provides means to switch the current theme. An `ITheme` is composed of the following colors:
- **Control,** the main color (background) of controls, can be used to distinguish controls from the background
- **Background,** the primary background color
- **Text,** the text color
- **Accent,** a color that is used to bring more visual fidelity to the application, mainly used for headers and seperators
- **Action,** a variation of the accent color that is used to visualize focused and hovered states
- **Light,** a light deviation to the background color, used to seperate parts of the UI, 
- **Strong,** a string deviation of the background color, used for control borders and icons
- **Info,** a color that is used to represent information
- **Warning,** a color that is used to represent warnings
- **Error,** a color that is used to represent errors, also used for validation information

The `ThemeBrush` and `ThemeColor` markup extensions allow easy access to the different colors of the theme.
```
<ComboBox Background="{haf:ThemeBrush Accent}"/>
```
