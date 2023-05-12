using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
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
    public partial class EditWindow : Window
    {
        user25Entities1 dbmodel = new user25Entities1();
        Product _currentProduct;
        User _currentUser;

        public EditWindow(Product product, User user)
        {
            InitializeComponent();
            LoadComboBoxes();
            _currentProduct = ClientWindow._currentProduct;
            _currentUser = user;
            DataInitMethod();
            InitImage();
        }
        /// <summary>
        /// Загрузка данных выбранного ранее продукта в текстбоксы
        /// </summary>
        private void DataInitMethod()
        {
            if (_currentProduct != null)
            {
                ArticleTextBox.Text = _currentProduct.ProductArticleNumber;
                NameTextBox.Text = _currentProduct.ProductName;
                PriceTextBox.Text = Convert.ToString(_currentProduct.ProductCost);
                MaxDiscountTextBox.Text = Convert.ToString(_currentProduct.ProductMaxDiscountAmount);
                DiscountTextBox.Text = Convert.ToString(_currentProduct.ProductDiscountAmount);
                AmountInStockTextBox.Text = Convert.ToString(_currentProduct.ProductQuantityInStock);
                DescriptionTextBox.Text = _currentProduct.ProductDescription;
                UnitTypeIdComboBox.Text = Convert.ToString(_currentProduct.UnitType);
                ManufacturerComboBox.Text = Convert.ToString(_currentProduct.ProductManufacturer);
                SupplierIdComboBox.Text = Convert.ToString(_currentProduct.ProductSupplier);
                CategoryIdComboBox.Text = Convert.ToString(_currentProduct.ProductCategory);
            }
        }
        /// <summary>
        /// Метод заполнения комбобоксов
        /// </summary>
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

        /// <summary>
        /// Метод инициализации картинки
        /// </summary>
        private void InitImage()
        {
            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            try
            {
                imageSource.UriSource = new Uri(@"/DemoApp;component" + _currentProduct.ProductPhoto);
                BitmapImage nullImage = new BitmapImage();
                nullImage.BeginInit();
                nullImage.UriSource = new Uri(@"/DemoApp;component/Resources/", UriKind.Relative);
                if (imageSource.UriSource == nullImage.UriSource)
                {
                    imageSource.UriSource = new Uri(@"/DemoApp;component/Resources/picture.png", UriKind.Relative);
                }
            }
            catch
            {
                return;
            }
            imageSource.EndInit();
            Picture.Source = imageSource;
        }

        private void EditMethod()
        {
            Product item = ClientWindow._currentProduct;
            foreach (Product product in dbmodel.Product)
            {
                if (product.ProductID == item.ProductID)
                {
                    _currentProduct = product;
                    break;
                }
            }
            StringBuilder errors = new StringBuilder();
            _currentProduct.ProductArticleNumber = ArticleTextBox.Text;
            _currentProduct.ProductName = NameTextBox.Text;
            _currentProduct.ProductCost = Convert.ToDecimal(PriceTextBox.Text);
            _currentProduct.ProductMaxDiscountAmount = Convert.ToByte(MaxDiscountTextBox.Text);
            _currentProduct.UnitType = (from ut in dbmodel.UnitType where ut.UnitTypeName == UnitTypeIdComboBox.Text select ut).FirstOrDefault();
            _currentProduct.ProductManufacturer = (from pm in dbmodel.ProductManufacturer where pm.ProductManufacturerName == ManufacturerComboBox.Text select pm).FirstOrDefault();
            _currentProduct.ProductSupplier = (from ps in dbmodel.ProductSupplier where ps.ProductSupplierName == SupplierIdComboBox.Text select ps).FirstOrDefault();
            _currentProduct.ProductCategory = (from pc in dbmodel.ProductCategory where pc.ProductCategoryName == CategoryIdComboBox.Text select pc).FirstOrDefault();

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
            if (Convert.ToInt16(MaxDiscountTextBox.Text) < Convert.ToInt16(DiscountTextBox.Text))
                MessageBox.Show("Максимальная скидка не может быть меньше действующей!", "Ошибка");
            if (Convert.ToInt16(AmountInStockTextBox.Text) <= 0)
                MessageBox.Show("Товар можно добавить только если он есть на складе!", "Ошибка");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            else
            {
                try
                {
                    dbmodel.Product.AddOrUpdate(_currentProduct);
                    dbmodel.SaveChanges();
                    MessageBox.Show("Информация успешно добавлена!", "Окно оповещений");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// Методы передачи данных из комбобоксов в редактируемый продукт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnitTypeIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.UnitType = (from ut in dbmodel.UnitType where ut.UnitTypeName == UnitTypeIdComboBox.Text select ut).FirstOrDefault();
        }

        private void SupplierIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductSupplier = (from ps in dbmodel.ProductSupplier where ps.ProductSupplierName == SupplierIdComboBox.Text select ps).FirstOrDefault();
        }

        private void CategoryIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductCategory = (from pc in dbmodel.ProductCategory where pc.ProductCategoryName == CategoryIdComboBox.Text select pc).FirstOrDefault();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct.ProductManufacturer = (from pm in dbmodel.ProductManufacturer where pm.ProductManufacturerName == ManufacturerComboBox.Text select pm).FirstOrDefault();
        }

        /// <summary>
        /// Редактирование картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageButoon_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                string filename = ofd.FileName;
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                FileInfo fileInfo = new FileInfo(currentDir);
                DirectoryInfo dirInfo = fileInfo.Directory.Parent;
                string parentDirName = dirInfo.Name;

                fileInfo = new FileInfo(parentDirName);
                dirInfo = fileInfo.Directory.Parent;
                parentDirName = dirInfo.Name;

                fileInfo = new FileInfo(dirInfo.ToString());
                dirInfo = fileInfo.Directory.Parent;
                parentDirName = dirInfo.ToString() + "\\Resources\\" + ofd.SafeFileName;

                System.IO.File.Copy(filename, parentDirName, true);

                _currentProduct.ProductPhoto = ofd.SafeFileName;

                dbmodel.Entry(_currentProduct).State = EntityState.Modified;
                dbmodel.SaveChanges();

                InitImage();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            EditMethod();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow(_currentUser);
            clientWindow.Show();
            Close();
        }
    }
}
