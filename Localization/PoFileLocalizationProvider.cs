using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karambolo.PO;

namespace HAF {
  public class PoFileLocalizationProvider: ILocalizationProvider {

    private readonly string filePath;
    private readonly Stream stream;
    private POCatalog catalog;

    public PoFileLocalizationProvider(string filePath) {
      this.filePath = filePath;
    }

    public PoFileLocalizationProvider(Stream stream) {
      this.stream = stream;
    }

    public void Load() {
      var parser = new POParser(new POParserSettings {
      });
      POParseResult result;
      if(this.stream != null) {
        this.stream.Seek(0, SeekOrigin.Begin);
        result = parser.Parse(this.stream);
      } else if(this.filePath != null) {
        using(TextReader reader = File.OpenText(this.filePath)) {
          result = parser.Parse(reader);
        }
      } else {
        throw new Exception("no file path and no stream provided");
      }
      if(result.Success) {
        this.catalog = result.Catalog;
      } else {
        throw new Exception($"failed to parse PO file \"{this.filePath}\": {string.Join(", ", result.Diagnostics)}");
      }
    }

    public void Unload() {
      this.catalog.Clear();
    }

    public string GetText(string id) {
      var key = new POKey(id);
      return this.catalog.GetTranslation(key);
    }

    public string GetText(string contextId, string id) {
      var key = new POKey(id, contextId: contextId);
      return this.catalog.GetTranslation(key);
    }

    public string GetText(string id, string pluralId, int count) {
      var key = new POKey(id, pluralId);
      return this.catalog.GetTranslation(key, count);
    }

    public string GetText(string contextId, string id, string pluralId, int count) {
      var key = new POKey(id, pluralId, contextId);
      return this.catalog.GetTranslation(key, count);
    }
  }
}
