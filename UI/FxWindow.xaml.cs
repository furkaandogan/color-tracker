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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for FxWindow.xaml
    /// </summary>
    public partial class FxWindow : UserControl
    {
        public FxWindow()
        {
            InitializeComponent();
        }

        #region Fied

        private Window _window;

        #endregion

        #region Property

        public Window Window
        {
            set
            {
                _window = value;
            }
        }
        public string Title
        {
            set
            {
                TitleTxtBlock.Text = value;
            }
        }
        public string About
        {
            set
            {
                AboutTxtBlock.Text = value;
            }
        }


        #endregion

        private void Close(object sender, RoutedEventArgs e)
        {
            _window.Close();
        }

        private void Down(object sender, MouseButtonEventArgs e)
        {
        }

    }
}
