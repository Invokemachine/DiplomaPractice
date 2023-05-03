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
        user25Entities1 dbmodel = new user25Entities1();
        List<Product> productList = new List<Product>();

        public ClientWindow()
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
            updateRecordAmount();
        }

        private void PriceFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
    }
}
