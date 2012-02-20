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
    class CustomController2 : SkeletonController
    {
        private MainWindow window;

        // Variables to hold store hand positions
        public Point curHandPoint, lastHandPoint;
        public int leftCount = 0;

        public CustomController2(MainWindow win) 
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            //Scale the joints to the size of the window
            Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
            Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

            // Selecting "container" or "clutch" (i.e. target 1)
            if (targets[1].isSelected())
            {
                targets[1].setTargetPosition(leftHand.Position.X - 40, leftHand.Position.Y - 40);
            }
            else  // else, check whether 1 is selected
            {
                //Calculate how far our left hand is from the target in both x and y directions
                double deltaX_left = Math.Abs(leftHand.Position.X - targets[1].getXPosition());
                double deltaY_left = Math.Abs(leftHand.Position.Y - targets[1].getYPosition());
                if (deltaX_left < 15 && deltaY_left < 15)
                {
                    targets[1].setTargetSelected(); // select target 1 (container)

                    //show other targets (objects)
                    targets[2].showTarget();
                    targets[3].showTarget();
                    targets[4].showTarget();
                    targets[5].showTarget();
                }
            }

            // Selecting "files" or "photos" (i.e. targets 2-5)
            foreach (var target in targets)
            {
                Target cur = target.Value;
                int targetID = cur.id; //ID in range [1..5]

                if (targetID != 1)
                {
                    //Calculate how far our right hand is from the target in both x and y directions
                    double deltaX_right = Math.Abs(rightHand.Position.X - cur.getXPosition());
                    double deltaY_right = Math.Abs(rightHand.Position.Y - cur.getYPosition());

                    if (deltaX_right < 15 && deltaY_right < 15)
                    {
                        cur.setTargetSelected();
                    }
                }
            }

            // Tracks the swipe gesture to "disappear" selected items
            Point handPosition;
            Joint handJoint = skeleton.Joints[JointID.HandRight];
            handPosition = new Point(handJoint.Position.X, handJoint.Position.Y);

            if (targets[1].isSelected())
            {
                if (lastHandPoint == null) lastHandPoint = handPosition;
                curHandPoint = handPosition;

                if (curHandPoint.X - lastHandPoint.X < 0)
                {
                    leftCount++; //swipe left
                }

                if (leftCount > 35)
                {
                    // swipe left
                    for (int j = 2; j <= 5; j++)
                    {
                        if (targets[j].isSelected())
                        {
                            targets[j].hideTarget();
                        }
                    }
                    leftCount = 0;  // Reset after the swipe
                }
            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            adjustScale(1.1f);
            targets[1].setTargetPosition(140, 200); //set "container" start position
            targets[2].hideTarget();
            targets[3].hideTarget();
            targets[4].hideTarget();
            targets[5].hideTarget();

            targets[2].setTargetPosition(350, 80);
            targets[3].setTargetPosition(380, 150);
            targets[4].setTargetPosition(380, 225);
            targets[5].setTargetPosition(340, 290);

            // hide left, right
            targets[6].hideTarget();
            targets[7].hideTarget();
        }
    }
}
