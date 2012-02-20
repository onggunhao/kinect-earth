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
using System.Timers;

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
            Browser.Navigate(new Uri("C:/Users/n00b/Downloads/CS247/CS247/p4-functional-prototype-1/index2.html"));
            Keyboard.Focus(Browser);
        }
    
        private void BrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventsArgs)
        {
          
        }

        private void Button_W_Click(object sender, RoutedEventArgs e)
        {
            //InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W);
            
            //InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);

            for (int i = 1; i <= 2; i++)
            {
                Keyboard.Focus(Browser);


                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W);

                //InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
            }
           
            
        }


            

    
    }
}
