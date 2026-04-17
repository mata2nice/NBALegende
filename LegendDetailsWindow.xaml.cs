using NBALegende.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace NBALegende
{
    public partial class LegendDetailsWindow : Window
    {
        private NBALegend legend;

        private string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public LegendDetailsWindow(NBALegend legend)
        {
            InitializeComponent();
            this.legend = legend;
            LoadLegendData();
        }

        private void LoadLegendData()
        {
            txtFullName.Text = legend.FullName;
            txtJerseyNumber.Text = legend.JerseyNumber.ToString();
            txtDateAdded.Text = legend.DateAdded.ToString("dd.MM.yyyy. HH:mm");

            string imagePath = ResolvePath(legend.ImagePath);
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();
                imgLegend.Source = bitmap;
            }

            txtBiography.Text = LoadRtfAsPlainText(ResolvePath(legend.RtfPath));
        }

        private string LoadRtfAsPlainText(string rtfPath)
        {
            if (string.IsNullOrEmpty(rtfPath) || !File.Exists(rtfPath))
            {
                return "Biografija nije pronađena.";
            }

            FlowDocument document = new FlowDocument();
            TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

            FileStream fileStream = new FileStream(rtfPath, FileMode.Open);
            range.Load(fileStream, DataFormats.Rtf);
            fileStream.Close();

            return range.Text.Trim();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}