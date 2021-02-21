using System;

namespace HAF {
  public interface IHotkeysService : IService {
    void Dispose();
    void Register(System.Windows.Window window);
    void RegisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key, Action action);
    void Unregister();
    void UnregisterHotkey(HotkeysService.Modifiers modifiers, HotkeysService.Keys key);
  }
}