using System.Windows.Input;
using System.Reflection;
using System.IO;
using System.Linq;

namespace Niero.SupportClasses
{
    public static class CustomCursors
    {
        public static Cursor Busy
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Busy.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Normal_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Normal Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Help_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Help Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Working_in_Background
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Working in Background.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Text_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Text Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Unavailable
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Unavailable.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Link_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Link Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Precision
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Precision.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Vertical_Resize
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Vertical Resize.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Horizontal_Resize
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Horizontal Resize.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Diagonal_Resize_1
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Diagonal Resize 1.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Diagonal_Resize_2
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Diagonal Resize 2.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Move
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Move.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Alternate_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Alternate Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Handwriting
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Handwriting.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Location_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Location Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }

        public static Cursor Person_Select
        {
            get
            {
                string resourcePath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(str => str.EndsWith("Person Select.ani"));
                Stream cursStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                return new Cursor(cursStream);
            }
        }
    }
}
