using System.ComponentModel;

namespace HAF.Models {
  public interface IProject: INotifyPropertyChanged {
    string FilePath { get; set; }
    bool IsCurrent { get; set; }
    bool IsDefault { get; set; }
    string Name { get; set; }
  }
}