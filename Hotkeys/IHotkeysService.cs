using System;

namespace HAF {
  public interface IHotkeysService : IService {
    void Dispose();

    /// <summary>
    /// Register action for hotkey with modifiers.
    /// </summary>
    /// <param name="modifiers">Required modifiers for the hotkey to trigger.</param>
    /// <param name="key">Key that triggers the registered action when pressed.</param>
    /// <param name="action">The action that is executed when the hotkey triggers.</param>
    void RegisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key, Action action);

    /// <summary>
    /// Unregister last registered action for hotkey and modifier.
    /// </summary>
    /// <param name="modifiers">Modifiers of the registered hotkey.</param>
    /// <param name="key">Key of the registered hotkey.</param>
    void UnregisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key);
  }
}