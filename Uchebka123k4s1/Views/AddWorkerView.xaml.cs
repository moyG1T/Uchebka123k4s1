using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Uchebka123k4s1.Views
{
    /// <summary>
    /// Логика взаимодействия для AddWorkerView.xaml
    /// </summary>
    public partial class AddWorkerView : UserControl
    {
        private readonly Regex _onlyDigits = new Regex("[^0-9]+");
        public AddWorkerView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _onlyDigits.IsMatch(e.Text);
        }
    }
}
