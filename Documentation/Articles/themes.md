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
