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

    #region attribute

    private string ReadAttribute(string name) {
      return this.context.Attribute(name)?.Value;
    }

    public string ReadStringAttribute(string name, string fallbackValue) {
      return this.ReadAttribute(name) ?? fallbackValue;
    }

    public bool ReadStringAttribute(string name, out string value) {
      value = this.ReadAttribute(name);
      return value != null;
    }

    public bool ReadBooleanAttribute(string name, bool fallbackValue) {
      if (bool.TryParse(this.ReadAttribute(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadBooleanAttribute(string name, out bool value) {
      return bool.TryParse(this.ReadAttribute(name), out value);
    }

    public int ReadIntegerAttribute(string name, int fallbackValue) {
      if (int.TryParse(this.ReadAttribute(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadIntegerAttribute(string name, out int value) {
      return int.TryParse(this.ReadAttribute(name), out value);
    }

    public double ReadDoubleAttribute(string name, double fallbackValue) {
      if (double.TryParse(this.ReadAttribute(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadDoubleAttribute(string name, out double value) {
      return double.TryParse(this.ReadAttribute(name), out value);
    }

    public ServiceConfigurationEntry WriteAttribute(string name, string value) {
      this.context.SetAttributeValue(name, value);
      return this;
    }

    public ServiceConfigurationEntry WriteAttribute(string name, bool value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public ServiceConfigurationEntry WriteAttribute(string name, int value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public ServiceConfigurationEntry WriteAttribute(string name, double value) {
      return this.WriteAttribute(name, value.ToString());
    }

    #endregion

    #region value

    private string ReadValue(string name) {
      return this.context.Descendants(name).FirstOrDefault()?.Value;
    }

    public string ReadStringValue(string name, string fallbackValue) {
      return this.ReadValue(name) ?? fallbackValue;
    }

    public bool ReadStringValue(string name, out string value) {
      value = this.ReadValue(name);
      return value != null;
    }

    public bool ReadBooleanValue(string name, bool fallbackValue) {
      if(bool.TryParse(this.ReadValue(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadBooleanValue(string name, out bool value) {
      return bool.TryParse(this.ReadValue(name), out value);
    }

    public int ReadIntegerValue(string name, int fallbackValue) {
      if (int.TryParse(this.ReadValue(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadIntegerValue(string name, out int value) {
      return int.TryParse(this.ReadValue(name), out value);
    }

    public double ReadDoubleValue(string name, double fallbackValue) {
      if (double.TryParse(this.ReadValue(name), out var value)) {
        return value;
      } else {
        return fallbackValue;
      }
    }

    public bool ReadDoubleValue(string name, out double value) {
      return double.TryParse(this.ReadValue(name), out value);
    }

    public ServiceConfigurationEntry WriteValue(string name, string value) {
      var element = this.context.Element(name);
      element.Value = value;
      this.context.Add(element);
      return this;
    }

    public ServiceConfigurationEntry WriteValue(string name, bool value) {
      return this.WriteValue(name, value.ToString());
    }

    public ServiceConfigurationEntry WriteValue(string name, int value) {
      return this.WriteValue(name, value.ToString());
    }

    public ServiceConfigurationEntry WriteValue(string name, double value) {
      return this.WriteValue(name, value.ToString());
    }

    #endregion

    #region entry

    public bool ReadEntry(string name, out ServiceConfigurationEntry entry) {
      var element = this.context.Descendants(name).FirstOrDefault();
      if(element != null) { 
        entry = new ServiceConfigurationEntry(element);
        return true;
      }
      entry = null;
      return false;
    }

    public ServiceConfigurationEntry WriteEntry(string name, bool reuseExisting) {
      if (reuseExisting) {
        var element = this.context.Descendants(name).FirstOrDefault();
        if(element != null) {
          return new ServiceConfigurationEntry(element);
        }
      }
      return new ServiceConfigurationEntry(this.context.Element(name));
    }

    #endregion

    public IEnumerable<string> ReadValues(string name) {
      return this.context.Descendants(name).Select(d => d.Value);
    }

    public IEnumerable<ServiceConfigurationEntry> ReadEntries(string name) {
      return this.context.Descendants(name).Select(d => new ServiceConfigurationEntry(d));
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
