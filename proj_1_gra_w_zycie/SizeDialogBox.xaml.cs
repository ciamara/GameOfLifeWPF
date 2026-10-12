using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace proj_1_gra_w_zycie
{
    /// <summary>
    /// Interaction logic for SizeDialogBox.xaml
    /// </summary>
    public partial class SizeDialogBox : Window
    {
        public int BoardSize { get; private set; } = 50;

        public SizeDialogBox()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            BoardSize = Convert.ToInt32(SizeSlider.Value);
            DialogResult = true;
        }
    }
}