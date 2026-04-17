using Microsoft.Win32;
using NBALegende.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Shapes;

namespace NBALegende
{
    public partial class AddLegendWindow : Window
    {
        private NBALegend legendForEdit;
        private bool isEditMode = false;
        private ObservableCollection<NBALegend> legends;
        private string selectedImageAbsolutePath;

        public AddLegendWindow(ObservableCollection<NBALegend> legends, NBALegend legendForEdit)
        {
            InitializeComponent();
            this.legends = legends;
            this.legendForEdit = legendForEdit;
            isEditMode = true;

            PrepareWindow();
            LoadLegendForEdit();
        }

        public AddLegendWindow(ObservableCollection<NBALegend> legends)
        {
            InitializeComponent();
            this.legends = legends;
            PrepareWindow();
        }


        private void LoadTextColors()
        {
            cbTextColor.Items.Clear();

            PropertyInfo[] colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo property in colorProperties.OrderBy(p => p.Name))
            {
                Color color = (Color)property.GetValue(null, null);

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                Rectangle rectangle = new Rectangle();
                rectangle.Width = 16;
                rectangle.Height = 16;
                rectangle.Fill = new SolidColorBrush(color);
                rectangle.Stroke = Brushes.Black;
                rectangle.Margin = new Thickness(0, 0, 6, 0);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = property.Name;
                textBlock.VerticalAlignment = VerticalAlignment.Center;

                stackPanel.Children.Add(rectangle);
                stackPanel.Children.Add(textBlock);

                ComboBoxItem item = new ComboBoxItem();
                item.Content = stackPanel;
                item.Tag = color;

                cbTextColor.Items.Add(item);
            }

            cbTextColor.SelectedIndex = 0;
        }

        private void PrepareWindow()
        {
            LoadFonts();
            LoadTextColors();
            cbFontSize.SelectedIndex = 2;

            if (isEditMode)
            {
                this.Title = "Izmena NBA legende";
                txtWindowTitle.Text = "IZMENA NBA LEGENDE";
                btnSave.Content = "IZMENI";
            }
        }

        private string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            if (System.IO.Path.IsPathRooted(path))
            {
                return path;
            }

