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

    class HuyenController : SkeletonController
    {
        private MainWindow window;

        // Variables to keep "time"
        public int menuCount = 0;
        int menu = 0;

        public HuyenController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
           

        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            targets[1].setTargetUnselected();
            targets[1].showTarget();
            targets[1].setTargetPosition(262, 30);
            targets[2].setTargetUnselected();
            targets[2].showTarget();
            targets[2].setTargetPosition(262, 150);
            targets[3].setTargetUnselected();
            targets[3].showTarget();
            targets[3].setTargetPosition(262, 270);
            targets[4].setTargetUnselected();
            targets[4].hideTarget();
            targets[4].setTargetPosition(400, 30);
            targets[5].setTargetUnselected();
            targets[5].hideTarget();
            targets[5].setTargetPosition(400, 150);

            // show left-right directional targets
            targets[6].setTargetUnselected();
            targets[6].showTarget();
            targets[7].setTargetUnselected();
            targets[7].showTarget();
        }
    }
}
