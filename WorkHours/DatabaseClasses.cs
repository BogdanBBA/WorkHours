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

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ObjectWithID))
                return false;
            return this.GetHashCode() == (obj as ObjectWithID).GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public XmlNode ToXml(XmlDocument doc, string name)
        {
            XmlNode node = doc.CreateElement(name);
            node.AddAttribute(doc, "ID", this.ID);
            return node;
        }
    }

    public class WorkData
    {
        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }
        public TimeSpan Break { get; private set; }
        public TimeSpan Interruption { get; private set; }

        public TimeSpan TotalInterval { get; private set; }
        public TimeSpan TotalWork { get; private set; }
        public TimeSpan Flextime { get; private set; }
        public TimeSpan OverallFlextime { get; private set; }

        public WorkData()
            : this(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero)
        {
        }

        public WorkData(TimeSpan start, TimeSpan end, TimeSpan breakTime, TimeSpan interruption)
        {
            this.Start = start;
            this.End = end;
            this.Break = breakTime;
            this.Interruption = interruption;
        }

        public void CalculateValues()
        {
            this.TotalInterval = this.End.Subtract(this.Start);
            this.TotalWork = this.TotalInterval.Subtract(this.Break).Subtract(this.Interruption);
            this.Flextime = this.TotalInterval.Subtract(this.TotalWork);
        }

        public XmlNode ToXml(XmlDocument doc, string name)
        {
            XmlNode node = doc.CreateElement(name);
            node.AddAttribute(doc, "start", this.Start);
            node.AddAttribute(doc, "end", this.End);
            node.AddAttribute(doc, "break", this.Break);
            node.AddAttribute(doc, "interruption", this.Interruption);
            return node;
        }

        public static WorkData Parse(XmlNode node)
        {
            TimeSpan start = TimeSpan.Parse(node.Attributes["start"].Value);
            TimeSpan end = TimeSpan.Parse(node.Attributes["end"].Value);
            TimeSpan breakTime = TimeSpan.Parse(node.Attributes["break"].Value);
            TimeSpan interruption = TimeSpan.Parse(node.Attributes["interruption"].Value);
            return new WorkData(start, end, breakTime, interruption);
        }
    }

    public class WorkDay : WorkData
    {
        public DateTime Date { get; private set; }

        public WorkDay(DateTime date, TimeSpan start, TimeSpan end, TimeSpan breakTime, TimeSpan interruption)
            : base(start, end, breakTime, interruption)
        {
            this.Date = date;
        }

        public new XmlNode ToXml(XmlDocument doc, string name)
        {
            XmlNode node = base.ToXml(doc, name);
            node.AddAttribute(doc, "date", this.Date.ToString("d'/'M'/'yyyy"));
            XmlAttribute attr = node.Attributes[node.Attributes.Count - 1];
            node.Attributes.Remove(attr);
            node.Attributes.Prepend(attr);
            return node;
        }

        public static new WorkDay Parse(XmlNode node)
        {
            string[] dateParts = node.Attributes["date"].Value.Split('/');
            DateTime date = new DateTime(Int32.Parse(dateParts[2]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[0]));
            WorkData data = WorkData.Parse(node);
            return new WorkDay(date, data.Start, data.End, data.Break, data.Interruption);
        }
    }
}
