using NBALegende.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace NBALegende
{
    public class LegendDataIO
    {
        private string legendsFilePath = "legends.xml";

        public ObservableCollection<NBALegend> LoadLegends()
        {
            if (!File.Exists(legendsFilePath))
            {
                ObservableCollection<NBALegend> defaultLegends = CreateDefaultLegends();
                SaveLegends(defaultLegends);
                return defaultLegends;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<NBALegend>));

            using (FileStream fileStream = new FileStream(legendsFilePath, FileMode.Open))
            {
                return (ObservableCollection<NBALegend>)serializer.Deserialize(fileStream);
            }
        }

        public void SaveLegends(ObservableCollection<NBALegend> legends)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<NBALegend>));

            using (FileStream fileStream = new FileStream(legendsFilePath, FileMode.Create))
            {
                serializer.Serialize(fileStream, legends);
            }
        }

        private ObservableCollection<NBALegend> CreateDefaultLegends()
        {
            ObservableCollection<NBALegend> legends = new ObservableCollection<NBALegend>();

            legends.Add(new NBALegend
            {
                IsSelected = false,
                JerseyNumber = 23,
                FullName = "Michael Jordan",
                ImagePath = "Images/nba-logo_2x.png",
                RtfPath = "Data/jordan.rtf",
                DateAdded = DateTime.Now
            });

            legends.Add(new NBALegend
            {
                IsSelected = false,
                JerseyNumber = 24,
                FullName = "Kobe Bryant",
                ImagePath = "Images/nba-logo_2x.png",
                RtfPath = "Data/kobe.rtf",
                DateAdded = DateTime.Now
            });

            legends.Add(new NBALegend
            {
                IsSelected = false,
                JerseyNumber = 23,
                FullName = "LeBron James",
                ImagePath = "Images/nba-logo_2x.png",
                RtfPath = "Data/lebron.rtf",
                DateAdded = DateTime.Now
            });

            return legends;
        }
    }
}