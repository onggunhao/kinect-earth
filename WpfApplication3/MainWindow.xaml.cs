using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using System.Web.Services;
using WindowsInput;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Browser.LoadCompleted += BrowserOnLoadCompleted;
            Browser.Navigate(new Uri("C:/Users/Daniel/Documents/My Dropbox/2011-2012/Academics/Winter/CS247/p4-functional-prototype-1/index2.html"));
        }
    
        private void BrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventsArgs)
        {
            // To be filled out
        }




    
    }
}
