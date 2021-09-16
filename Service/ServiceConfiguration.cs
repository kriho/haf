using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HAF {

  public class ServiceConfigurationEntry {

    protected XElement context;

    public ServiceConfigurationEntry() {
      // needed for inheritance in class Configuration
    }

    public ServiceConfigurationEntry(XElement element) {
      this.context = element;
    }

    private string ReadAttribute(string name) {
      return this.context.Attribute(name)?.Value;
    }

    public string ReadAttribute(string name, string fallbackValue) {
      return this.ReadAttribute(name) ?? fallbackValue;
    }

    public bool TryReadAttribute(string name, out string value) {
      value = this.ReadAttribute(name);
      return value != null;
    }

    public T ReadAttribute<T>(string name, T fallbackValue) {
      return Utils.TryParse(this.ReadAttribute(name), fallbackValue);
    }

    public bool TryReadAttribute<T>(string name, out T value) {
      return Utils.TryParse(this.ReadAttribute(name), out value);
    }

    public ServiceConfigurationEntry WriteAttribute(string name, string value) {
      if(value != null) {
        this.context.SetAttributeValue(name, value);
      }
      return this;
    }

    public ServiceConfigurationEntry WriteAttribute<T>(string name, T value) {
      return this.WriteAttribute(name, value?.ToString());
    }

    private string ReadValue(string name, int index = 0) {
      if(index == 0) {
        return this.context.Element(name)?.Value;
      } else {
        return this.context.Descendants(name).ElementAtOrDefault(index)?.Value;
      }
    }

    public string ReadValue(string name, string fallbackValue, int index = 0) {
      return this.ReadValue(name, index) ?? fallbackValue;
    }

    public bool TryReadValue(string name, out string value, int index = 0) {
      value = this.ReadValue(name, index);
      return value != null;
    }

    public T ReadValue<T>(string name, T fallbackValue, int index = 0) {
      return Utils.TryParse(this.ReadValue(name, index), fallbackValue);
    }

    public bool TryReadValue<T>(string name, out T value, int index = 0) {
      return Utils.TryParse(this.ReadValue(name, index), out value);
    }

    public IEnumerable<string> ReadValues(string name) {
      var index = 0;
      while(this.TryReadValue(name, out string value, index++)) {
        yield return value;
      }
    }

    public IEnumerable<T> ReadValues<T>(string name) {
      var index = 0;
      while(this.TryReadValue(name, out T value, index++)) {
        yield return value;
      }
    }

    public ServiceConfigurationEntry WriteValue(string name, string value) {
      if(value != null) { 
        this.context.Add(new XElement(name) {
          Value = value
        });
      }
      return this;
    }

    public ServiceConfigurationEntry WriteValue<T>(string name, T value) {
      return this.WriteValue(name, value?.ToString());
    }

    public ServiceConfigurationEntry WriteValues<T>(string name, IEnumerable<T> values) {
      foreach(var value in values) {
        this.WriteValue(name, value);
      }
      return this;
    }

    public bool TryReadEntry(string name, out ServiceConfigurationEntry entry) {
      var element = this.context.Descendants(name).FirstOrDefault();
      if(element != null) { 
        entry = new ServiceConfigurationEntry(element);
        return true;
      }
      entry = null;
      return false;
    }

    public IEnumerable<ServiceConfigurationEntry> ReadEntries(string name) {
      return this.context.Descendants(name).Select(d => new ServiceConfigurationEntry(d));
    }

    public ServiceConfigurationEntry WriteEntry(string name, bool reuseExisting) {
      XElement element;
      if (reuseExisting) {
        element = this.context.Element(name);
        if(element != null) {
          return new ServiceConfigurationEntry(element);
        }
      }
      element = new XElement(name);
      this.context.Add(element);
      return new ServiceConfigurationEntry(element);
    }

    public ServiceConfigurationEntry Parent() {
      return new ServiceConfigurationEntry(this.context.Parent ?? this.context);
    }
  }

  public class ServiceConfiguration: ServiceConfigurationEntry {

    private readonly XDocument document;

    public ServiceConfiguration(string name) {
      this.context = new XElement(name);
      this.document = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        this.context
      );
    }

    public ServiceConfiguration(XDocument document) {
      this.document = document;
      this.context = document.Root;
    }

    public static ServiceConfiguration FromFile(string filePath) {
      return new ServiceConfiguration(XDocument.Load(filePath));
    }

    public void SaveToFile(string filePath) {
      this.document.Save(filePath);
    }
  }
}
