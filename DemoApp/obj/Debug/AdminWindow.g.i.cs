// Updated by XamlIntelliSenseFileGenerator 04.05.2023 10:52:22
#pragma checksum "..\..\AdminWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3C262B1C6B35DFC5F96DC1B33427D4F7CF61267A5CAE35FF7F045722CB6DEB7D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using DemoApp;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace DemoApp
{


    /// <summary>
    /// AdminWindow
    /// </summary>
    public partial class AdminWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DemoApp;component/adminwindow.xaml", System.UriKind.Relative);

#line 1 "..\..\AdminWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.ComboBox DiscountFilter;
        internal System.Windows.Controls.ComboBox PriceFilter;
        internal System.Windows.Controls.TextBox SearchTextBox;
        internal System.Windows.Controls.Button BackButton;
        internal System.Windows.Controls.Label recordAmountLabel;
        internal System.Windows.Controls.ListView ProductsList;
        internal System.Windows.Controls.TextBox ArticleTextBox;
        internal System.Windows.Controls.TextBox NameTextBox;
        internal System.Windows.Controls.TextBox UnitTypeIdTextBox;
        internal System.Windows.Controls.TextBox PriceTextBox;
        internal System.Windows.Controls.TextBox MaxDiscountTextBox;
        internal System.Windows.Controls.TextBox ManufacturerIdTextBox;
        internal System.Windows.Controls.TextBox SupplierIdTextBox;
        internal System.Windows.Controls.TextBox CategoryIdTextBox;
        internal System.Windows.Controls.TextBox DiscountTextBox;
        internal System.Windows.Controls.TextBox AmountInStockTextBox;
        internal System.Windows.Controls.TextBox DescriptionTextBox;
        internal System.Windows.Controls.Button ImageButoon;
    }
}

