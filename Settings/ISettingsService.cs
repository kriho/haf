using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HAF {
  public interface ISettingsService : IService {
    /// <summary>
    /// Current settings filter.
    /// </summary>
    string Filter { get; set; }

    /// <summary>
    /// Clear settings filter.
    /// </summary>
    IRelayCommand DoClearFilter { get; }

    /// <summary>
    /// Filtered settings.
    /// </summary>
    ICollectionView FilteredRegistrations { get; }

    /// <summary>
    /// Reveal a setting by its name. Requires <see cref="ISettingsService.RevealSetting"/> to be set.
    /// </summary>
    IRelayCommand<string> DoRevealSetting { get; }

    /// <summary>
    /// Must be set to enable use of <see cref="ISettingsService.DoRevealSetting"/>. The implementation must use the provided datacontext to locate an reveal the associated drawer.
    /// </summary>
    Action<ISettingsRegistration> RevealSetting { get; set; }

    /// <summary>
    /// Register settings region.
    /// </summary>
    /// <param name="name">Internal name of the region. Used to reference the region.</param>
    /// <returns>The registered region.</returns>
    ISettingsRegion RegisterRegion(string name, string displayName = null, string description = null, int? displayOrder = null);

    /// <summary>
    /// Try to find setting.
    /// </summary>
    /// <typeparam name="T">Type of the setting value.</typeparam>
    /// <param name="regionName">Internal name of the setting region.</param>
    /// <param name="name">Internal name of the setting.</param>
    /// <param name="value">The discovered setting.</param>
    /// <returns>True if a setting was found.</returns>
    bool TryFindSetting<T>(string regionName, string name, out T value);

    /// <summary>
    /// The available setting regions.
    /// </summary>
    IReadOnlyObservableCollection<ISettingsRegion> Regions { get; }

    /// <summary>
    /// Get a matching settings drawer for the provided setting value.
    /// </summary>
    /// <returns>The first matching settings drawer.</returns>
    ISettingsDrawer GetDrawer(ISettingsValueBase settingsValue);

    /// <summary>
    /// Available settings drawers.
    /// </summary>
    IReadOnlyList<ExportFactory<ISettingsDrawer, ISettingsDrawerMeta>> Drawers { get; }
    
    /// <summary>
    /// Save service configuration for specific settings owner.
    /// </summary>
    /// <param name="owner">The settings ownser.</param>
    /// <param name="configuration">Target configuration.</param>
    Task SaveConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
  
    /// <summary>
    /// Load service configuration for specific settings owner.
    /// </summary>
    /// <param name="owner">The settings ownser.</param>
    /// <param name="configuration">Source configuration.</param>
    Task LoadConfiguration(ISettingsOwner owner, ServiceConfiguration configuration);
  }
}