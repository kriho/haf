using System;

namespace HAF {
  public interface IMainWindowService : IService {
    string GetWindowLayout();
    void SetWindowLayout(string layout);
    void ShowPane(string name, Type type, bool canUserClose);
    string Title { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    bool Topmost { get; set; }
  }
}