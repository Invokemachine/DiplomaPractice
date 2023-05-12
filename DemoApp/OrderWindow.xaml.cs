using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        User _currentUser;
        Product _currentProduct;
        Order _currentOrder;
        OrderProduct _currentOrderProduct;
        List<OrderProduct> _currentOrderProducts = new List<OrderProduct>();

        user25Entities1 dbmodel = new user25Entities1();

        decimal orderSum = 0;

        public OrderWindow(User user, Order order)
        {
            InitializeComponent();
            _currentUser = user;
            _currentOrder = order;
            LoadComboBoxes();

            if (order != null)
            {
                _currentOrder = order;
            }
            InitProductOrder();
            InitList();
        }
        private void LoadComboBoxes()
        {
            using (var db = new user25Entities1())
            {
                List<PickupPoint> pickupPoints;
                pickupPoints = (from pp in db.PickupPoint select pp).ToList<PickupPoint>();
                foreach (var pickupPoint in pickupPoints)
                    PointOfIssueComboBox.Items.Add(pickupPoint.Address);
            }
        }

        private void InitList()
        {
            productsList.ItemsSource = _currentOrderProducts;
            productsList.Items.Refresh();
            Configure();
        }

        private void PointOfIssueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentOrder.PickupPointID = (from pp in dbmodel.PickupPoint where pp.Address == PointOfIssueComboBox.SelectedValue select pp.PickupPointID).FirstOrDefault();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            using (var db = new user25Entities1())
            {
                if (_currentOrderProduct.Count > 0)
                {
                    _currentOrderProduct = button.DataContext as OrderProduct;
                    _currentOrderProduct.Count--;
                    db.Order.AddOrUpdate(_currentOrder);
                    db.SaveChanges();
                    InitProductOrder();
                    InitList();
                }
                else
                {
                    _currentOrderProduct.Count = 0;
                    db.Order.AddOrUpdate(_currentOrder);
                    db.SaveChanges();
                    InitProductOrder();
                    InitList();
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities1())
            {
                Button button = (Button)sender;
                _currentOrderProduct = button.DataContext as OrderProduct;
                _currentOrderProduct.Count++;
                db.Order.AddOrUpdate(_currentOrder);
                db.SaveChanges();
                InitProductOrder();
                InitList();
            }
        }

        private void InitProductOrder()
        {
            var orderprod = new OrderProduct();
            _currentOrderProducts.Clear();
            foreach (var prod in dbmodel.OrderProduct)
            {
                if (prod.OrderID == _currentOrder.OrderID)
                {
                    orderprod = prod;
                    orderprod.Order = _currentOrder;
                    orderprod.Product = GetProductById(orderprod.ProductID);

                    _currentOrderProducts.Add(orderprod);
                }
            }
        }
        private Product GetProductById(int prodId)
        {
            foreach (var prod in dbmodel.Product)
            {
                if (prod.ProductID == prodId)
                {
                    _currentProduct = prod;
                    return prod;
                }
            }
            return null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow(_currentUser, _currentOrder, true);
            clientWindow.Show();
            Close();
        }

        private void Configure()
        {
            var itemsInStock = 0;
            foreach (var item in _currentOrderProducts)
            {
                if (item.Product.ProductQuantityInStock > 3)
                {
                    itemsInStock++;
                }
            }
            if (itemsInStock == _currentOrderProducts.Count)
            {
                _currentOrder.OrderDeliveryDate = _currentOrder.OrderCreateDate.AddDays(3);
            }
            else
            {
                _currentOrder.OrderDeliveryDate = _currentOrder.OrderCreateDate.AddDays(6);
            }
            using (var db = new user25Entities1())
            {
                db.Order.AddOrUpdate(_currentOrder);
                db.SaveChanges();
            }

            Date.Content = _currentOrder.OrderDeliveryDate;
            orderSum = 0;

            foreach (var item in _currentOrderProducts)
            {
                foreach (var product in dbmodel.Product)
                {
                    if (item.ProductID == product.ProductID)
                    {
                        orderSum += (decimal)((double)product.ProductCost * ((100 - (double)product.ProductDiscountAmount) / 100) * item.Count);
                    }
                }
            }
            TotalLabel.Content = "Сумма заказа: " + orderSum.ToString();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new user25Entities1())
            {
                if (PointOfIssueComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Не выбран пункт выдачи!", "Ошибка");
                    return;
                }
                _currentOrder.OrderStatusID = 2;
                db.Order.AddOrUpdate(_currentOrder);
                db.SaveChanges();
                MessageBox.Show("Заказ подтвержден!");
                return;
            }
        }
    }
}
