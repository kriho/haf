using System.Globalization;

namespace HAF {
  public interface ILocalizationService: IService {
    IReadOnlyObservableCollection<CultureInfo> AvailableCultures { get; }
    string this[string compoundId] { get; }
    CultureInfo SelectedCulture { get; set; }
    void RegisterProvider(CultureInfo culture, ILocalizationProvider provider);
    string GetText(string id);
    string GetText(string contextId, string id);
    string GetText(string id, string pluralId, int count);
    string GetText(string contextId, string id, string pluralId, int count);
    LinkedEvent OnSelectedCultureChanged { get; }
  }
}