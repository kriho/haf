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

## Localization
The framework provides a localization suite that provides everything that is needed to localize real-world applications while supporting a live switching of cultures.
The `LocalizationService` provides a [gettext](https://www.gnu.org/software/gettext/) interface for resolving a translated text. The service is used to register `ILocalizationProvider` instances that provide translations. The `SystemLocalizationProvider` simply returns the original ids and thus provides the english translations. Use the included `PoFileLocalizationProvider` to provide translations via po files.
```
this.Description = localizationService.GetText("Select folder that contains movies");
```
Using the `LocalizeExtension` binding, it is simple to place translated texts in the UI. 
```
<TextBlock Text="{haf:Localize 'Add from folder'}"/>
```
Dynamically created objects that contain translated texts and persist and must be updated when the selected culture changes. This can be achieved using the `LocalizedText`. It has multiple constructory that mimic the gettext interface. The object is implicetly converted to string and has a `Value` member that can be used in XAML bindings and will update when appropriate.
```
theme.Name = new LocalizedText("Dark");
```
The haf.tools CLI has a verb "localize" that scans a project for used strings and generates a pot file.
