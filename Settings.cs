using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HAF {

  public class SettingsEntry {

    protected XmlElement root;

    public SettingsEntry() {
    }

    public SettingsEntry(XmlElement group) {
      this.root = group;
    }

    private string EscapeXml(string xml) {
      if (xml == null) {
        return null;
      }
      return xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
    }

    private string UnescapeXml(string xml) {
      if(xml == null) {
        return null;
      }
      return xml.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&amp;", "&");
    }


    public string ReadAttribute(string name) {
      return this.root.GetAttribute(name);
    }

    public int ReadNumberAttribute(string name) {
      return int.Parse(this.root.GetAttribute(name));
    }

    public SettingsEntry WriteAttribute(string name, string value) {
      this.root.SetAttribute(name, value);
      return this;
    }

    public SettingsEntry WriteAttribute(string name, bool value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public SettingsEntry WriteAttribute(string name, double value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public SettingsEntry WriteAttribute(string name, int value) {
      return this.WriteAttribute(name, value.ToString());
    }

    public string ReadValue(string name) {
      return this.root.SelectSingleNode($"./{name}")?.InnerText;
    }

    public IEnumerable<string> ReadValues(string name) {
      return this.root.SelectNodes($"./{name}").Cast<XmlElement>().Select(e => e.InnerText);
    }

    public SettingsEntry WriteValue(string name, double value) {
      return this.WriteValue(name, value.ToString());
    }

    public SettingsEntry WriteValue(string name, bool value) {
      return this.WriteValue(name, value.ToString());
    }

    public SettingsEntry WriteValue(string name, string value) {
      var element = this.root.OwnerDocument.CreateElement(name);
      element.InnerText = value;
      this.root.AppendChild(element);
      return this;
    }

    public SettingsEntry FindEntry(string xpath) {
      if (!(this.root.SelectSingleNode($"./{xpath}") is XmlElement element)) {
        return null;
      }
      return new SettingsEntry(element);
    }

    public SettingsEntry ReadEntry(string name) {
      if (!(this.root.SelectSingleNode($"./{name}") is XmlElement element)) {
        return null;
      }
      return new SettingsEntry(element);
    }

    public IEnumerable<SettingsEntry> ReadEntries(string name) {
      return this.root.SelectNodes($"./{name}").Cast<XmlElement>().Select(e => new SettingsEntry(e));
    }

    public SettingsEntry WriteEntry(string name) {
      var element = this.root.OwnerDocument.CreateElement(name);
      this.root.AppendChild(element);
      return new SettingsEntry(element);
    }
  }

  public class Settings: SettingsEntry {

    private readonly XmlDocument document;

    public Settings(string name) {
      this.document = new XmlDocument();
      this.document.AppendChild(this.document.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\""));
      this.root = this.document.CreateElement(name);
      this.document.AppendChild(this.root);
    }

    public Settings(XmlDocument document) {
      this.document = document;
      this.root = document.DocumentElement;
    }

    public static Settings FromFile(string filePath) {
      var document = new XmlDocument();
      document.Load(filePath);
      return new Settings(document);
    }

    public void SaveToFile(string filePath) {
      this.document.Save(filePath);
    }
  }
}
