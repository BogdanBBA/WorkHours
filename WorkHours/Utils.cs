using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WorkHours.VisualComponents;

namespace WorkHours
{
    /// <summary>
    /// A pair of values (a more convenient form of KeyValuePair).
    /// </summary>
    /// <typeparam name="T">the data type of the values (unrestricted)</typeparam>
    public class Pair<T>
    {
        private KeyValuePair<T, T> pair;

        public Pair(T normal, T highlighted)
        {
            this.pair = new KeyValuePair<T, T>(normal, highlighted);
        }

        public T Normal
        {
            get { return this.pair.Key; }
            set { this.pair = new KeyValuePair<T, T>(value, this.pair.Value); }
        }

        public T Highlighted
        {
            get { return this.pair.Value; }
            set { this.pair = new KeyValuePair<T, T>(this.pair.Key, value); }
        }

        public T this[bool highlighted]
        { get { return this.GetValue(highlighted); } }

        public T GetValue(bool highlighted)
        {
            return highlighted ? this.Highlighted : this.Normal;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", this.Normal.ToString(), this.Highlighted.ToString());
        }
    }
    /// <summary>
    /// A pair of values (a more convenient form of KeyValuePair) identical to class Pair, except more football-match relevant way.
    /// </summary>
    /// <typeparam name="T">the data type of the values (still unrestricted)</typeparam>
    public class PairT<T>
    {
        private KeyValuePair<T, T> pair;

        public PairT(T home, T away)
        {
            this.pair = new KeyValuePair<T, T>(home, away);
        }

        public T Home
        {
            get { return this.pair.Key; }
            set { this.pair = new KeyValuePair<T, T>(value, this.pair.Value); }
        }

        public T Away
        {
            get { return this.pair.Value; }
            set { this.pair = new KeyValuePair<T, T>(this.pair.Key, value); }
        }

        public T this[bool home]
        { get { return this.GetValue(home); } }

        public T GetValue(bool home)
        {
            return home ? this.Home : this.Away;
        }
    }

    /// <summary>
    /// Defines a list of objects that are descended from ObjectWithID, containing only unique IDs.
    /// </summary>
    public class ListOfIDObjects<TYPE> : List<TYPE> where TYPE : ObjectWithID
    {
        public new void Add(TYPE item)
        {
            if (this.GetIndexOfItemByID(item.ID) == -1)
                base.Add(item);
        }

        public new void AddRange(IEnumerable<TYPE> items)
        {
            foreach (TYPE item in items)
                this.Add(item);
        }

        public int GetIndexOfItemByID(string id)
        {
            for (int iItem = 0; iItem < this.Count; iItem++)
                if (this[iItem].ID.Equals(id))
                    return iItem;
            return -1;
        }

        public int GetIndexOfItem(TYPE item)
        {
            return this.GetIndexOfItemByID(item.ID);
        }

        public TYPE GetItemByID(string id)
        {
            int index = this.GetIndexOfItemByID(id);
            return index != -1 ? this[index] : null;
        }

        public TYPE GetItem(TYPE item)
        {
            return item != null ? this.GetItemByID(item.ID) : null;
        }

        public ListOfIDObjects<TYPE> GetDeepCopy()
        {
            ListOfIDObjects<TYPE> result = new ListOfIDObjects<TYPE>();
            foreach (TYPE item in this)
                result.Add(item);
            return result;
        }

        public void SwapItemsAtPositions(int posA, int posB)
        {
            if (posA >= 0 && posA < this.Count && posB >= 0 && posB < this.Count)
            {
                TYPE aux = this[posA];
                this[posA] = this[posB];
                this[posB] = aux;
            }
        }
    }

    /// <summary>
    /// Utility functions and extension methods.
    /// </summary>
    public static class Utils
    {
        internal const string NullString = "null";
        internal const string DefaultSeparator = ";";
        internal static readonly Random Random = new Random();

