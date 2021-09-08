﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;

namespace HAF {
  [Export(typeof(ILocalizationService)), PartCreationPolicy(CreationPolicy.Shared)]
  public class LocalizationService: Service, ILocalizationService {
    private Dictionary<CultureInfo, ILocalizationProvider> providers = new Dictionary<CultureInfo, ILocalizationProvider>();
    private ILocalizationProvider currentProvider;

    private CultureInfo selectedCulture;
    public CultureInfo SelectedCulture {
      get => this.selectedCulture;
      set {
        if(this.SetValue(ref this.selectedCulture, value)) {
          if(this.providers.TryGetValue(value, out var provider)) {
            if(this.currentProvider != null) {
              // unload last provider
              this.currentProvider.Unload();
            }
            this.currentProvider = provider;
            this.currentProvider.Load();
            this.NotifyPropertyChanged("Item[]");
            System.Threading.Thread.CurrentThread.CurrentUICulture = value;
            System.Threading.Thread.CurrentThread.CurrentCulture = value;
            this.OnSelectedCultureChanged.Fire();
          } else {
            throw new InvalidOperationException($"no localization provider found for culture {value.DisplayName}");
          }
        }
      }
    }

    private ObservableCollection<CultureInfo> availableCultures = new ObservableCollection<CultureInfo>();
    public IReadOnlyObservableCollection<CultureInfo> AvailableCultures => this.availableCultures;

    public LinkedEvent OnSelectedCultureChanged { get; private set; } = new LinkedEvent(nameof(OnSelectedCultureChanged));

    public string this[string compoundId] {
      get {
      #if DEBUG
        if(ObservableObject.IsInDesignModeStatic) {
          return "$" + compoundId + "$";
        }
      #endif
        var index = compoundId.IndexOf("//");
        if(index == -1) {
          return this.currentProvider?.GetText(compoundId) ?? compoundId;
        } else {
          return this.currentProvider?.GetText(compoundId.Substring(0, index), compoundId.Substring(index + 2)) ?? compoundId.Substring(index + 2);
        }
      }
    }

    public LocalizationService() {
      var english = new CultureInfo("en-US");
      this.RegisterProvider(english, new SystemLocalizationProvider());
      this.SelectedCulture = english;
    }

    public void RegisterProvider(CultureInfo culture, ILocalizationProvider provider) {
      if(this.providers.ContainsKey(culture)) {
        throw new InvalidOperationException($"the culture {culture.DisplayName} was already registered");
      }
      this.providers[culture] = provider;
      this.availableCultures.Add(culture);
    }

    public string GetText(string id) {
      return this.currentProvider.GetText(id);
    }

    public string GetText(string contextId, string id) {
      return this.currentProvider.GetText(contextId, id);
    }

    public string GetText(string contextId, string id, string pluralId, int count) {
      return this.currentProvider.GetText(contextId, id, pluralId, count);
    }

    public string GetText(string id, string pluralId, int count) {
      return this.currentProvider.GetText(id, pluralId, count);
    }

    public override void ClearConfiguration() {
      this.SelectedCulture = this.AvailableCultures.First();
    }

    public override void SaveConfiguration(ServiceConfiguration configuration) {
      if(this.selectedCulture != null) {
        configuration.WriteValue("language", this.selectedCulture.Name);
      }
    }

    public override void LoadConfiguration(ServiceConfiguration configuration) {
      if(configuration.ReadStringValue("language", out var name)) {
        var culture = this.availableCultures.FirstOrDefault(c => c.Name == name);
        if(culture != null) {
          this.SelectedCulture = culture;
        }
      }
    }
  }
}