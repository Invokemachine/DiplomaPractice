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
using System.Xml.Linq;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Paragraph = Spire.Doc.Documents.Paragraph;
using Section = Spire.Doc.Section;
using TextRange = Spire.Doc.Fields.TextRange;

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
        /// <summary>
        /// Загрузка пунктов выдачи в комбобокс
        /// </summary>
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
        /// <summary>
        /// Метод инициализации списка добавленных в заказ продуктов
        /// </summary>
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
        /// <summary>
        /// Удаление 1 элемента выбранного продукта в заказе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Добавление +1 элемента в заказе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Инициализация выбранного продукта в заказе
        /// </summary>
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
        /// <summary>
        /// Конфигурация заказа
        /// </summary>
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
            ///Добавление времени к дате доставке
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
            ///Счёт суммы заказа с учётом скидок
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
        /// <summary>
        /// Подтверждение заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Создание пдф-чека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            ///Создание нового документа
            Document document = new Document();
            Section section = document.AddSection();
            Spire.Doc.Documents.Paragraph par = section.AddParagraph();
            ///Если пользователь не авторизован
            if (_currentUser.UserID == 0)
            {
                par.Text = "Guest";
            }
            else
            {
                par.Text = _currentUser.UserName;
            }
            Spire.Doc.Documents.Paragraph par1 = section.AddParagraph();
            TextRange text = par1.AppendText($"Код получения заказа: {_currentOrder.OrderGetCode}");
            text.CharacterFormat.FontSize = 20;
            text.CharacterFormat.Bold = true;
            Spire.Doc.Documents.Paragraph par2 = section.AddParagraph();
            par2.Text = $"Дата заказа: {_currentOrder.OrderCreateDate}\n" +
                $"Сумма заказа: {orderSum}\n";
            Paragraph par3 = section.AddParagraph();
            par3.Text = "Состав заказа: \n";
            foreach (var item in _currentOrderProducts)
            {
                par3.Text += $"{item.Product.ProductName} : {item.Count}\n" + "\n";
            }
            document.SaveToFile("TheBill.pdf", FileFormat.PDF);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "TheBill.pdf", UseShellExecute = true });
        }
    }
}
