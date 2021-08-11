using System;

namespace HAF {
  public interface IDockingWindowService : IWindowService {
    object Docking { get; set; }

    string GetWindowLayout();
    void SetWindowLayout(string layout);
    void ShowPane(string name);
    void ShowPane(string name, Type viewType, bool canUserClose);
  }
}