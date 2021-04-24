﻿using DoomLauncher.DataSources;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WadReader;

namespace DoomLauncher
{
    public static class Util
    {
        public static IEnumerable<object> TableToStructure(DataTable dt, Type type)
        {
            List<object> ret = new List<object>();
            object convertedObj;
            PropertyInfo[] properties = type.GetProperties().Where(x => x.GetSetMethod() != null && x.GetGetMethod() != null).ToArray();

            foreach (DataRow dr in dt.Rows)
            {
                object obj = Activator.CreateInstance(type);

                foreach (PropertyInfo pi in properties)
                {
                    Type pType = pi.PropertyType;

                    if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        pType = pType.GetGenericArguments()[0];

                    if (dt.Columns.Contains(pi.Name) && ChangeType(dr[pi.Name].ToString(), pType, out convertedObj))
                        pi.SetValue(obj, convertedObj, null);
                }

                ret.Add(obj);
            }

            return ret;
        }

        public static bool ChangeType(string obj, Type t, out object convertedObj)
        {
            convertedObj = null;
            if (obj == null) return false;

            if (obj.GetType() == typeof(string) && t == typeof(string))
            {
                convertedObj = obj;
                return true;
            }
            else if (obj.GetType() == typeof(string) && t == typeof(bool) &&
                (obj == "0" || obj == "1"))
            {
                if (obj == "0")
                    convertedObj = false;
                else
                    convertedObj = true;
                return true;
            }
            else if (t.BaseType == typeof(Enum))
            {
                convertedObj = Convert.ToInt32(obj);
                return true;
            }

            MethodInfo method = t.GetMethod("TryParse", new[] { typeof(string), Type.GetType(string.Format("{0}&", t.FullName)) });

            if (method != null)
            {
                object[] args = new object[] { obj, convertedObj };

                if ((bool)method.Invoke(null, args))
                {
                    convertedObj = args[1];
                    return true;
                }
            }

            return false;
        }

        public static string GetMapStringFromWad(string file)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WadFileReader wadReader = new WadFileReader(fs);

                if (wadReader.WadType != WadType.Unknown)
                {
                    var mapLumps = WadFileReader.GetMapMarkerLumps(wadReader.ReadLumps()).OrderBy(x => x.Name).ToArray();
                    fs.Close();

                    sb.Append(string.Join(", ", mapLumps.Select(x => x.Name)));
                }
                else
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                DisplayUnexpectedException(null, ex);
            }

