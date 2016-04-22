using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WorkHours
{
    /// <summary>
    /// Contains settings regarding map generation. Please construct instances of this class using instance initializers.
    /// </summary>
    public class Settings
    {
        public bool TestThingy { get; set; }

        public string ReadSettings(XmlNode settingsNode)
        {
            try
            {
                try { this.TestThingy = bool.Parse(settingsNode.SelectSingleNode("TestThingy").Attributes["value"].Value); }
                catch (Exception) { this.TestThingy = true; }

                return "";
            }
            catch (Exception E) { return E.ToString(); }
        }

        public string ToXml(XmlDocument doc, string nodeName, out XmlNode resultNode)
        {
            try
            {
                XmlNode result = doc.CreateElement(nodeName), node;

                node = result.AppendChild(doc.CreateElement("TestThingy"));
                node.AddAttribute(doc, "value", this.TestThingy.ToString());

                resultNode = result;
                return "";
            }
            catch (Exception E) { resultNode = null; return E.ToString(); }
        }
    }

    /// <summary>
    /// Defines the top-level data structures for the administrative unit- or data container-related information.
    /// </summary>
    public class Database
    {
        public List<WorkDay> WorkDays { get; private set; }
        public Settings Settings { get; private set; }

        /// <summary>Constructs an empty Database object.</summary>
        public Database()
        {
            this.WorkDays = new List<WorkDay>();
            this.Settings = new Settings();
        }

        /// <summary>Loads a Database object using data from the database file.</summary>
        /// <param name="databaseFile">the path to the database file</param>
        public string LoadDatabase(string databaseFile)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(databaseFile);

                XmlNodeList nodes = doc.SelectNodes("DATABASE/WORK_DAYS/WorkDay");
                foreach (XmlNode node in nodes)
                    this.WorkDays.Add(WorkDay.Parse(node));

                this.Settings.ReadSettings(doc.SelectSingleNode("DATABASE/SETTINGS"));

                return "";
            }
            catch (Exception E)
            { return E.ToString(); }
        }

        /// <summary>
        /// Saves database to file.
        /// </summary>
        public string SaveDatabase(string filepath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode root = doc.AppendChild(doc.CreateElement("DATABASE"));
                root.AddAttribute(doc, "lastSaved", DateTime.Now.ToString("dddd, d MMMM yyyy, HH:mm:ss"));

                XmlNode node = root.AppendChild(doc.CreateElement("WORK_DAYS"));
                foreach (WorkDay day in this.WorkDays)
                    node.AppendChild(day.ToXml(doc, "WorkDay"));

                XmlNode settingsNode;
                this.Settings.ToXml(doc, "SETTINGS", out settingsNode);
                root.AppendChild(settingsNode);

                doc.Save(filepath);
                return "";
            }
            catch (Exception E)
            { return E.ToString(); }
        }
    }
}
