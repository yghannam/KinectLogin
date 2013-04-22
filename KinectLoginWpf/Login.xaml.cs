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
using Gestures;

namespace KinectLoginWpf
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        KinectHelper helper;

        public Login()
        {
            InitializeComponent();

            helper = new KinectHelper();
            helper.StartKinectST();

            helper.DepthImageUpdated += helper_DepthImageUpdated;
        }

        private void helper_DepthImageUpdated(object sender, EventArgs e)
        {
            this.depthImage.Source = helper.outputBitmap;
        }
    }
}
