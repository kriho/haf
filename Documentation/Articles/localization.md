# Localization
The framework provides a localization suite that contains everything that is needed to localize real-world applications while supporting a live switching of cultures.
The [ILocalizationService](xref:HAF.ILocalizationService) provides a [gettext](https://www.gnu.org/software/gettext/) interface for resolving a translated text. The service is used to register [ILocalizationProvider](xref:HAF.ILocalizationProvider) instances that provide translations. The [SystemLocalizationProvider](xref:HAF.SystemLocalizationProvider) simply returns the original ids and thus provides the english translations. Use the included [PoFileLocalizationProvider](xref:HAF.PoFileLocalizationProvider) to provide translations via po files.

Translated texts can be resolved in code using the localization service or the static class [Localization](xref:HAF.Localization).
```csharp
this.Description = localizationService.GetText("Select folder that contains movies");
this.Description = Localization.GetText("Select folder that contains movies");
```
Using the [LocalizeExtension](xref:HAF.LocalizeExtension) binding, it is simple to use translated texts in the views. 
```xml
<Button Content="{haf:Localize 'Add from folder'}"/>
```
Dynamically created objects that contain translated texts and persist and must be updated when the selected culture changes. This can be achieved using [LocalizedText](xref:HAF.LocalizedText). It has multiple constructors that mimic the gettext interface. The object is implicitly converted to string and has a [Value](xref:HAF.LocalizedText.Value) property that can be used in XAML bindings and will update when appropriate.
```csharp
theme.Name = new LocalizedText("Dark");
```

The [haf.tools](https://github.com/kriho/haf.tools) CLI has a verb `localize` that scans a project for used strings and generates a pot file.

Notable mentions: 
- [adams85](https://github.com/adams85/aspnetskeleton) for a great way to extract texts from the source files
- [Christina Mosers](https://www.wpftutorial.net/LocalizeMarkupExtension.html) for a great overall concept