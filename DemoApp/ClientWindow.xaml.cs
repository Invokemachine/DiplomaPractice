using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
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
    public partial class ClientWindow : Window
    {
        private User _currentUser = new User();
        private bool _orderExists;
        public static Product _currentProduct;
        user25Entities1 dbmodel = new user25Entities1();
        List<Product> productList = new List<Product>();
        Order _currentOrder = new Order();

        public ClientWindow(User user, Order order = null, bool orderExists = false)
        {
            InitializeComponent();
            productList = dbmodel.Product.ToList();

            ///Инициализация фильтров
            DiscountFilter.ItemsSource = new List<string>()
            {
                "0-10%","10-15%","15-100%", "Все диапазоны"
            };
            PriceFilter.ItemsSource = new List<string>
            {
                "По возрастанию", "По убыванию"
            };
            ProductsList.ItemsSource = productList;
            ///Вывод картинок в список
            foreach (Product product in productList)
            {
                product.ProductPhoto = $"Resources/{product.ProductPhoto}";

                foreach (ProductManufacturer manufacturer in dbmodel.ProductManufacturer)
                {
                    if (manufacturer.ProductManufacturerID == product.ProductManufacturerID)
                    {
                        product.ProductManufacturer = manufacturer;
                    }
                }
            }
            ///Если пользователь уже создал заказ и он не пустой
            _orderExists = orderExists;
            if (order != null)
            {
                _currentOrder = order;
            }
            ///Кнопка заказов видима только для клиентов и гостей
            if (_currentUser.RoleID == 1 || _currentUser.RoleID == 0)
            {
                NewOrderButton.Visibility = Visibility.Visible;
            }
            if (user != null)
            {
                _currentUser = user;
            }
            updateRecordAmount();
            ///Скрытие кнопок добавления, удаления и редактирования товаров для клиентов
            if (_currentUser != null)
            {
                if (_currentUser.RoleID == 2 || _currentUser.RoleID == 3)
                    AddButton.IsEnabled = true;
                else if (_currentUser.RoleID == 1)
                {
                    AddButton.Visibility = Visibility.Hidden;
                    EditButton.Visibility = Visibility.Hidden;
                    DeleteButton.Visibility = Visibility.Hidden;
                    NewOrderButton.Visibility = Visibility.Visible;
                }
            }
            if (_currentUser.UserID == 0)
            {
                _currentOrder.UserID = null;
            }
        }
        /// <summary>
        /// Метод для блокировки и разблокировки кнопок
        /// </summary>
        private void EnableButtons()
        {
            if (_currentUser != null && _currentUser.RoleID == 2 && ProductsList.SelectedItem != null)
            {
                DeleteButton.IsEnabled = true;
                EditButton.IsEnabled = true;
            }
            else if (_currentUser == null)
            {
                AddButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
            else
            {
                AddButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }
        /// <summary>
        /// Фильтр по цене
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Фильтр по скидкам
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Метод для обновления количества элементов в списке
        /// </summary>
        private void updateRecordAmount()
        {
            recordAmountLabel.Content = $"{ProductsList.Items.Count} из {productList.Count}";
        }
        /// <summary>
        /// Поиск по названию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var elements = ProductsList.SelectedItems.Cast<Product>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {elements.Count} элементов?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    dbmodel.Product.RemoveRange(elements);
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

        private void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow(_currentUser, _currentOrder);
            orderWindow.Show();
            Close();
        }
        /// <summary>
        /// Добавление к заказу через контекстное меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities1())
            {
                Order order;
                var product = new Product();
                var foundOrder = false;

                ///Поиск товара
                foreach (var item in dbmodel.Product)
                {
                    if (item == ProductsList.SelectedItem)
                    {
                        product = item;
                    }
                }
                ///Проверка на наличие уже созданного ранее заказа
                foreach (var ord in dbmodel.Order)
                {
                    if (ord.UserID == _currentUser.UserID && ord.OrderStatusID == 1)
                    {
                        _currentOrder = ord;
                        foundOrder = true;
                        break;
                    }
                    if (_orderExists)
                    {
                        if (ord.OrderID == _currentOrder.OrderID)
                        {
                            _currentOrder = ord;
                            foundOrder = true;
                            break;
                        }
                    }
                }
                ///Заполнение существующего заказа
                if (foundOrder)
                {
                    foreach (var ordprod in db.OrderProduct.ToList())
                    {
                        if (ordprod.ProductID == product.ProductID && ordprod.OrderID == _currentOrder.OrderID)
                        {
                            ordprod.Count++;
                            db.Entry(ordprod).State = System.Data.Entity.EntityState.Modified;
                            MessageBox.Show("Товар успешно добавлен в корзину!", "Внимание");
                            return;
                        }
                    }
                    db.OrderProduct.Add(new OrderProduct() { OrderID = _currentOrder.OrderID, Count = 1, ProductID = product.ProductID });
                    db.SaveChanges();
                    MessageBox.Show("Товар успешно добавлен в корзину!", "Внимание");
                    return;
                }
                if (foundOrder == true)
                {
                    foreach (var ordprod in dbmodel.OrderProduct.ToList())
                    {
                        if (ordprod.ProductID == product.ProductID && ordprod.OrderID == _currentOrder.OrderID)
                        {
                            ordprod.Count++;
                            db.Entry(ordprod).State = EntityState.Modified;
                            db.SaveChanges();
                            MessageBox.Show("Товар успешно добавлен в корзину!", "Внимание");
                            return;
                        }
                    }

                    db.OrderProduct.Add(new OrderProduct() { OrderID = _currentOrder.OrderID, Count = 1, ProductID = product.ProductID });
                    db.SaveChanges();
                    MessageBox.Show("Товар успешно добавлен в корзину!", "Внимание");
                    return;
                }

                ///Проверка на авторизованность пользователя для передачи последующей передачи null
                int? userId = _currentUser.UserID;
                if (_currentUser.UserID == 0)
                    userId = null;

                order = new Order()
                {
                    OrderCreateDate = DateTime.Now,
                    OrderStatusID = 1,
                    UserID = userId,
                    OrderGetCode = db.Order.OrderByDescending(o => o.OrderGetCode).First().OrderGetCode + 1,
                    PickupPointID = 1,
                    OrderDeliveryDate = DateTime.Now
                };
                db.Order.Add(order);
                db.SaveChanges();

                var orderProduct = new OrderProduct()
                {
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    Count = 1
                };
                db.OrderProduct.Add(orderProduct);
                db.SaveChanges();

                _currentOrder = order;
                _orderExists = true;
                MessageBox.Show("Заказ создан, товар успешно добавлен!", "Внимание");
                return;
            }
        }
    }
}
