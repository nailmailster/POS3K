using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POS3K
{
    public partial class WindowChecks : Window
    {
        public WindowChecks()
        {
            InitializeComponent();
            Loaded += WindowChecks_Loaded;
        }

        private void WindowChecks_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridChecks.ItemsSource = ChequesDBF.GetDataTableCheques().DefaultView;
            dataGridChecks.Columns[3].Visibility = Visibility.Hidden;
            dataGridChecks.Columns[5].Visibility = Visibility.Hidden;
            dataGridChecks.Columns[7].Visibility = Visibility.Hidden;

            int itemsCount = dataGridChecks.Items.Count;

            //dataGridChecks.Focus();

            dataGridChecks.SelectedItems.Clear();
            dataGridChecks.SelectedItem = dataGridChecks.Items[itemsCount - 1];
            dataGridChecks.CurrentItem = dataGridChecks.SelectedItem;
            dataGridChecks.ScrollIntoView(dataGridChecks.SelectedItem);

            DataRowView rowViewSelected = dataGridChecks.SelectedItem as DataRowView;
            if (rowViewSelected != null)
            {
                string dn = rowViewSelected["dn"].ToString();

                dataGridCheckGoods.ItemsSource = ChequesDBF.GetDataTableCheqGood(dn).DefaultView;
                dataGridCheckGoods.Columns[4].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[5].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[6].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[7].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[9].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[10].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[11].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[13].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[14].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[15].Visibility = Visibility.Hidden;
            }
            dataGridChecks.Focus();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            //  сделаем окно-владелец (окно меню) непрозрачным (видимым)
            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            //  стартуем анимацию непрозрачности
            Owner.BeginAnimation(OpacityProperty, doubleAnimation);
            Close();
        }

        private void DataGridChecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView rowViewSelected = dataGridChecks.SelectedItem as DataRowView;
            if (rowViewSelected != null)
            {
                string dn = rowViewSelected["dn"].ToString();
                dataGridCheckGoods.ItemsSource = ChequesDBF.GetDataTableCheqGood(dn).DefaultView;
                dataGridCheckGoods.Columns[4].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[5].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[6].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[7].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[9].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[11].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[13].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[14].Visibility = Visibility.Hidden;
                dataGridCheckGoods.Columns[15].Visibility = Visibility.Hidden;
            }
        }
    }
}
