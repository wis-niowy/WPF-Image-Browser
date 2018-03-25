using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IV_sem___PwSG___WPF_task1
{
    /// <summary>
    /// Interaction logic for AddToCartWindow.xaml
    /// </summary>
    public partial class AddToCartWindow : Window
    {
        public Item currentItem;
        public Window parentWindow;
        public AddToCartWindow()
        {
            InitializeComponent();
        }
        public AddToCartWindow(Item it, Window parentWind)
        {
            InitializeComponent();
            NameOfProductBox.Text = it.Name;
            currentItem = it;
            parentWindow = parentWind;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)parentWindow).cartList.Add(new CartItem(currentItem, int.Parse(CountBox.Text)));
        }
    }

    public class Communicator
    {
        public Category CurrentCategor { get; set; }
    }
}
