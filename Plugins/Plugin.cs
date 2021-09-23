using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public abstract class Plugin: ObservableObject, IPlugin {
    public LocalizedText Name { get; private set; }
    public LocalizedText Description { get; private set; }
    public string Icon => this.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), true).OfType<ExportMetadataAttribute>().First(a => a.Name == "Icon").Value as string;

    public Plugin() {
      this.Name =  new LocalizedText(this.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), true).OfType<ExportMetadataAttribute>().First(a => a.Name == "Name").Value as string);
      this.Description = new LocalizedText(this.GetType().GetCustomAttributes(typeof(ExportMetadataAttribute), true).OfType<ExportMetadataAttribute>().First(a => a.Name == "Description").Value as string);
    }
  }
}
