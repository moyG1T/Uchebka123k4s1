using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Uchebka123k4s1.Views
{
    /// <summary>
    /// Логика взаимодействия для HardwareInteractionView.xaml
    /// </summary>
    public partial class HardwareInteractionView : UserControl
    {
        private readonly Regex _onlyDigits = new Regex("[^0-9]+");
        public HardwareInteractionView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _onlyDigits.IsMatch(e.Text);
        }
    }
}
