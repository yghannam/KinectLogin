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

namespace KinectLogin
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            KinectManager.getKinectHelper().DepthImageUpdated += helper_DepthImageUpdated;
        }

        private void helper_DepthImageUpdated(object sender, EventArgs e)
        {
            this.depthImage.Source = KinectManager.getKinectHelper().outputBitmap;

            // TO-DO Add this for the login
            /*
            if (gestureSet.getGestures() != null && gestureSet.getGestures().Length >= 2)
            {
                gestureSet.compare(gestureSet.getGestures()[0], gestureSet.getGestures()[1]);
            }
            */
        }
    }
}
