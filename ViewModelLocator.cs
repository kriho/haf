using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  /*
  public class ViewModelLocator : DynamicObject {
    [ImportMany(typeof(ViewModel), AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
    private IEnumerable<Lazy<object, IViewMetadata>> ViewModels { get; set; }
    private static Dictionary<string, object> dictionary = new Dictionary<string, object>();

    public int Count { get { return dictionary.Count; } }

    public override bool TryGetMember(GetMemberBinder binder, out object result) {
      var name = binder.Name;
      if (!dictionary.TryGetValue(name, out result)) {
        try {
          if (ViewModels == null) {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ViewModelLocator).Assembly));
            var container = new CompositionContainer(catalog);
            var compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);
          }
          dictionary[binder.Name] = (result = ViewModels.Single(v => v.Metadata.Name.Equals(name)).Value);
          return result != null;
        } catch (Exception ex) {
          Console.WriteLine(ex);
        }
      }
      return true;
    }
  }*/
}
