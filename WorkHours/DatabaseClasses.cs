using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WorkHours
{
    /// <summary>
    /// Defines an object with a string ID.
    /// </summary>
    public abstract class ObjectWithID
    {
        public string ID { get; private set; }

        public ObjectWithID(string id)
        {
            this.ID = id;
        }

        public XmlNode ToXml(XmlDocument doc, string name)
        {
            XmlNode node = doc.CreateElement(name);
            node.AddAttribute(doc, "ID", this.ID );
            return node;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ObjectWithID))
                return false;
            return this.ID.Equals((obj as ObjectWithID).ID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.ID;
        }
    }

    public class WorkDay
    {
        public DateTime Date { get; private set; }
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }

        public WorkDay()
        {

        }
    }
}
