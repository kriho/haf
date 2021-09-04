using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  /// <summary>
  /// use MEF to export the view model, this will make it available to the ViewModelLocator
  /// </summary>
  /// <remarks>
  /// the name of the file without extension is used as the view model name by default
  /// </remarks>
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
  public class ExportViewModelAttribute: ExportAttribute {
    public string Name { get; private set; }

    public ExportViewModelAttribute([CallerFilePath] string name = null) : base(typeof(ViewModel)) {
      Name = System.IO.Path.GetFileNameWithoutExtension(name);
    }
  }

  public interface IViewModelMetadata {
    string Name { get; }
  }

  /// <summary>
  /// assign the view models directly and let MEF handle the dependency injection
  /// </summary>
  [TypeDescriptionProvider(typeof(ModelViewMapDescriptionProvider))]
  public class ViewModelLocator: DynamicObject, ITypedList {

    [ImportMany(typeof(ViewModel), AllowRecomposition = true)]
    private IEnumerable<Lazy<object, IViewModelMetadata>> viewModels { get; set; }

    private static Dictionary<string, object> cache = new Dictionary<string, object>();

    public int Count {
      get { return ViewModelLocator.cache.Count; }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result) {
      if (!ViewModelLocator.cache.TryGetValue(binder.Name, out result)) {
        try {
          if (this.viewModels == null) {
            Configuration.Container.ComposeParts(this);
          }
          result = this.viewModels.Single(v => v.Metadata.Name == binder.Name).Value;
          ViewModelLocator.cache[binder.Name] = result;
        } catch (Exception ex) {
          Console.WriteLine(ex);
          result = null;
        }
      }
      return result != null;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value) {
      ViewModelLocator.cache[binder.Name] = value;
      return true;
    }

    public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
      var result = new PropertyDescriptorCollection(null);
      foreach (var pair in ViewModelLocator.cache) {
        result.Add(new ModelViewPropertyDescriptor(pair.Key, pair.Value));
      }
      return result;
    }

    public string GetListName(PropertyDescriptor[] listAccessors) {
      return "Models";
    }

    internal class ModelViewPropertyDescriptor: PropertyDescriptor {
      internal object ModelView { get; set; }

      public ModelViewPropertyDescriptor(string name, object modelView) : base(name, null) {
        ModelView = modelView;
      }

      public override bool IsReadOnly {
        get { return true; }
      }

      public override bool CanResetValue(object component) {
        return false;
      }

      public override Type ComponentType {
        get { return ModelView.GetType(); }
      }

      public override object GetValue(object component) {
        return ModelView;
      }

      public override Type PropertyType {
        get { return ModelView.GetType(); }
      }

      public override void ResetValue(object component) {
        throw new NotImplementedException();
      }

      public override void SetValue(object component, object value) {
        throw new NotImplementedException();
      }

      public override bool ShouldSerializeValue(object component) {
        return true;
      }

      public override string ToString() {
        return this.Name;
      }
    }

    private class ModelViewMapDescriptionProvider: TypeDescriptionProvider {

      public ModelViewMapDescriptionProvider() : this(TypeDescriptor.GetProvider(typeof(ViewModelLocator))) {
      }

      public ModelViewMapDescriptionProvider(TypeDescriptionProvider parent) : base(parent) {
      }

      public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
        return new ModelViewDescriptor(base.GetTypeDescriptor(objectType, instance));
      }
    }

    private class ModelViewDescriptor: CustomTypeDescriptor {

      public ModelViewDescriptor(ICustomTypeDescriptor descriptor) : base(descriptor) {
      }

      public override PropertyDescriptorCollection GetProperties() {
        var result = new PropertyDescriptorCollection(null);
        foreach (var pair in ViewModelLocator.cache) {
          result.Add(new ModelViewPropertyDescriptor(pair.Key, pair.Value));
        }
        return result;
      }
    }
  }
}
