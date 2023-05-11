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

namespace DemoApp
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private User _currentUser = new User();
        public static Product _currentProduct;
        user25Entities1 dbmodel = new user25Entities1();
        List<Product> productList = new List<Product>();

        public ClientWindow(User user)
        {
            InitializeComponent();

            productList = dbmodel.Product.ToList();
                DiscountFilter.ItemsSource = new List<string>()
            {
                "0-10%","10-15%","15-100%", "Все диапазоны"
            };
            PriceFilter.ItemsSource = new List<string>
            {
                "По возрастанию", "По убыванию"
            };
            ProductsList.ItemsSource = productList;

            foreach (Product product in productList)
            {
                product.ProductPhoto = $"/Resources/{product.ProductPhoto}";
                foreach (ProductManufacturer manufacturer in dbmodel.ProductManufacturer)
                {
                    if (manufacturer.ProductManufacturerID == product.ProductManufacturerID)
                    {
                        product.ProductManufacturer = manufacturer;
                    }
                }
            }
            _currentUser = user;
            updateRecordAmount();
            if(_currentUser != null)
            {
                if (_currentUser.RoleID == 2)
                    AddButton.IsEnabled = true;
                else
                    AddButton.IsEnabled = false;
            }
        }

        private void EnableButtons()
        {
            if (_currentUser.RoleID == 2 && ProductsList.SelectedItem != null)
            {
                DeleteButton.IsEnabled = true;
                EditButton.IsEnabled = true;
            }
            else
            {
                AddButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }

        private void PriceFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableButtons();
            productList = dbmodel.Product.ToList();
            switch (PriceFilter.SelectedIndex)
            {
                case 0:
                    {
                        ProductsList.ItemsSource = productList.OrderBy(p => p.ProductCost);
                        updateRecordAmount();
                        break;
                    }
                case 1:
                    {
                        ProductsList.ItemsSource = productList.OrderByDescending(p => p.ProductCost);
                        updateRecordAmount();
                        break;
                    }
            }
        }

        private void DiscountFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableButtons();
            productList = dbmodel.Product.ToList();
			switch (DiscountFilter.SelectedIndex)
			{
				case 0:
					{
						productList = dbmodel.Product.Where(p => p.ProductDiscountAmount < 10).ToList();
                        ProductsList.ItemsSource = productList;
                        updateRecordAmount();
						break;
					}
				case 1:
					{
						productList = dbmodel.Product.Where(p => p.ProductDiscountAmount > 10 && p.ProductDiscountAmount < 15).ToList();
                        ProductsList.ItemsSource = productList;

                        updateRecordAmount();
						break;
					}
				case 2:
					{
						productList = dbmodel.Product.Where(p => p.ProductDiscountAmount >= 15).ToList();
                        ProductsList.ItemsSource = productList;
                        updateRecordAmount();
						break;
					}
                case 3:
					{
						productList = dbmodel.Product.ToList();
                        ProductsList.ItemsSource = productList;
                        updateRecordAmount();
						break;
					}
            }
        }
        private void updateRecordAmount()
        {
            recordAmountLabel.Content = $"{ProductsList.Items.Count} из {productList.Count}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableButtons();
            productList = productList.Where(p => p.ProductName.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
            ProductsList.ItemsSource = productList;
            updateRecordAmount();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void ProductsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableButtons();
            _currentProduct = ProductsList.SelectedItem as Product;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(new Product(), _currentUser);
            addWindow.Show();
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditWindow editWindow = new EditWindow(ProductsList.SelectedItem as Product, _currentUser);
            editWindow.Show();
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var driversForDeleting = ProductsList.SelectedItems.Cast<Product>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {driversForDeleting.Count} элементов?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    dbmodel.Product.RemoveRange(driversForDeleting);
                    dbmodel.SaveChanges();
                    MessageBox.Show("Данные удалены!", "Окно оповещений");
                    ProductsList.ItemsSource = dbmodel.Product.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
