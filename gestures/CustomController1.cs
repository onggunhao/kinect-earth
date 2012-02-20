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

namespace SkeletalTracking
{

    // Controller 1: Shoulder-tracking of user orientation
    class CustomController1 : SkeletonController
    {
        private MainWindow window;

        public CustomController1(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            // Gets right shoulder position
            Point rightShoulderPosition;
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
            rightShoulderPosition = new Point(rightShoulder.Position.X, rightShoulder.Position.Z);

            Point leftShoulderPosition;
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];
            leftShoulderPosition = new Point(leftShoulder.Position.X, leftShoulder.Position.Z);

            double shoulderDifferential = leftShoulderPosition.Y - rightShoulderPosition.Y;

            if (shoulderDifferential < -0.08)       // Rightmost (i.e. 5)
            {
                targets[1].setTargetUnselected();
                targets[2].setTargetUnselected();
                targets[3].setTargetUnselected();
                targets[4].setTargetUnselected();
                targets[5].setTargetSelected();

            }
            else if (shoulderDifferential < -0.025 && shoulderDifferential > -0.08)   // "4"
            {
                targets[1].setTargetUnselected();
                targets[2].setTargetUnselected();
                targets[3].setTargetUnselected();
                targets[4].setTargetSelected();
                targets[5].setTargetUnselected();

            }
            else if (shoulderDifferential > -0.025 && shoulderDifferential < 0.025)   // "3"
            {
                targets[1].setTargetUnselected();
                targets[2].setTargetUnselected();
                targets[3].setTargetSelected();
                targets[4].setTargetUnselected();
                targets[5].setTargetUnselected();

            }
            else if (shoulderDifferential > 0.025 && shoulderDifferential < 0.08)    // "2" 
            {
                targets[1].setTargetUnselected();
                targets[2].setTargetSelected();
                targets[3].setTargetUnselected();
                targets[4].setTargetUnselected();
                targets[5].setTargetUnselected();
            }
            else if (shoulderDifferential > 0.08)       // "1" or leftmost
            {
                targets[1].setTargetSelected();
                targets[2].setTargetUnselected();
                targets[3].setTargetUnselected();
                targets[4].setTargetUnselected();
                targets[5].setTargetUnselected();

            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            adjustScale(1.1f);
            targets[1].setTargetUnselected();
            targets[1].showTarget();
            targets[1].setTargetPosition(23, 220);
            targets[2].setTargetUnselected();
            targets[2].showTarget();
            targets[2].setTargetPosition(111, 96);
            targets[3].setTargetUnselected();
            targets[3].showTarget();
            targets[3].setTargetPosition(262, 33);
            targets[4].setTargetUnselected();
            targets[4].showTarget();
            targets[4].setTargetPosition(409, 96);
            targets[5].setTargetUnselected();
            targets[5].showTarget();
            targets[5].setTargetPosition(505, 220);
            
            // hide left, right
            targets[6].hideTarget();
            targets[7].hideTarget();
        }
    }
}
