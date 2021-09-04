using System.Globalization;

namespace HAF {
  public interface ILocalizationService: IService {
    IReadOnlyObservableCollection<CultureInfo> SupportedCultures { get; }
    string this[string english] { get; }
    CultureInfo CurrentCulture { get; }
    void SetCulture(CultureInfo culture);
    void RegisterProvider(CultureInfo culture, ILocalizationProvider provider);
  }
}