        public static bool IsNumber(string text)
        {
            double number;
            return double.TryParse(text, out number);
        }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, string value)
        {
            XmlAttribute attribute = doc.CreateAttribute(key);
            attribute.Value = value != null ? value : Utils.NullString;
            node.Attributes.Append(attribute);
        }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, int value)
        { node.AddAttribute(doc, key, value.ToString()); }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, long value)
        { node.AddAttribute(doc, key, value.ToString()); }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, DateTime value)
        { node.AddAttribute(doc, key, value.ToString("d'/'m'/'yyyy")); }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, TimeSpan value)
        { node.AddAttribute(doc, key, value.Hours.ToString().PadLeft(2, '0') + ":" + value.Minutes.ToString().PadLeft(2, '0')); }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, Pair<ColorResource> value)
        {
            node.AddAttribute(doc, "normal", ColorTranslator.ToHtml(value.Normal.Color));
            node.AddAttribute(doc, "highlighted", ColorTranslator.ToHtml(value.Highlighted.Color));
        }

        public static string DecodeNullableString(string text)
        { return text == null || text.Equals(Utils.NullString) ? null : text; }

        public static DateTime? DecodeNullableDateTime(string text)
        { return text.Equals(Utils.NullString) ? (DateTime?) null : DateTime.Parse(text); }

        public static ulong? DecodeNullableUnsignedLong(string text)
        { return text.Equals(Utils.NullString) ? (ulong?) null : ulong.Parse(text); }

        public static Point MinimumPointValues(Point a, Point b)
        { return new Point(a.X < b.X ? a.X : b.X, a.Y < b.Y ? a.Y : b.Y); }

        public static Point MaximumPointValues(Point a, Point b)
        { return new Point(a.X > b.X ? a.X : b.X, a.Y > b.Y ? a.Y : b.Y); }

        public static string Plural(string singularForm, long quantity, bool includeQuantity)
        {
            string form = quantity == 1 ? singularForm : singularForm + "s";
            return includeQuantity ? Utils.FormatNumber(quantity) + " " + form : form;
        }

        public static Size ScaleRectangle(int width, int height, int maxWidth, int maxHeight)
        {
            var ratioX = (double) maxWidth / width;
            var ratioY = (double) maxHeight / height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int) (width * ratio);
            var newHeight = (int) (height * ratio);

            return new Size(newWidth, newHeight);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight, InterpolationMode mode, bool disposeOldImage)
        {
            Size newSize = ScaleRectangle(image.Width, image.Height, maxWidth, maxHeight);
            Image newImage = new Bitmap(newSize.Width, newSize.Height);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = mode;
            g.DrawImage(image, 0, 0, newSize.Width, newSize.Height);
            if (disposeOldImage)
                image.Dispose();
            return newImage;
        }

        public static Image GetScaledImageOrScaledDefault(string imagePath, int maxWidth, int maxHeight, InterpolationMode mode, Image defaultImg)
        {
            try
            { return Utils.ScaleImage(new Bitmap(imagePath), maxWidth, maxHeight, mode, true); }
            catch (Exception)
            { return Utils.ScaleImage(defaultImg, maxWidth, maxHeight, mode, false); }
        }

        public static string FormatNumber(long number)
        {
            return number.ToString("#,##0");
        }

        public static string FormatDuration(TimeSpan duration)
        {
            return (int) duration.TotalMinutes + ":" + duration.Seconds.ToString("D2");
        }

        public static void ApplyAlphaMask(Bitmap bmp, Bitmap alphaMaskImage)
        {
            int width = bmp.Width;
            int height = bmp.Height;

            BitmapData dataAlphaMask = alphaMaskImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                try
                {
                    unsafe // using pointer requires the unsafe keyword
                    {
                        byte* pData0Mask = (byte*) dataAlphaMask.Scan0;
                        byte* pData0 = (byte*) data.Scan0;

                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                byte* pData = pData0 + (y * data.Stride) + (x * 4);
                                byte* pDataMask = pData0Mask + (y * dataAlphaMask.Stride) + (x * 4);

                                byte maskBlue = pDataMask[0];
                                byte maskGreen = pDataMask[1];
                                byte maskRed = pDataMask[2];

                                // the closer the color is to black the more opaque it will be.
                                byte alpha = (byte) (255 - (maskRed + maskBlue + maskGreen) / 3);

                                // respect the original alpha value
                                byte originalAlpha = pData[3];
                                pData[3] = (byte) (((float) (alpha * originalAlpha)) / 255f);
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
            finally
            {
                alphaMaskImage.UnlockBits(dataAlphaMask);
            }
        }

        public static Image ConvertToGrayscale(Image original)
        {
            //create a blank bitmap the same size as original and get a graphics object 
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][] {
                    new float[] { .3f, .3f, .3f, 0, 0 },
                    new float[] { .59f, .59f, .59f, 0, 0 },
                    new float[] { .11f, .11f, .11f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 } });

            //create some image attributes and set the color matrix attribute
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image using the grayscale color matrix
            g.DrawImage(original,
                new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height,
                GraphicsUnit.Pixel,
                attributes);

            //dispose the Graphics object and return the result
            g.Dispose();
            return newBitmap;
        }

        public static void CheckItemAndUncheckAllOthers<TYPE>(this ICollection<TYPE> controls, TYPE controlToCheck) where TYPE : MyAppSpecificBaseControl
        {
            foreach (TYPE control in controls)
                control.Checked = control.Equals(controlToCheck);
        }

        public static void SizeAndPositionControlsInPanel<TYPE>(System.Windows.Forms.Panel container, IList<TYPE> controls, bool horizontally, int padding) where TYPE : Control
        {
            int newControlSize = (int) (((horizontally ? container.Width : container.Height) - (controls.Count - 1) * padding) / (double) controls.Count);
            for (int index = 0, lastPos = 0; index < controls.Count; index++, lastPos += newControlSize + padding)
            {
                controls[index].Parent = container;
                if (horizontally)
                    controls[index].SetBounds(lastPos, 0, newControlSize, container.Height);
                else
                    controls[index].SetBounds(0, lastPos, container.Width, newControlSize);
            }
        }

        public static void SwapItemsAtPositions<TYPE>(this List<TYPE> list, int posA, int posB)
        {
            if (posA >= 0 && posA < list.Count && posB >= 0 && posB < list.Count)
            {
                TYPE aux = list[posA];
                list[posA] = list[posB];
                list[posB] = aux;
            }
        }

        public static string GetListString(this List<string> list, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in list)
                sb.Append(item).Append(separator);
            if (sb.Length > 0)
                sb = sb.Remove(sb.Length - separator.Length, separator.Length);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Paths
    {
        public static readonly string ProgramFilesFolder = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName + "/program-files/";

        public static readonly string DatabaseFile = ProgramFilesFolder + "database.xml";

        public static readonly string[] Folders = { ProgramFilesFolder };
        public static readonly string[] Files = { DatabaseFile };

        public static string CheckPaths(bool tryToCreateMissingFolders)
        {
            string phase = "initializing";
            try
            {
                phase = "checking folders";

                foreach (string folder in Folders)
                {
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    if (!Directory.Exists(folder))
                        throw new Exception("Folder '" + folder + "' does not exist!");
                }

                foreach (string file in Files)
                    if (!File.Exists(file))
                        throw new Exception("File '" + file + "' does not exist!");

                return "";
            }
            catch (Exception E)
            {
                return "ERROR: Paths.CheckPaths(), phase '" + phase + "'\n\n" + E.ToString();
            }
        }
    }
}