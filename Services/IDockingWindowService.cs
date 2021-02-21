using System;
using Telerik.Windows.Controls;

namespace HAF {
  public interface IDockingWindowService : IWindowService {
    RadDocking Docking { get; set; }

    string GetWindowLayout();
    void SetWindowLayout(string layout);
    void ShowPane(string name);
    void ShowPane(string name, Type viewType, bool canUserClose);
  }
}