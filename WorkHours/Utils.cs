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

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, ulong? value)
        { node.AddAttribute(doc, key, value.HasValue ? value.Value.ToString() : null); }

        public static void AddAttribute(this XmlNode node, XmlDocument doc, string key, DateTime? value)
        { node.AddAttribute(doc, key, value.HasValue ? value.Value.ToString() : null); }

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

        public static void CheckItemAndUncheckAllOthers<TYPE>(this ICollection<TYPE> controls, TYPE controlToCheck) where TYPE : MyEuroBaseControl
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

        public static void SortMatchesChronologically(this ListOfIDObjects<Match> matches)
        {
            for (int iM = 0; iM < matches.Count - 1; iM++)
                for (int jm = iM + 1; jm < matches.Count; jm++)
                    if (matches[iM].When.CompareTo(matches[jm].When) > 0)
                        matches.SwapItemsAtPositions(iM, jm);
        }

        /// <summary>Searches the group matches of this list and finds the one played between the given teams, then returns the WhichTeamWon property value of the FinalScore of that match, 
        /// for the order in which the two team parameters were passed (-1 for team A, 0 for draw, 1 for team B). Note that if the match has not been played or cannot be found, the returned value will be -2.</summary>
        public static int WhoWonGroupMatchBetween(this List<Match> matches, Team teamA, Team teamB)
        {
            foreach (Match match in matches)
                if (match.IsGroupMatch)
                    if (match.Teams.Home.Equals(teamA) && match.Teams.Away.Equals(teamB))
                        return match.Scoreboard.Played ? match.Scoreboard.FinalScore.WhichTeamWon : -2;
                    else if (match.Teams.Home.Equals(teamB) && match.Teams.Away.Equals(teamA))
                        return match.Scoreboard.Played ? -match.Scoreboard.FinalScore.WhichTeamWon : -2;
            return -2;
        }

        public static MatchScoreboard GetRandomResult(bool canEndInDraw)
        {
            List<HalfScoreboard> halves = new List<HalfScoreboard>();
            halves.Add(new HalfScoreboard(Utils.Random.Next(5), Utils.Random.Next(5)));
            halves.Add(new HalfScoreboard(Utils.Random.Next(5), Utils.Random.Next(5)));
            MatchScoreboard result = new MatchScoreboard(halves);
            if (canEndInDraw || result.FinalScore.WhichTeamWon != 0)
                return result;
            halves.Add(new HalfScoreboard(Utils.Random.Next(3), Utils.Random.Next(3)));
            halves.Add(new HalfScoreboard(Utils.Random.Next(3), Utils.Random.Next(3)));
            result.SetHalves(halves);
            if (result.FinalScore.WhichTeamWon != 0)
                return result;
            int home = 0, away = 0;
            while (home == away)
            {
                home = Utils.Random.Next(6);
                away = Utils.Random.Next(6);
            }
            halves.Add(new HalfScoreboard(home, away));
            result.SetHalves(halves);
            return result;
        }

        /// <summary>Searches this list of matches and returns a sublist containing all items that are relevant to the given parameter. The parameter can be an instance of Venue, Team, Group, DateTime, string (category) or bool (played).</summary>
        public static ListOfIDObjects<Match> GetMatchesBy(this ListOfIDObjects<Match> matches, object whatever)
        {
            ListOfIDObjects<Match> result = new ListOfIDObjects<Match>();

            if (whatever is Venue)
            {
                Venue venue = whatever as Venue;
                foreach (Match match in matches)
                    if (match.Where.Equals(venue))
                        result.Add(match);
            }
            else if (whatever is Team)
            {
                Team team = whatever as Team;
                foreach (Match match in matches)
                    if ((match.Teams.Home != null && match.Teams.Home.Equals(team)) || (match.Teams.Away != null && match.Teams.Away.Equals(team)))
                        result.Add(match);
            }
            else if (whatever is Group)
            {
                Group group = whatever as Group;
                foreach (Match match in matches)
                    if (match.IsGroupMatch && match.Category.Split(':')[1].Equals(group.ID))
                        result.Add(match);
            }
            else if (whatever is DateTime)
            {
                DateTime date = (DateTime) whatever;
                foreach (Match match in matches)
                    if (match.When.Date.Equals(date.Date))
                        result.Add(match);
            }
            else if (whatever is string)
            {
                string category = whatever as string;
                foreach (Match match in matches)
                    if (match.Category.Contains(category))
                        result.Add(match);
            }
            else if (whatever is bool)
            {
                bool played = (bool) whatever;
                foreach (Match match in matches)
                    if (match.Scoreboard.Played == played)
                        result.Add(match);
            }

            return result;
        }
    }

    public static class StaticData
    {
        public static SortedDictionary<string, Bitmap> Images { get; private set; }
        public static PrivateFontCollection PVC { get; private set; }
        public static int FontExoLight_Index { get; private set; }
        public static int FontExo_Index { get; private set; }
        public static int FontExoBold_Index { get; private set; }

        static StaticData()
        {
            StaticData.Images = new SortedDictionary<string, Bitmap>();
        }

        public static string LoadData()
        {
            try
            {
                StaticData.Images.Add(Paths.LogoImageFile, new Bitmap(Paths.LogoImageFile));
                StaticData.Images.Add(Paths.UnknownTeamImageFile, new Bitmap(Paths.UnknownTeamImageFile));
                StaticData.Images.Add(Paths.KnockoutImageFile, new Bitmap(Paths.KnockoutImageFile));

                StaticData.PVC = new PrivateFontCollection();
                StaticData.PVC.AddFontFile(Paths.FontsFolder + "exo2-xlite.ttf");
                StaticData.FontExoLight_Index = 0;
                StaticData.PVC.AddFontFile(Paths.FontsFolder + "exo2.ttf");
                StaticData.FontExo_Index = 0;
                StaticData.PVC.AddFontFile(Paths.FontsFolder + "exo2-bold.ttf");
                StaticData.FontExoBold_Index = 0;

                return "";
            }
            catch (Exception E)
            {
                return "ERROR: StaticImages.LoadImages()\n\n" + E.ToString();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Paths
    {
        public static readonly string ProgramFilesFolder = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName + "/program-files/";
        public static readonly string ResourcesFolder = ProgramFilesFolder + "resources/";
        public static readonly string FontsFolder = ResourcesFolder + "fonts/";
        public static readonly string FlagsFolder = ProgramFilesFolder + "flags/";
        public static readonly string CitiesFolder = ProgramFilesFolder + "cities/";
        public static readonly string StadiumLocationsFolder = ProgramFilesFolder + "stadium-locations/";
        public static readonly string StadiumOutsidesFolder = ProgramFilesFolder + "stadiums-outside/";
        public static readonly string StadiumInsidesFolder = ProgramFilesFolder + "stadiums-inside/";

        public static readonly string DatabaseFile = ProgramFilesFolder + "database.xml";
        public static readonly string LogoImageFile = ResourcesFolder + "logo.png";
        public static readonly string UnknownTeamImageFile = ResourcesFolder + "unknownTeam.png";
        public static readonly string KnockoutImageFile = ResourcesFolder + "knockout.png";

        public static readonly string[] Folders = { ProgramFilesFolder, ResourcesFolder, FlagsFolder, CitiesFolder, StadiumLocationsFolder, StadiumOutsidesFolder, StadiumInsidesFolder };
        public static readonly string[] Files = { DatabaseFile, LogoImageFile, UnknownTeamImageFile };

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