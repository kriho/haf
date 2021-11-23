using HAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HAF {

  public abstract class LinkedObservableObject: ObservableObject {
#if DEBUG
    public string Link;
#endif

    public LinkedObservableObject() {
#if DEBUG
      LinkedObservableObjectManager.Register(this);
#endif
    }

    internal void AssignLinks() {
#if DEBUG
      var derivedType = this.GetType();
      foreach(var propertyInfo in derivedType.GetProperties()) {
        var linkName = $"{this.Link ?? derivedType.Name}/{propertyInfo.Name}";
        if(propertyInfo.PropertyType == typeof(CompoundState) || propertyInfo.PropertyType == typeof(ICompoundState)) {
          if(propertyInfo.GetValue(this) is CompoundState state) {
            state.Link = linkName;
          }
        } else if(propertyInfo.PropertyType == typeof(State) || propertyInfo.PropertyType == typeof(IState)) {
          if(propertyInfo.GetValue(this) is State state) {
            state.Link = linkName;
          }
        } else if(propertyInfo.PropertyType == typeof(Event) || propertyInfo.PropertyType == typeof(IEvent)) {
          if(propertyInfo.GetValue(this) is Event @event) {
            @event.Link = linkName;
          }
        } else if(propertyInfo.PropertyType.IsSubclassOf(typeof(LinkedObservableObject))) {
          if(propertyInfo.GetValue(this) is LinkedObservableObject linkedObject) {
            linkedObject.Link = linkName;
          }
        }
      }
#endif
    }
  }
}
