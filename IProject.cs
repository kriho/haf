using System.ComponentModel;

namespace HAF.Models {
  public interface IProject: INotifyPropertyChanged {
    /// <summary>
    /// File path of the project.
    /// </summary>
    string FilePath { get; set; }

    /// <summary>
    /// Is the project currently loaded.
    /// </summary>
    bool IsCurrent { get; set; }

    /// <summary>
    /// Is the project loaded on applycation start.
    /// </summary>
    bool IsDefault { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    string Name { get; set; }
  }
}