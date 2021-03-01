using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class ServiceAwareCatalog: ComposablePartCatalog {
    private readonly ComposablePartCatalog catalogToFilter;

    public ServiceAwareCatalog(ComposablePartCatalog catalogToFilter) {
      this.catalogToFilter = catalogToFilter;
    }

    public override IQueryable<ComposablePartDefinition> Parts {
      get {
        var serviceTypeIdentities = new List<string>();
        return from part in this.catalogToFilter.Parts
               from exportDefinition in part.ExportDefinitions
               where this.IsMatch(exportDefinition, serviceTypeIdentities)
               select part;
      }
    }

    public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition) {
      var serviceTypeIdentities = new List<string>();
      return from export in base.GetExports(definition)
             where this.IsMatch(export.Item2, serviceTypeIdentities)
             select export;
    }

    private bool IsMatch(ExportDefinition exportDefinition, List<string> serviceTypeIdentities) {
      if(exportDefinition.ContractName.Contains("Service")) {
        if(exportDefinition.Metadata.TryGetValue("ExportTypeIdentity", out var typeIdentity)) {
          if(!serviceTypeIdentities.Contains(typeIdentity)) {
            serviceTypeIdentities.Add(typeIdentity as string);
            return true;
          } else {
            return false;
          }
        } else {
          throw new Exception("service export has no type identity");
        }
      } else {
        // not a service, always export
        return true;
      }
    }
  }

}