            return sb.ToString();
        }

        public static void DisplayUnexpectedException(Form form, Exception ex)
        {
#if DEBUG
            throw ex;
#else
            if (form.InvokeRequired)
                form.Invoke(new Action<Form, Exception>(DisplayUnexpectedException), form, ex);
            else
                DisplayException(form, ex);
#endif
        }

        private static void DisplayException(Form form, Exception ex)
        {
            if (form != null && form.InvokeRequired)
            {
                form.Invoke(new Action<Form, Exception>(DisplayException), new object[] { form, ex });
            }
            else
            {
                TextBoxForm txt = new TextBoxForm();
                txt.Text = "Unexpected Error";
                txt.HeaderText = "An unexpected error occurred. Please submit the error report by clicking the link below. The report has been copied to your clipboard." + Environment.NewLine;
                txt.DisplayText = ex.ToString();
                txt.SetLink("Click here to submit", GitHubRepository);
                Clipboard.SetText(txt.DisplayText);

                if (form == null)
                {
                    txt.ShowDialog();
                }
                else
                {
                    txt.StartPosition = FormStartPosition.CenterParent;
                    txt.ShowDialog(form);
                }
            }
        }

        public static string GitHubRepository => $"https://github.com/{GitHubUser}/{GitHubRepositoryName}";

        public static string GitHubUser => "nstlaurent";

        public static string GitHubRepositoryName => "DoomLauncher";

        public static string DoomworldThread => "http://www.doomworld.com/vb/doom-general/69346-doom-launcher-doom-frontend-database/";

        public static string Realm667Thread => "http://realm667.com/index.php/en/kunena/doom-launcher";

        public static void SetDefaultSearchFields(SearchControl ctrlSearch)
        {
            string[] filters = new string[]
            {
                "Title",
                "Author",
                "Filename",
                "Description",
            };

            ctrlSearch.SetSearchFilters(filters);
            ctrlSearch.SetSearchFilter(filters[0], true);
            ctrlSearch.SetSearchFilter(filters[1], true);
            ctrlSearch.SetSearchFilter(filters[2], true);
        }

        public static GameFileSearchField[] SearchFieldsFromSearchCtrl(SearchControl ctrlSearch)
        {
            string[] items = ctrlSearch.GetSelectedSearchFilters();
            List<GameFileSearchField> ret = new List<GameFileSearchField>();
            GameFileFieldType type;

            foreach (string item in items)
            {
                if (Enum.TryParse(item, out type))
                {
                    ret.Add(new GameFileSearchField(type, GameFileSearchOp.Like, ctrlSearch.SearchText));
                }
            }

            return ret.ToArray();
        }

        public static List<ISourcePortData> GetSourcePortsData(IDataSourceAdapter adapter)
        {
            List<ISourcePortData> sourcePorts = adapter.GetSourcePorts().ToList();
            SourcePortData noPort = new SourcePortData();
            noPort.Name = "N/A";
            noPort.SourcePortID = -1;
            sourcePorts.Insert(0, noPort);
            return sourcePorts;
        }

        public static string[] GetSkills()
        {
            return new string[] { "1", "2", "3", "4", "5" };
        }

        public static string GetTimePlayedString(int minutes)
        {
            List<string> items = new List<string>();

            TimeSpan ts = new TimeSpan(0, minutes, 0);

            if (ts.Days > 0)
                items.Add(TimeString(ts.Days, "Day"));
            if (ts.Hours > 0)
                items.Add(TimeString(ts.Hours, "Hour"));

            items.Add(TimeString(ts.Minutes, "Minute"));

            return string.Join(", ", items.ToArray());
        }

        private static string TimeString(int time, string type)
        {
            return string.Concat(time.ToString(), " ",  type, time == 1 ? string.Empty : "s");
        }

        public static List<IGameFile> GetAdditionalFiles(IDataSourceAdapter adapter, IGameProfile gameFile)
        {
            if (gameFile != null && !string.IsNullOrEmpty(gameFile.SettingsFiles))
                return GetAdditionalFiles(adapter, gameFile.SettingsFiles);

            return new List<IGameFile>();
        }

        public static List<IGameFile> GetIWadAdditionalFiles(IDataSourceAdapter adapter, IGameProfile gameFile)
        {
            if (gameFile != null && !string.IsNullOrEmpty(gameFile.SettingsFilesIWAD))
                return GetAdditionalFiles(adapter, gameFile.SettingsFilesIWAD);

            return new List<IGameFile>();
        }

        public static List<IGameFile> GetSourcePortAdditionalFiles(IDataSourceAdapter adapter, IGameProfile gameFile)
        {
            if (gameFile != null && !string.IsNullOrEmpty(gameFile.SettingsFilesSourcePort))
                return GetAdditionalFiles(adapter, gameFile.SettingsFilesSourcePort);

            return new List<IGameFile>();
        }

        public static List<IGameFile> GetAdditionalFiles(IDataSourceAdapter adapter, ISourcePortData sourcePort)
        {
            return GetAdditionalFiles(adapter, sourcePort.SettingsFiles);
        }

        private static List<IGameFile> GetAdditionalFiles(IDataSourceAdapter adapter, string property)
        {
            string[] fileNames = Util.SplitString(property);
            List<IGameFile> gameFiles = new List<IGameFile>();
            Array.ForEach(fileNames, x => gameFiles.Add(adapter.GetGameFile(x)));
            return gameFiles.Where(x => x != null).ToList();
        }

        [Conditional("DEBUG")]
        public static void ThrowDebugException(string msg)
        {
            throw new Exception(msg);
        }

        public static IEnumerable<IArchiveEntry> GetEntriesByExtension(IArchiveReader reader, string[] extensions)
        {
            List<IArchiveEntry> entries = new List<IArchiveEntry>();

            foreach (var ext in extensions)
            {
                 entries.AddRange(reader.Entries
                     .Where(x => x.Name.Contains('.') && Path.GetExtension(x.Name).Equals(ext, StringComparison.OrdinalIgnoreCase)));
            }

            return entries;
        }

        public static string[] GetPkExtenstions()
        {
            return new string[] { ".pk3", ".ipk3", ".pk7", ".zip" };
        }

        public static string[] GetReadablePkExtensions()
        {
            return new string[] { ".pk3", ".ipk3", ".pke", ".zip" };
        }

        public static string[] GetDehackedExtensions()
        {
            return new string[] { ".deh", ".bex" };
        }

        public static string[] GetSourcePortPkExtensions()
        {
            return new string[] { ".pk3", ".ipk3", ".pk7", ".pke"};
        }

        public static GameFileFieldType[] DefaultGameFileUpdateFields
        {
            get
            {
                return new GameFileFieldType[]
                {
                    GameFileFieldType.Author,
                    GameFileFieldType.Title,
                    GameFileFieldType.Description,
                    GameFileFieldType.Downloaded,
                    GameFileFieldType.LastPlayed,
                    GameFileFieldType.ReleaseDate,
                    GameFileFieldType.Comments,
                    GameFileFieldType.Rating,
                    GameFileFieldType.Map,
                    GameFileFieldType.MapCount,
                };
            }
        }

        //Takes a file 'MAP01.wad' and makes it 'MAP01_GUID.wad'.
        //Checks if file with prefix MAP01 exists with same file length and returns that file (same file).
        //Otherwise a new file is extracted and returned.
        public static string ExtractTempFile(string tempDirectory, IArchiveEntry entry)
        {
            // The file is a regular file and not an archive - return the FulName
            if (!entry.ExtractRequired)
                return entry.FullName;

            string ext = Path.GetExtension(entry.Name);
            string file = entry.Name.Replace(ext, string.Empty) + "_";
            string[] searchFiles = Directory.GetFiles(tempDirectory, file + "*");

            string matchingFile = searchFiles.FirstOrDefault(x => new FileInfo(x).Length == entry.Length);

            if (matchingFile == null)
            {
                string extractFile = Path.Combine(tempDirectory, string.Concat(file, Guid.NewGuid().ToString(), ext));
                entry.ExtractToFile(extractFile);
                return extractFile;
            }

            return matchingFile;
        }

        public static List<IIWadData> GetIWadsDataSource(IDataSourceAdapter adapter)
        {
            List<IIWadData> iwads = adapter.GetIWads().ToList();
            iwads.ForEach(x => x.FileName = Path.GetFileNameWithoutExtension(x.FileName));
            return iwads;
        }

        public static string CleanDescription(string description)
        {
            string[] items = description.Split(new char[] { '\n' });
            StringBuilder sb = new StringBuilder();

            foreach (string item in items)
            {
                string text = Regex.Replace(item, @"\s+", " ");
                if (text.StartsWith(" "))
                    text = text.Substring(1);
                sb.Append(text);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        //returns the first position after the magicID is found, else returns -1
        public static long ReadAfter(MemoryStream ms, byte[] magicID)
        {
            long position = ms.Position;
            byte[] check = new byte[magicID.Length];

            while (ms.Position + magicID.Length < ms.Length)
            {
                ms.Read(check, 0, check.Length);

                if (magicID.SequenceEqual(check))
                    return ms.Position;

                ms.Position = ++position;
            }

            return -1;
        }

        public static int GetPreviewScreenshotWidth(int value)
        {
            if (value > 0)
                return 200 + (40 * value);
            else
                return 200 + (10 * value);
        }

        public static string[] SplitString(string value)
        {
            if (!string.IsNullOrEmpty(value))
                return value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
                return new string[] { };
        }

        public static string GetExecutableNoPath() => AppDomain.CurrentDomain.FriendlyName;

        public static string GetClippedEllipsesText(Graphics g, Font f, string text, SizeF layout)
        {
            int charactersFitted, linesFilled;
            g.MeasureString(text, f, layout, StringFormat.GenericDefault, out charactersFitted, out linesFilled);

            if (charactersFitted != text.Length && charactersFitted > 3)
                return text.Substring(0, charactersFitted - 3) + "...";

            return text.Substring(0, charactersFitted);
        }

        static public SizeF MeasureDisplayString(this Graphics graphics, string text, Font font)
        {
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };

            format.SetMeasurableCharacterRanges(ranges);

            Region[] regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);
            rect.Inflate(2, 2);

            return rect.Size;
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(WinPoint Point);

        [StructLayout(LayoutKind.Sequential)]
        public struct WinPoint
        {
            public int X;
            public int Y;

            public WinPoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public static bool IsVisibleAtPoint(this Control control, Point windowPoint)
        {
            var hwnd = WindowFromPoint(new WinPoint(windowPoint.X, windowPoint.Y));
            var other = Control.FromChildHandle(hwnd);
            if (other == null)
                return false;

            if (control == other || control.Contains(other))
                return true;

            return false;
        }

        public static Image FixedSize(Image imgPhoto, int width, int height, Color backColor)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent;
            float nPercentW = width / (float)sourceWidth;
            float nPercentH = height / (float)sourceHeight;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(backColor);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image RotateImage(Image image, float angle)
        {
            Bitmap bm = image as Bitmap;

            Matrix matrixOrigin = new Matrix();
            matrixOrigin.Rotate(angle);

            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(bm.Width, 0),
                new PointF(bm.Width, bm.Height),
                new PointF(0, bm.Height),
            };
            matrixOrigin.TransformPoints(points);
            GetPointBounds(points, out float xMin, out float xMax,
                out float yMin, out float yMax);

            int width = (int)Math.Round(xMax - xMin);
            int height = (int)Math.Round(yMax - yMin);
            Bitmap result = new Bitmap(width, height);

            Matrix matrixCenter = new Matrix();
            matrixCenter.RotateAt(angle, new PointF(width / 2f, height / 2f));

            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = InterpolationMode.High;
                gr.Clear(bm.GetPixel(0, 0));
                gr.Transform = matrixCenter;

                int x = (width - bm.Width) / 2;
                int y = (height - bm.Height) / 2;
                gr.DrawImage(bm, x, y);
            }

            return result;
        }

        private static void GetPointBounds(PointF[] points,
            out float xmin, out float xmax,
            out float ymin, out float ymax)
        {
            xmin = points[0].X;
            xmax = xmin;
            ymin = points[0].Y;
            ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }
        }
    }
}
