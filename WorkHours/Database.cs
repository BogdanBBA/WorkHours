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
        public Team FavoriteTeam { get; set; }
        public bool ShowCountryNamesInNativeLanguage { get; set; }
        public bool ShowKnockoutStageOnStartup { get; set; }

        public string ReadSettings(XmlNode settingsNode, List<Team> teams)
        {
            try
            {
                try { this.FavoriteTeam = teams.First(t => t.Country.ID.Equals(settingsNode.SelectSingleNode("FavoriteTeam").Attributes["value"].Value)); }
                catch (Exception) { this.FavoriteTeam = teams.First(); }

                try { this.ShowCountryNamesInNativeLanguage = bool.Parse(settingsNode.SelectSingleNode("ShowCountryNamesInNativeLanguage").Attributes["value"].Value); }
                catch (Exception) { this.ShowCountryNamesInNativeLanguage = false; }

                try { this.ShowKnockoutStageOnStartup = bool.Parse(settingsNode.SelectSingleNode("ShowKnockoutStageOnStartup").Attributes["value"].Value); }
                catch (Exception) { this.ShowKnockoutStageOnStartup = true; }

                return "";
            }
            catch (Exception E) { return E.ToString(); }
        }

        public string ToXml(XmlDocument doc, string nodeName, out XmlNode resultNode)
        {
            try
            {
                XmlNode result = doc.CreateElement(nodeName), node;

                node = result.AppendChild(doc.CreateElement("FavoriteTeam"));
                node.AddAttribute(doc, "value", this.FavoriteTeam.Country.ID);

                node = result.AppendChild(doc.CreateElement("ShowCountryNamesInNativeLanguage"));
                node.AddAttribute(doc, "value", this.ShowCountryNamesInNativeLanguage.ToString());

                node = result.AppendChild(doc.CreateElement("ShowKnockoutStageOnStartup"));
                node.AddAttribute(doc, "value", this.ShowKnockoutStageOnStartup.ToString());

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
        public const string ThirdPlacedTeamsGroupID = "T";

        public ListOfIDObjects<Venue> Venues { get; private set; }
        public ListOfIDObjects<Country> Countries { get; private set; }
        public List<Team> Teams { get; private set; }
        public ListOfIDObjects<Group> Groups { get; private set; }
        public Group ThirdPlacedTeams { get; private set; }
        public ListOfIDObjects<Match> Matches { get; private set; }
        public Settings Settings { get; private set; }

        /// <summary>Constructs an empty Database object.</summary>
        public Database()
        {
            this.Venues = new ListOfIDObjects<Venue>();
            this.Countries = new ListOfIDObjects<Country>();
            this.Teams = new List<Team>();
            this.Groups = new ListOfIDObjects<Group>();
            this.Matches = new ListOfIDObjects<Match>();
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

                XmlNodeList nodes = doc.SelectNodes("DATABASE/VENUES/Venue");
                foreach (XmlNode node in nodes)
                    this.Venues.Add(Venue.Parse(node));

                nodes = doc.SelectNodes("DATABASE/COUNTRIES/Country");
                foreach (XmlNode node in nodes)
                    this.Countries.Add(Country.Parse(node));

                nodes = doc.SelectNodes("DATABASE/TEAMS/Team");
                foreach (XmlNode node in nodes)
                    this.Teams.Add(Team.Parse(node, this.Countries));

                nodes = doc.SelectNodes("DATABASE/GROUPS/Group");
                foreach (XmlNode node in nodes)
                    this.Groups.Add(Group.Parse(node, this.Teams));

                this.ThirdPlacedTeams = new Group(Database.ThirdPlacedTeamsGroupID, "Third place", new List<TableLine>());
                this.Groups.Add(this.ThirdPlacedTeams);

                nodes = doc.SelectNodes("DATABASE/MATCHES/Match");
                foreach (XmlNode node in nodes)
                    this.Matches.Add(Match.Parse(node, this));

                this.Settings.ReadSettings(doc.SelectSingleNode("DATABASE/SETTINGS"), this.Teams);

                this.CalculateGroupMatchTeams();
                this.CalculateGroups();
                this.CalculateKnockoutMatches();

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

                XmlNode node = root.AppendChild(doc.CreateElement("VENUES"));
                foreach (Venue venue in this.Venues)
                    node.AppendChild(venue.ToXml(doc, "Venue"));

                node = root.AppendChild(doc.CreateElement("COUNTRIES"));
                foreach (Country country in this.Countries)
                    node.AppendChild(country.ToXml(doc, "Country"));

                node = root.AppendChild(doc.CreateElement("TEAMS"));
                foreach (Team team in this.Teams)
                    node.AppendChild(team.ToXml(doc, "Team"));

                node = root.AppendChild(doc.CreateElement("GROUPS"));
                foreach (Group group in this.Groups)
                    if (!group.ID.Equals(Database.ThirdPlacedTeamsGroupID))
                        node.AppendChild(group.ToXml(doc, "Group"));

                node = root.AppendChild(doc.CreateElement("MATCHES"));
                foreach (Match match in this.Matches)
                    node.AppendChild(match.ToXml(doc, "Match"));

                XmlNode settingsNode;
                this.Settings.ToXml(doc, "SETTINGS", out settingsNode);
                root.AppendChild(settingsNode);

                doc.Save(filepath);
                return "";
            }
            catch (Exception E)
            { return E.ToString(); }
        }

        public void Calculate(bool matchTeams, bool groups, bool knockoutMatches)
        {
            if (matchTeams)
                this.CalculateGroupMatchTeams();
            if (groups)
                this.CalculateGroups();
            if (knockoutMatches)
                this.CalculateKnockoutMatches();
        }

        public void CalculateGroupMatchTeams()
        {
            foreach (Match match in this.Matches)
                if (match.IsGroupMatch)
                {
                    match.Teams.Home = this.Teams.First(t => t.Country.ID.Equals(match.TeamReferences.Home));
                    match.Teams.Away = this.Teams.First(t => t.Country.ID.Equals(match.TeamReferences.Away));
                }
        }

        public void CalculateGroups()
        {
            this.ThirdPlacedTeams.TableLines.Clear();
            foreach (Group group in this.Groups)
                if (!group.ID.Equals(Database.ThirdPlacedTeamsGroupID))
                {
                    foreach (Match match in this.Matches.GetMatchesBy(group))
                        foreach (TableLine line in group.TableLines)
                            line.AddMatchResult(match);
                    group.SortTableLines(false, this.Matches);
                    this.ThirdPlacedTeams.TableLines.Add(new TableLine(group.TableLines[2]));
                }
            this.ThirdPlacedTeams.SortTableLines(true, this.Matches);
        }

        private Team CalculateTeam(string reference) // format: "B:1" or "T:A/C/D"
        {
            if (reference.Contains(':')) // table line reference
            {
                string refGroup = reference.Split(':')[0], refTeam = reference.Split(':')[1];

                if (!refGroup.Equals(Database.ThirdPlacedTeamsGroupID)) // normal group
                {
                    Group group = this.Groups.GetItemByID(refGroup);
                    if (group != null && group.AllMatchesPlayed)
                        return group.TableLines[Int32.Parse(refTeam) - 1].Team;
                    return null;
                }
                else // third-place group (also, certainly an eight-final)
                {
                    Group tempGroup = new Group("temp", refTeam, new List<TableLine>());
                    foreach (string groupID in refTeam.Split('/'))
                    {
                        Group group = this.Groups.GetItemByID(groupID);
                        if (!group.AllMatchesPlayed)
                            return null;
                        tempGroup.TableLines.Add(new TableLine(group.TableLines[2]));
                    }
                    tempGroup.SortTableLines(true, this.Matches);
                    for (int index = 0; index < tempGroup.TableLines.Count; index++)
                        if (this.Matches.GetMatchesBy("KO:8").GetMatchesBy(tempGroup.TableLines[index].Team).Count == 0)
                            return tempGroup.TableLines[index].Team;
                    return null;
                }
            }
            else // match winner reference
            {
                Match match = this.Matches.GetItemByID(reference);
                if (match.Teams.Home != null & match.Teams.Away != null && match.Scoreboard.Played)
                    return match.Scoreboard.FinalScore.HomeWin ? match.Teams.Home : match.Teams.Away;
                return null;
            }
        }

        public void CalculateKnockoutMatches()
        {
            ListOfIDObjects<Match> matches = this.Matches.GetMatchesBy("KO");
            foreach (Match match in matches)
            {
                match.Teams.Home = this.CalculateTeam(match.TeamReferences.Home);
                match.Teams.Away = this.CalculateTeam(match.TeamReferences.Away);
            }
        }
    }
}
