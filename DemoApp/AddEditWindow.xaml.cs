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
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        user25Entities1 dbmodel = new user25Entities1();
        List<Product> productList = new List<Product>();
        Product _currentProduct;

        public AddEditWindow(Product product)
        {
            InitializeComponent();
            _currentProduct = product;
            productList = dbmodel.Product.ToList();
            LoadComboBoxes();
            InitTextBoxes();
        }

        private void LoadComboBoxes()
        {
            using (var db = new user25Entities1())
            {
                List<ProductCategory> productCategories;
                productCategories = (from pc in db.ProductCategory select pc).ToList<ProductCategory>();
                foreach (var productCategory in productCategories)
                    CategoryIdComboBox.Items.Add(productCategory.ProductCategoryName);
            }
            using (var db = new user25Entities1())
            {
                List<UnitType> unitTypes;
                unitTypes = (from ut in db.UnitType select ut).ToList<UnitType>();
                foreach (var unitType in unitTypes)
                    UnitTypeIdComboBox.Items.Add(unitType.UnitTypeName);
            }
            using (var db = new user25Entities1())
            {
                List<ProductSupplier> productSuppliers;
                productSuppliers = (from ps in db.ProductSupplier select ps).ToList<ProductSupplier>();
                foreach (var productSupplier in productSuppliers)
                    SupplierIdComboBox.Items.Add(productSupplier.ProductSupplierName);
            }
            using (var db = new user25Entities1())
            {
                List<ProductManufacturer> productManufacturers;
                productManufacturers = (from pm in db.ProductManufacturer select pm).ToList<ProductManufacturer>();
                foreach (var productManufacturer in productManufacturers)
                    ManufacturerComboBox.Items.Add(productManufacturer.ProductManufacturerName);
            }
        }

        private void UnitTypeIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.UnitType = (from ut in dbmodel.UnitType where ut.UnitTypeName == UnitTypeIdComboBox.Text select ut).FirstOrDefault();
        }

        private void SupplierIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductManufacturer = (from pm in dbmodel.ProductManufacturer where pm.ProductManufacturerName == ManufacturerComboBox.Text select pm).FirstOrDefault();
        }

        private void CategoryIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductSupplier = (from ps in dbmodel.ProductSupplier where ps.ProductSupplierName == SupplierIdComboBox.Text select ps).FirstOrDefault();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductCategory = (from pc in dbmodel.ProductCategory where pc.ProductCategoryName == CategoryIdComboBox.Text select pc).FirstOrDefault();
        }

        private void InitTextBoxes()
        {
            ArticleTextBox.Text = _currentProduct.ProductArticleNumber;
            NameTextBox.Text = _currentProduct.ProductName;
            PriceTextBox.Text = _currentProduct.ProductCost.ToString();
            MaxDiscountTextBox.Text = _currentProduct.ProductMaxDiscountAmount.ToString();
            DiscountTextBox.Text = _currentProduct.ProductDiscountAmount.ToString();
            AmountInStockTextBox.Text = _currentProduct.ProductQuantityInStock.ToString();
            DescriptionTextBox.Text = _currentProduct.ProductDescription;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(ArticleTextBox.Text))
                MessageBox.Show("Введите артикул!", "Ошибка");
            if (string.IsNullOrEmpty(NameTextBox.Text))
                MessageBox.Show("Введите название!", "Ошибка");
            if (string.IsNullOrEmpty(PriceTextBox.Text))
                MessageBox.Show("Введите цену!", "Ошибка");
            if (string.IsNullOrEmpty(MaxDiscountTextBox.Text))
                MessageBox.Show("Введите макс. скидку!", "Ошибка");
            if (string.IsNullOrEmpty(DiscountTextBox.Text))
                MessageBox.Show("Введите действ. скидку!", "Ошибка");
            if (string.IsNullOrEmpty(AmountInStockTextBox.Text))
                MessageBox.Show("Введите количество!", "Ошибка");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            using (var dbmodel = new user25Entities1())
            {
                _currentProduct.ProductArticleNumber = ArticleTextBox.Text;
                _currentProduct.ProductName = NameTextBox.Text;
                _currentProduct.ProductCost = Convert.ToInt32(PriceTextBox.Text);
                _currentProduct.ProductMaxDiscountAmount = Convert.ToByte(MaxDiscountTextBox.Text);
                _currentProduct.ProductDiscountAmount = Convert.ToByte(DiscountTextBox.Text);
                _currentProduct.ProductQuantityInStock = Convert.ToInt32(AmountInStockTextBox.Text);
                _currentProduct.ProductDescription = DescriptionTextBox.Text;
                _currentProduct.UnitType = (from ut in dbmodel.UnitType where ut.UnitTypeName == UnitTypeIdComboBox.Text select ut).FirstOrDefault();
                _currentProduct.ProductManufacturer = (from pm in dbmodel.ProductManufacturer where pm.ProductManufacturerName == ManufacturerComboBox.Text select pm).FirstOrDefault();
                _currentProduct.ProductSupplier = (from ps in dbmodel.ProductSupplier where ps.ProductSupplierName == SupplierIdComboBox.Text select ps).FirstOrDefault();
                _currentProduct.ProductCategory = (from pc in dbmodel.ProductCategory where pc.ProductCategoryName == CategoryIdComboBox.Text select pc).FirstOrDefault();
                dbmodel.Product.Add(_currentProduct);
                try
                {
                    dbmodel.SaveChanges();
                    MessageBox.Show("Успех!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
