using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBALegende.Models
{
    public class NBALegend
    {
        public bool IsSelected { get; set; }

        public int JerseyNumber { get; set; }      // broj dresa - brojcano polje
        public string FullName { get; set; }       // tekstualno polje
        public string ImagePath { get; set; }      // putanja do slike
        public string RtfPath { get; set; }        // putanja do rtf fajla
        public DateTime DateAdded { get; set; }    // datum dodavanja

        public string ResolvedImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return "";
                }

                if (System.IO.Path.IsPathRooted(ImagePath))
                {
                    return ImagePath;
                }

                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);
            }
        }
    }
}
