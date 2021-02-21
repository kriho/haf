using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HAF {

  public class ConfigurationEntry {

    protected XElement context;

    public ConfigurationEntry() {
      // needed for inheritance in class Configuration
    }

    public ConfigurationEntry(XElement element) {
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

    public ConfigurationEntry WriteAttribute(string name, string value) {
      this.context.SetAttributeValue(name, value);
      return this;
    }

    public ConfigurationEntry WriteAttribute(string name, bool value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public ConfigurationEntry WriteAttribute(string name, int value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public ConfigurationEntry WriteAttribute(string name, double value) {
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

    public ConfigurationEntry WriteValue(string name, string value) {
      var element = this.context.Element(name);
      element.Value = value;
      this.context.Add(element);
      return this;
    }

    public ConfigurationEntry WriteValue(string name, bool value) {
      return this.WriteValue(name, value.ToString());
    }

    public ConfigurationEntry WriteValue(string name, int value) {
      return this.WriteValue(name, value.ToString());
    }

    public ConfigurationEntry WriteValue(string name, double value) {
      return this.WriteValue(name, value.ToString());
    }

    #endregion

    #region entry

    public bool ReadEntry(string name, out ConfigurationEntry entry) {
      var element = this.context.Descendants(name).FirstOrDefault();
      if(element != null) { 
        entry = new ConfigurationEntry(element);
        return true;
      }
      entry = null;
      return false;
    }

    public ConfigurationEntry WriteEntry(string name, bool reuseExisting) {
      if (reuseExisting) {
        var element = this.context.Descendants(name).FirstOrDefault();
        if(element != null) {
          return new ConfigurationEntry(element);
        }
      }
      return new ConfigurationEntry(this.context.Element(name));
    }

    #endregion

    public IEnumerable<string> ReadValues(string name) {
      return this.context.Descendants(name).Select(d => d.Value);
    }

    public IEnumerable<ConfigurationEntry> ReadEntries(string name) {
      return this.context.Descendants(name).Select(d => new ConfigurationEntry(d));
    }

  }

  public class Configuration: ConfigurationEntry {

    private readonly XDocument document;

    public Configuration(string name) {
      this.document = new XDocument();
      this.document.Add(new XProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\""));
      this.context = this.document.Element(name);
    }

    public Configuration(XDocument document) {
      this.document = document;
      this.context = document.Root;
    }

    public static Configuration FromFile(string filePath) {
      return new Configuration(XDocument.Load(filePath));
    }

    public void SaveToFile(string filePath) {
      this.document.Save(filePath);
    }
  }
}
