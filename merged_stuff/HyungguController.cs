using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using WindowsInput;

namespace SkeletalTracking
{

    class HyungguController : SkeletonController
    {
        private MainWindow window;

        public HyungguController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            // gesture recognition

        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            // setup targets
        }
    }
}
