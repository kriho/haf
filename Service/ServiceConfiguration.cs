using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HAF {
  /// <summary>
  /// An entry of the service configuration that can be used to read and write values.
  /// </summary>
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

    /// <summary>
    /// Read a <c>string</c> value from the attributes of the current configuration entry.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="fallbackValue">The value that will be returned if the attribute does not exist.</param>
    /// <returns>The attribute value.</returns>
    public string ReadAttribute(string name, string fallbackValue) {
      return this.ReadAttribute(name) ?? fallbackValue;
    }

    /// <summary>
    /// Try to read a <c>string</c> value from the attributes of the current configuration entry.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>True if the attribute exists.</returns>
    public bool TryReadAttribute(string name, out string value) {
      value = this.ReadAttribute(name);
      return value != null;
    }

    /// <summary>
    /// Read a typed value from the attributes of the current configuration entry.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="fallbackValue">The value that will be returned if the attribute does not exist.</param>
    /// <returns>The attribute value.</returns>
    public T ReadAttribute<T>(string name, T fallbackValue) {
      return Utils.TryParse(this.ReadAttribute(name), fallbackValue);
    }

    /// <summary>
    /// Try to read a typed value from the attributes of the current configuration entry.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>True if the attribute exists.</returns>
    public bool TryReadAttribute<T>(string name, out T value) {
      return Utils.TryParse(this.ReadAttribute(name), out value);
    }

    /// <summary>
    /// Write a <c>string</c> value as attribute.
    /// </summary>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>The current configuration entry. Used to enable chain syntax.</returns>
    public ServiceConfigurationEntry WriteAttribute(string name, string value) {
      if(value != null) {
        this.context.SetAttributeValue(name, value);
      }
      return this;
    }

    /// <summary>
    /// Write a typed value as attribute.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>The current configuration entry. Used to enable chain syntax.</returns>
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

    /// <summary>
    /// Read a <c>string</c> value from a named child element of the current configuration entry.
    /// </summary>
    /// <param name="name">The element name.</param>
    /// <param name="fallbackValue">The value that will be returned if the element does not exist</param>
    /// <param name="index">The index of the target element in the list of child elements with a matching name.</param>
    /// <returns>The value of the element.</returns>
    public string ReadValue(string name, string fallbackValue, int index = 0) {
      return this.ReadValue(name, index) ?? fallbackValue;
    }

    /// <summary>
    /// Try to read a <c>string</c> value from a named child element of the current configuration entry.
    /// </summary>
    /// <param name="name">The element name.</param>
    /// <param name="value">The element value.</param>
    /// <param name="index">The index of the target element in the list of child elements with a matching name.</param>
    /// <returns>True if the element exists.</returns>
    public bool TryReadValue(string name, out string value, int index = 0) {
      value = this.ReadValue(name, index);
      return value != null;
    }

    /// <summary>
    /// Read a typed value from a named child element of the current configuration entry.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">The element name.</param>
    /// <param name="fallbackValue">The value that will be returned if the element does not exist</param>
    /// <param name="index">The index of the target element in the list of child elements with a matching name.</param>
    /// <returns>The value of the element.</returns>
    public T ReadValue<T>(string name, T fallbackValue, int index = 0) {
      return Utils.TryParse(this.ReadValue(name, index), fallbackValue);
    }

    /// <summary>
    /// Try to read a typed value from a named child element of the current configuration entry.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">The element name.</param>
    /// <param name="value">The element value.</param>
    /// <param name="index">The index of the target element in the list of child elements with a matching name.</param>
    /// <returns>True if the element exists.</returns>
    public bool TryReadValue<T>(string name, out T value, int index = 0) {
      return Utils.TryParse(this.ReadValue(name, index), out value);
    }

    /// <summary>
    /// Read <c>string</c> values from all named child elements of the current configuration entry.
    /// </summary>
    /// <param name="name">The name of the elements.</param>
    /// <returns>The element values.</returns>
    public IEnumerable<string> ReadValues(string name) {
      var index = 0;
      while(this.TryReadValue(name, out string value, index++)) {
        yield return value;
      }
    }

    /// <summary>
    /// Read typed values from all named child elements of the current configuration entry.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">The name of the elements.</param>
    /// <returns>The element values.</returns>
    public IEnumerable<T> ReadValues<T>(string name) {
      var index = 0;
      while(this.TryReadValue(name, out T value, index++)) {
        yield return value;
      }
    }

    /// <summary>
    /// Write a <c>string</c> value as child element.
    /// </summary>
    /// <param name="name">Name of the element.</param>
    /// <param name="value">The value of the element.</param>
    /// <returns>The current configuration entry. Used to enable chain syntax.</returns>
    public ServiceConfigurationEntry WriteValue(string name, string value) {
      if(value != null) { 
        this.context.Add(new XElement(name) {
          Value = value
        });
      }
      return this;
    }

    /// <summary>
    /// Write a typed value as child element.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="name">Name of the element.</param>
    /// <param name="value">The value of the element.</param>
    /// <returns>The current configuration entry. Used to enable chain syntax.</returns>
    public ServiceConfigurationEntry WriteValue<T>(string name, T value) {
      return this.WriteValue(name, value?.ToString());
    }

    /// <summary>
    /// Write typed values as child elements.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="name">Name of the elements.</param>
    /// <param name="values">The element values.</param>
    /// <returns>The current configuration entry. Used to enable chain syntax.</returns>
    public ServiceConfigurationEntry WriteValues<T>(string name, IEnumerable<T> values) {
      foreach(var value in values) {
        this.WriteValue(name, value);
      }
      return this;
    }

    /// <summary>
    /// Try to read a configuration entry.
    /// </summary>
    /// <param name="name">The name of the configuration entry.</param>
    /// <param name="entry">The configuration entry.</param>
    /// <returns>True if the configuration entry exists.</returns>
    public bool TryReadEntry(string name, out ServiceConfigurationEntry entry) {
      var element = this.context.Descendants(name).FirstOrDefault();
      if(element != null) { 
        entry = new ServiceConfigurationEntry(element);
        return true;
      }
      entry = null;
      return false;
    }

    /// <summary>
    /// Read a configuration entries.
    /// </summary>
    /// <param name="name">The name of the configuration entries.</param>
    /// <returns>The configuration entries.</returns>
    public IEnumerable<ServiceConfigurationEntry> ReadEntries(string name) {
      return this.context.Descendants(name).Select(d => new ServiceConfigurationEntry(d));
    }

    /// <summary>
    /// Write a configuration entry.
    /// </summary>
    /// <param name="name">Name of the configuration entry.</param>
    /// <param name="reuseExisting">Reuse an existing configuration entry if possible.</param>
    /// <returns></returns>
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

    /// <summary>
    /// The parent configuration entry or the current configuration entry if no parent exists.
    /// </summary>
    public ServiceConfigurationEntry Parent {
      get => new ServiceConfigurationEntry(this.context.Parent ?? this.context);
    }
  }

  public class ServiceConfiguration: ServiceConfigurationEntry {

    private readonly XDocument document;

    /// <summary>
    /// Create a new configuration.
    /// </summary>
    /// <param name="name">The name of the root element.</param>
    public ServiceConfiguration(string name) {
      this.context = new XElement(name);
      this.document = new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        this.context
      );
    }

    /// <summary>
    /// Create a configuration from an existing XML document.
    /// </summary>
    /// <param name="document"></param>
    public ServiceConfiguration(XDocument document) {
      this.document = document;
      this.context = document.Root;
    }

    /// <summary>
    /// Create a configuration from a file that represents an XML document.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>The configuration.</returns>
    public static ServiceConfiguration FromFile(string filePath) {
      return new ServiceConfiguration(XDocument.Load(filePath));
    }

    /// <summary>
    /// Save the configuration to a file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    public void SaveToFile(string filePath) {
      this.document.Save(filePath);
    }

    /// <summary>
    /// Create a configuration from a stream that represents an XML document.
    /// </summary>
    /// <param name="stream">The source stream.</param>
    /// <returns>The configuration.</returns>
    public static ServiceConfiguration FromStream(Stream stream) {
      return new ServiceConfiguration(XDocument.Load(stream));
    }

    /// <summary>
    /// Save the configuration to a stream.
    /// </summary>
    /// <param name="filePath">The target stream.</param>
    public void SaveToStream(Stream stream) {
      this.document.Save(stream);
    }
  }
}
