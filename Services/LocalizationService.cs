using System;
using System.Collections.Generic;
using System.Globalization;

namespace HAF {
  public class LocalizationService: Service, ILocalizationService {
    private Dictionary<CultureInfo, ILocalizationProvider> providers = new Dictionary<CultureInfo, ILocalizationProvider>();
    private ILocalizationProvider currentProvider;

    public CultureInfo CurrentCulture { get; private set; }

    private ObservableCollection<CultureInfo> supportedCultures = new ObservableCollection<CultureInfo>();
    public IReadOnlyObservableCollection<CultureInfo> SupportedCultures => this.supportedCultures;

    public string this[string english] {
      get {
        if (this.currentProvider == null) {
        #if DEBUG
          if(ObservableObject.IsInDesignModeStatic) {
            return english;
          }
        #endif
          throw new InvalidOperationException($"no culture selected for localization");
        }
        return this.currentProvider.ProvideString(english);
      }
    }

    public void SetCulture(CultureInfo culture) {
      if(this.providers.TryGetValue(culture, out var provider)) {
        if(this.currentProvider != null) {
          // unload last provider
          this.currentProvider.Unload();
        }
        this.currentProvider = provider;
        this.currentProvider.Load();
        this.CurrentCulture = culture;
        this.NotifyPropertyChanged(() => this.CurrentCulture);
        this.NotifyPropertyChanged("Item[]");
      } else {
        throw new InvalidOperationException($"no localization provider found for culture {culture.DisplayName}");
      }
    }

    public void RegisterProvider(CultureInfo culture, ILocalizationProvider provider) {
      if(this.providers.ContainsKey(culture)) {
        throw new InvalidOperationException($"the culture {culture.DisplayName} was already registered");
      }
      this.providers[culture] = provider;
      this.supportedCultures.Add(culture);
    }
  }
}