using System;

namespace HAF {
  public interface IHotkeysService : IService {
    void Dispose();
    void RegisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key, Action action);
    void UnregisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key);
  }
}