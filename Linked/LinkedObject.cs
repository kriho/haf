using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Data;

namespace HAF {

  public abstract class LinkedObject: ObservableObject {

    public string Link = null;

    protected abstract void Initialize();

    public LinkedObject() {
      this.Initialize();
#if DEBUG
      var derivedType = this.GetType();
      foreach (var propertyInfo in derivedType.GetProperties()) {
        var linkName = $"{this.Link ?? derivedType.Name}/{propertyInfo.Name}";
        if (propertyInfo.PropertyType == typeof(LinkedDependency)) {
          if (propertyInfo.GetValue(this) is LinkedDependency dependency) {
            dependency.Link = linkName;
          }
        } else if (propertyInfo.PropertyType == typeof(LinkedState)) {
          if (propertyInfo.GetValue(this) is LinkedState state) {
            state.Name = linkName;
          }
        } else if (propertyInfo.PropertyType == typeof(LinkedEvent)) {
          if (propertyInfo.GetValue(this) is LinkedEvent @event) {
            @event.Name = linkName;
          }
        }
      }
#endif
    }
  }
}
