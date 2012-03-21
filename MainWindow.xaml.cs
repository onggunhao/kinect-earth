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
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.Web;
using System.Web.Services;
using WindowsInput;
using System.Timers;

namespace SkeletalTracking
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Make sure to change this path to your index2.html file
            Browser.Navigate(new Uri("C:/Users/n00b/Downloads/cs247-prototype/index2.html"));
            //Browser.Navigate(new Uri("C:/Users/Huyen Tran/Desktop/kinect-earth/index2.html"));
            Keyboard.Focus(Browser);
        }

        //Kinect Runtime
        Runtime nui;

        //Gesture controller
        SuperController gestureController;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupKinect();
            gestureController = new SuperController(this);
        }

        private void SetupKinect()
        {
            if (Runtime.Kinects.Count == 0)
            {
                this.Title = "No Kinect connected"; 
            }
            else
            {
                //use first Kinect
                nui = Runtime.Kinects[0];

                //Initialize to do skeletal tracking
                nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex);

                //add event to receive skeleton data
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                //to experiment, toggle TransformSmooth between true & false and play with parameters            
                nui.SkeletonEngine.TransformSmooth = true;
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                // parameters used to smooth the skeleton data
                parameters.Smoothing = 0.3f;
                parameters.Correction = 0.3f;
                parameters.Prediction = 0.4f;
                parameters.JitterRadius = 0.7f;
                parameters.MaxDeviationRadius = 0.2f;
                nui.SkeletonEngine.SmoothParameters = parameters;

            }
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();

            if(skeleton != null)
            {
                gestureController.processSkeletonFrame(skeleton);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Uninitialize();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //key to manually switch banner to paris (key 9 to switch location to paris in Google Earth)
            if (e.Key == Key.D1)
            {
                bannerSF.Visibility = Visibility.Hidden;
                bannerParis.Visibility = Visibility.Visible;
            }

            //key to manually switch banner to San Francisco (key 0 to switch location in Google Earth)
            if (e.Key == Key.D2)
            {
                bannerSF.Visibility = Visibility.Visible;
                bannerParis.Visibility = Visibility.Hidden;
            }
        }
    }


}