            return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, path);
        }

        private string MakeRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                return "";
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Uri baseUri = new Uri(baseDirectory.EndsWith("\\") ? baseDirectory : baseDirectory + "\\");
            Uri fileUri = new Uri(absolutePath);

            return Uri.UnescapeDataString(
                baseUri.MakeRelativeUri(fileUri).ToString().Replace('/', '\\')
            );
        }

        private void LoadLegendForEdit()
        {
            tbFullName.Text = legendForEdit.FullName;
            tbJerseyNumber.Text = legendForEdit.JerseyNumber.ToString();

            string imagePath = ResolvePath(legendForEdit.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                selectedImageAbsolutePath = imagePath;
                tbImagePath.Text = imagePath;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();

                imgPreview.Source = bitmap;
                tbNoImage.Visibility = Visibility.Hidden;
            }

            string rtfPath = ResolvePath(legendForEdit.RtfPath);
            if (System.IO.File.Exists(rtfPath))
            {
                TextRange range = new TextRange(rtbBiography.Document.ContentStart, rtbBiography.Document.ContentEnd);

                using (FileStream fileStream = new FileStream(rtfPath, FileMode.Open))
                {
                    range.Load(fileStream, DataFormats.Rtf);
                }
            }
        }
        private void LoadFonts()
        {
            foreach (FontFamily font in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = font.Source;
                item.FontFamily = font;
                cbFonts.Items.Add(item);
            }

            for (int i = 0; i < cbFonts.Items.Count; i++)
            {
                ComboBoxItem item = (ComboBoxItem)cbFonts.Items[i];
                if (item.Content.ToString() == "Arial")
                {
                    cbFonts.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedImageAbsolutePath = openFileDialog.FileName;
                tbImagePath.Text = selectedImageAbsolutePath;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedImageAbsolutePath, UriKind.Absolute);
                bitmap.EndInit();

                imgPreview.Source = bitmap;
                tbNoImage.Visibility = Visibility.Hidden;
            }
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, rtbBiography);
            rtbBiography.Focus();
        }

        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, rtbBiography);
            rtbBiography.Focus();
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, rtbBiography);
            rtbBiography.Focus();
        }

        private void cbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFonts.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cbFonts.SelectedItem;
                FontFamily fontFamily = new FontFamily(selectedItem.Content.ToString());
                rtbBiography.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
                rtbBiography.Focus();
            }
        }

        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFontSize.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cbFontSize.SelectedItem;
                double fontSize = Convert.ToDouble(selectedItem.Content.ToString());
                rtbBiography.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                rtbBiography.Focus();
            }
        }

        private void cbTextColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTextColor.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cbTextColor.SelectedItem;
                Color selectedColor = (Color)selectedItem.Tag;
                Brush brush = new SolidColorBrush(selectedColor);

                rtbBiography.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                rtbBiography.Focus();
            }
        }

        private void rtbBiography_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbBiography.Document.ContentStart, rtbBiography.Document.ContentEnd);
            string text = textRange.Text.Trim();

            if (text == "")
            {
                tbWordCount.Text = "Broj reči: 0";
                return;
            }

            string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            tbWordCount.Text = "Broj reči: " + words.Length;
        }

        private void rtbBiography_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object boldValue = rtbBiography.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            if (boldValue != DependencyProperty.UnsetValue && boldValue.Equals(FontWeights.Bold))
                btnBold.Background = Brushes.LightGray;
            else
                btnBold.ClearValue(Button.BackgroundProperty);

            object italicValue = rtbBiography.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            if (italicValue != DependencyProperty.UnsetValue && italicValue.Equals(FontStyles.Italic))
                btnItalic.Background = Brushes.LightGray;
            else
                btnItalic.ClearValue(Button.BackgroundProperty);

            object underlineValue = rtbBiography.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (underlineValue != DependencyProperty.UnsetValue && underlineValue.Equals(TextDecorations.Underline))
                btnUnderline.Background = Brushes.LightGray;
            else
                btnUnderline.ClearValue(Button.BackgroundProperty);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }
            string fullName = tbFullName.Text.Trim();
            int jerseyNumber = int.Parse(tbJerseyNumber.Text.Trim());


            TextRange textRange = new TextRange(rtbBiography.Document.ContentStart, rtbBiography.Document.ContentEnd);
            string biographyText = textRange.Text.Trim();

           

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string imagesDirectory = System.IO.Path.Combine(baseDirectory, "Images");
            string dataDirectory = System.IO.Path.Combine(baseDirectory, "Data");

            Directory.CreateDirectory(imagesDirectory);
            Directory.CreateDirectory(dataDirectory);

            string imagePathToSave;
            string rtfPathToSave;

            if (isEditMode)
            {
                string oldImagePath = ResolvePath(legendForEdit.ImagePath);

                if (selectedImageAbsolutePath == oldImagePath)
                {
                    imagePathToSave = legendForEdit.ImagePath;
                }
                else
                {
                    string imageFileName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(selectedImageAbsolutePath);
                    string newImageAbsolutePath = System.IO.Path.Combine(imagesDirectory, imageFileName);
                    File.Copy(selectedImageAbsolutePath, newImageAbsolutePath, true);
                    imagePathToSave = MakeRelativePath(newImageAbsolutePath);
                }

                string oldRtfPath = ResolvePath(legendForEdit.RtfPath);

                if (oldRtfPath != "" && File.Exists(oldRtfPath))
                {
                    using (FileStream fileStream = new FileStream(oldRtfPath, FileMode.Create))
                    {
                        textRange.Save(fileStream, DataFormats.Rtf);
                    }

                    rtfPathToSave = legendForEdit.RtfPath;
                }
                else
                {
                    string rtfFileName = DateTime.Now.Ticks + "_" + fullName.Replace(" ", "_") + ".rtf";
                    string newRtfAbsolutePath = System.IO.Path.Combine(dataDirectory, rtfFileName);

                    using (FileStream fileStream = new FileStream(newRtfAbsolutePath, FileMode.Create))
                    {
                        textRange.Save(fileStream, DataFormats.Rtf);
                    }

                    rtfPathToSave = MakeRelativePath(newRtfAbsolutePath);
                }

                legendForEdit.FullName = fullName;
                legendForEdit.JerseyNumber = jerseyNumber;
                legendForEdit.ImagePath = imagePathToSave;
                legendForEdit.RtfPath = rtfPathToSave;

                MessageBox.Show(
                    "Legenda je uspešno izmenjena.",
                    "Uspešna izmena",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                this.Close();
                return;
            }

            string newImageFileName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(selectedImageAbsolutePath);
            string createdImageAbsolutePath = System.IO.Path.Combine(imagesDirectory, newImageFileName);
            File.Copy(selectedImageAbsolutePath, createdImageAbsolutePath, true);

            string newRtfFileName = DateTime.Now.Ticks + "_" + fullName.Replace(" ", "_") + ".rtf";
            string createdRtfAbsolutePath = System.IO.Path.Combine(dataDirectory, newRtfFileName);

            using (FileStream fileStream = new FileStream(createdRtfAbsolutePath, FileMode.Create))
            {
                textRange.Save(fileStream, DataFormats.Rtf);
            }

            NBALegend newLegend = new NBALegend();
            newLegend.IsSelected = false;
            newLegend.JerseyNumber = jerseyNumber;
            newLegend.FullName = fullName;
            newLegend.ImagePath = MakeRelativePath(createdImageAbsolutePath);
            newLegend.RtfPath = MakeRelativePath(createdRtfAbsolutePath);
            newLegend.DateAdded = DateTime.Now;

            legends.Add(newLegend);

            MessageBox.Show(
                "Legenda je uspešno dodata.",
                "Uspešno dodavanje",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearValidationErrors()
        {
            txtFullNameError.Text = "";
            txtJerseyNumberError.Text = "";
            txtImageError.Text = "";
            txtBiographyError.Text = "";
        }

        private bool ValidateFields()
        {
            ClearValidationErrors();
            bool isValid = true;

            string fullName = tbFullName.Text.Trim();
            if (fullName == "")
            {
                txtFullNameError.Text = "Ime i prezime je obavezno.";
                isValid = false;
            }

            int jerseyNumber;
            bool jerseyOk = int.TryParse(tbJerseyNumber.Text.Trim(), out jerseyNumber);
            if (!jerseyOk)
            {
                txtJerseyNumberError.Text = "Broj dresa mora biti broj.";
                isValid = false;
            }
            else if (jerseyNumber <= 0)
            {
                txtJerseyNumberError.Text = "Broj dresa mora biti veći od nule.";
                isValid = false;
            }
            else if (jerseyNumber > 99)
            {
                txtJerseyNumberError.Text = "Broj dresa mora biti manji od 100.";
                isValid = false;
            }

            if (selectedImageAbsolutePath == null || selectedImageAbsolutePath == "")
            {
                txtImageError.Text = "Morate izabrati sliku igrača.";
                isValid = false;
            }

            TextRange textRange = new TextRange(rtbBiography.Document.ContentStart, rtbBiography.Document.ContentEnd);
            string biographyText = textRange.Text.Trim();

            if (biographyText == "")
            {
                txtBiographyError.Text = "Biografija je obavezna.";
                isValid = false;
            }

            return isValid;
        }
    }
}