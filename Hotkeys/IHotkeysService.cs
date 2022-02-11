using System;

namespace HAF {
  public interface IHotkeysService : IService {
    void Dispose();

    /// <summary>
    /// Register action for hotkey with modifier.
    /// </summary>
    /// <param name="action">The action that is executed when all provided modifiers and the hot key are pressed.</param>
    void RegisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key, Action action);
    
    /// <summary>
    /// Unregister last registered action for hotkey and modifier.
    /// </summary>
    void UnregisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key);
  }
}