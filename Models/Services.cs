using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace HAF {
  
  public abstract class Services: ObservableObject {

    public Services() {
      Backend.Container.ComposeParts(true);
    }

  }
}
