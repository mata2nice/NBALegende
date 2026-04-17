using NBALegende.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace NBALegende
{
    public partial class LegendsTableWindow : Window
    {
        private ObservableCollection<NBALegend> legends;
        private User loggedUser;
        private LegendDataIO legendDataIO;

        public LegendsTableWindow(User loggedUser)
        {
            InitializeComponent();
            this.loggedUser = loggedUser;
            legendDataIO = new LegendDataIO();
            LoadLegendsFromXml();
            AdjustInterfaceByRole();
        }

        private void AdjustInterfaceByRole()
        {
            if (loggedUser.Role == UserRole.Visitor)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                cbSelectAll.Visibility = Visibility.Collapsed;

                // sakrij i prvu kolonu sa checkbox-ovima u tabeli 
                dgLegends.Columns[0].Visibility = Visibility.Collapsed;

                // visitor se ne pita nis bvrate
                dgLegends.IsReadOnly = true;
            }
            else
            {
                btnAdd.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                cbSelectAll.Visibility = Visibility.Visible;
                dgLegends.Columns[0].Visibility = Visibility.Visible;
                dgLegends.IsReadOnly = false;
            }
        }


        private void LoadLegendsFromXml()
        {
            legends = legendDataIO.LoadLegends();
            dgLegends.ItemsSource = legends;
        }

        private void cbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (NBALegend legend in legends)
            {
                legend.IsSelected = true;
            }

            dgLegends.Items.Refresh();
        }

        private void cbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (NBALegend legend in legends)
            {
                legend.IsSelected = false;
            }

            dgLegends.Items.Refresh();
        }

        private void LegendHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = (Hyperlink)sender;
            NBALegend selectedLegend = (NBALegend)hyperlink.Tag;

            if (selectedLegend != null)
            {
                if (loggedUser.Role == UserRole.Visitor)
                {
                    LegendDetailsWindow legendDetailsWindow = new LegendDetailsWindow(selectedLegend);
                    legendDetailsWindow.ShowDialog();
                }
                else
                {
                    AddLegendWindow addLegendWindow = new AddLegendWindow(legends, selectedLegend);
                    addLegendWindow.ShowDialog();

                    legendDataIO.SaveLegends(legends);
                    dgLegends.Items.Refresh();
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddLegendWindow addLegendWindow = new AddLegendWindow(legends);
            addLegendWindow.ShowDialog();

            legendDataIO.SaveLegends(legends);
            dgLegends.Items.Refresh();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var legendsForDelete = legends.Where(l => l.IsSelected).ToList();

            if (legendsForDelete.Count == 0)
            {
                MessageBox.Show(
                    "Niste označili nijednu legendu za brisanje.",
                    "Brisanje nije moguće",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Da li ste sigurni da želite da obrišete označene legende?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (NBALegend legend in legendsForDelete)
                {
                    legends.Remove(legend);
                }
                legendDataIO.SaveLegends(legends);
                MessageBox.Show(
                    "Označene legende su uspešno obrisane.",
                    "Uspešno brisanje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}