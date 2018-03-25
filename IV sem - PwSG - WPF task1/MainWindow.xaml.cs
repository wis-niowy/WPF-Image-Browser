using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;

namespace IV_sem___PwSG___WPF_task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        [XmlArray("ItemCollection")]
        [XmlArrayItem("Item")]
        public ObservableCollection<Item> itemList { get; set; }
        public ObservableCollection<CartItem> cartList { get; set; }
        Communicator communicator {get; set; }

        public MainWindow()
        {
            itemList = new ObservableCollection<Item>();
            cartList = new ObservableCollection<CartItem>();
            InitializeComponent();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is simple shop manager.", "About application", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Added to cart!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            Item it = (Item)(sender as Button).Tag;
            Window wind = new AddToCartWindow(it, this);
            communicator = new Communicator();
            wind.Show();
        }

        //[XmlRoot("ItemCollection")]
        //public class Item
        //{
        //    [XmlElement("Name")]
        //    public string Name { get; set; }
        //    [XmlElement("Description")]
        //    public string Description { get; set; }
        //    [XmlElement("Category")]
        //    public Category Category { get; set; }
        //    [XmlElement("Price")]
        //    public double Price { get; set; }
        //    public Item() { }
        //    public Item(string a, string b, Category c, double d)
        //    {
        //        Name = a; Description = b; Category = c; Price = d;
        //    }

        //}

        private void AddProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            itemList.Add(new Item("Computer", "Computer's description", Category.Electronics, 2499.99));
            itemList.Add(new Item("Apple", "Apple's description", Category.Food, 1.99));
            itemList.Add(new Item("Denims", "Denims's description", Category.Clothes, 50));
            itemList.Add(new Item("Computer", "Computer's description", Category.Electronics, 2499.99));
            //cartList.Add(new CartItem(new Item("Computer", "Computer's description", Category.Electronics, 2499.99)), 0);
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML files (.xml) | *.xml";
            ofd.DefaultExt = ".xml";
            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(ofd.FileName);
                //List<XNode> tempList = xdoc.Root.Elements("Item").Select(el => el.Nodes().ToList()[0]).ToList();
                var res = from myItem in xdoc.Root.Elements("Item")
                          select new Item((string)myItem.Element("ItemName"), (string)myItem.Element("ItemDescription"),
                          (Category)Enum.Parse(typeof(Category), (string)myItem.Element("ItemCategory")), (double)myItem.Element("ItemPrice"));
                          //{
                          //    Name = (string)myItem.Element("ItemName"),
                          //    Description = (string)myItem.Element("ItemDescription"),
                          //    Category = (Category)Enum.Parse(typeof(Category), (string)myItem.Element("ItemCategory")),
                          //    Price = (double)myItem.Element("ItemPrice")
                          //};
                foreach (var el in res.ToList() )
                {
                    itemList.Add(el);
                }
            }
            
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //XmlSerializer ser = new XmlSerializer(typeof(List<Item>));
            sfd.Filter = "XML files (.xml) | *.xml";
            sfd.DefaultExt = ".xml";
            Nullable<bool> result = sfd.ShowDialog();
            if (result == true)
            {
                //TextWriter writer = new StreamWriter(sfd.FileName);
                //ser.Serialize(writer, itemList);
                //writer.Close();
                List<System.Xml.Linq.XElement> intNodeList = new List<System.Xml.Linq.XElement>();
                foreach (var item in itemList)
                {
                    intNodeList.Add(new System.Xml.Linq.XElement("Item", new System.Xml.Linq.XElement("ItemName", item.Name),
                                                                         new System.Xml.Linq.XElement("ItemDescription", item.Description),
                                                                         new System.Xml.Linq.XElement("ItemCategory", item.Category),
                                                                         new System.Xml.Linq.XElement("ItemPrice", item.Price)));
                }
                System.Xml.Linq.XElement externalNode = new System.Xml.Linq.XElement("ItemsCollection", intNodeList);
                externalNode.Save(sfd.FileName);
            }
        }

        private void ClearProductsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            itemList.Clear();
            cartList.Clear();
        }

        private void ByNameCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox bx = sender as CheckBox;
            if (bx.IsChecked == true)
            {
                byNameTextBox.IsEnabled = true;
            }
            else
                byNameTextBox.IsEnabled = false;
        }

        private void ByPriceCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox bx = sender as CheckBox;
            if (bx.IsChecked == true)
            {
                fromTextBox.IsEnabled = true;
                toTextBox.IsEnabled = true;
            }
            else
            {
                fromTextBox.IsEnabled = false;
                toTextBox.IsEnabled = false;
            }
        }

        private void ByCategoryCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox bx = sender as CheckBox;
            if (bx.IsChecked == true)
            {
                byCategoryComboBox.IsEnabled = true;
            }
            else
            {
                byCategoryComboBox.IsEnabled = false;
            }
        }


        private void FilterByName(ObservableCollection<Item> view, out ObservableCollection<Item> tempItem)
        {
            ObservableCollection<Item> temp = new ObservableCollection<Item>();
            if (byNameTextBox.Text != "")
            {
                    foreach (var p in view.Where(p => (p.Name == byNameTextBox.Text)))
                        temp.Add(p);
                tempItem = temp;
            }
            else
            {
                tempItem = itemList;
                return;
            }
        }
        private void FilterByPrice(ObservableCollection<Item> view, out ObservableCollection<Item> tempItem)
        {
            ObservableCollection<Item> temp = new ObservableCollection<Item>();
            int fromPrice, toPrice;
            Int32.TryParse(fromTextBox.Text, out fromPrice);
            Int32.TryParse(toTextBox.Text, out toPrice);

            if (fromPrice >= 0 && toPrice >= 0 && fromPrice < int.MaxValue && toPrice < int.MaxValue)
            {
                foreach (var p in view.Where(p => (p.Price > fromPrice && p.Price < toPrice)))
                    temp.Add(p);
                tempItem = temp;
            }
            else
            {
                MessageBox.Show("Invalid format of price!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tempItem = null;
                ShopListView.ItemsSource = itemList;
                return;
            }
        }

        private void FilterByCategory(ObservableCollection<Item> view, out ObservableCollection<Item> tempItem)
        {
            ObservableCollection<Item> temp = new ObservableCollection<Item>();
            Category c = (Category)byCategoryComboBox.SelectedIndex;

            foreach (var p in view.Where(p => (p.Category == c)))
                temp.Add(p);
            tempItem = temp;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (priceCheckBox.IsChecked == false && nameCheckBox.IsChecked == false && categoryCheckBox.IsChecked == false)
            {
                MessageBox.Show("No checkbox ticked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ObservableCollection<Item> currItems = new ObservableCollection<Item>();
            currItems = itemList;
            ObservableCollection<Item> tempItems;
            CollectionViewSource newCollectionViewSource = new CollectionViewSource();

            if (nameCheckBox.IsChecked == true)
            {
                FilterByName(currItems, out tempItems);
                if (tempItems != null)
                {
                    currItems = tempItems;
                }
            }

            if (priceCheckBox.IsChecked == true)
            {
                FilterByPrice(currItems, out tempItems);
                if (tempItems != null)
                {
                    currItems = tempItems;
                }
            }

            if (categoryCheckBox.IsChecked == true)
            {
                FilterByCategory(currItems, out tempItems);
                if (tempItems != null)
                {
                    currItems = tempItems;
                }
            }

            newCollectionViewSource.Source = currItems;
            var newShopView = newCollectionViewSource.View;
            ShopListView.ItemsSource = newShopView;
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource newCollectionViewSource = new CollectionViewSource();
            newCollectionViewSource.Source = itemList;
            var newShopView = newCollectionViewSource.View;
            ShopListView.ItemsSource = newShopView;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            CartItem it = (CartItem)(sender as Button).Tag;
            it.Counter++;
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            CartItem it = (CartItem)(sender as Button).Tag;
            if (it.Counter > 0)
                it.Counter--;
        }
    }

    public class Item
    {
        //[XmlElement("Name")]
        public string Name { get; set; }
        //[XmlElement("Description")]
        public string Description { get; set; }
        //[XmlElement("Category")]
        public Category Category { get; set; }
        //[XmlElement("Price")]
        public double Price { get; set; }
        public Item() { }
        public Item(string a, string b, Category c, double d)
        {
            Name = a; Description = b; Category = c; Price = d;
        }

    }
    public class CartItem: INotifyPropertyChanged
    {
        private int counter;
        public Item item { get; set; }
        public int Counter {
            get { return counter; }
            set
            {
                counter = value;
                NotifyPropertyChanged("Counter");
            }
        }
        public CartItem(Item it, int c)
        {
            item = it;
            Counter = c;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum Category
    {
        Electronics, Food, Clothes
    }
    
}
