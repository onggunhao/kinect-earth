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

        // Constant Variables
        int LEFTSIDE   = 6;
        int RIGHTSIDE  = 7;
        double THRESH = 0.1;

        public HyungguController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
                // Detect Navigation.
                // Get right foot position
                Point rightFootPosition;
                Joint rightFoot = skeleton.Joints[JointID.FootRight];
                rightFootPosition = new Point(rightFoot.Position.X, rightFoot.Position.Z);

                // Get left foot position
                Point leftFootPosition;
                Joint leftFoot = skeleton.Joints[JointID.FootLeft];
                leftFootPosition = new Point(leftFoot.Position.X, leftFoot.Position.Z);

                double feetDifferential = leftFootPosition.Y - rightFootPosition.Y;

                if (feetDifferential > 0.1)
                {
                    // move forward (select 1)
                    targets[2].setTargetUnselected();
                    targets[3].setTargetUnselected();
                    targets[1].setTargetSelected();

                    if (feetDifferential > 0.35)
                    {
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_2); // faster
                    }
                    else
                    {
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W); // regular
                    }

                }
                else if (feetDifferential < -0.1)
                {
                    // move backward (select 3)
                    targets[1].setTargetUnselected();
                    targets[2].setTargetUnselected();
                    targets[3].setTargetSelected();
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_S);
                }
                else
                {
                    // stay put (select 2)
                    targets[1].setTargetUnselected();
                    targets[3].setTargetUnselected();
                    targets[2].setTargetSelected();
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                }

            // gesture recognition by Hyunggu 02/24/2012
            // Detect left-right hand navigation
            // Get right hand position
            Joint HandRight = skeleton.Joints[JointID.HandRight];
            Joint ElbowRight = skeleton.Joints[JointID.ElbowRight];

            Point HandRightPoint = new Point(HandRight.Position.X, HandRight.Position.Y);
            Point ElbowRightPoint = new Point(ElbowRight.Position.X, ElbowRight.Position.Y);
           
            // Get shoulder position
            Joint RightShoulder = skeleton.Joints[JointID.ShoulderRight];
            Point RightShoulderPoint = new Point(RightShoulder.Position.X, RightShoulder.Position.Y);

            

            double diffHandShoulderRight = Math.Abs(HandRightPoint.Y - RightShoulderPoint.Y);

            // Gets left hand position
            Joint HandLeft = skeleton.Joints[JointID.HandLeft];
            Joint ElbowLeft = skeleton.Joints[JointID.ElbowLeft];

            Point HandLeftPoint = new Point(HandLeft.Position.X, HandLeft.Position.Y);
            Point ElbowLeftPoint = new Point(ElbowLeft.Position.X, ElbowLeft.Position.Y);

            Joint LeftShoulder = skeleton.Joints[JointID.ShoulderLeft];
            Point LeftShoulderPoint = new Point(LeftShoulder.Position.X, LeftShoulder.Position.Y);

            double diffHandShoulderLeft = Math.Abs(HandLeftPoint.Y - LeftShoulderPoint.Y);

            // right hand
            if (diffHandShoulderRight < THRESH && HandRightPoint.X > ElbowRightPoint.X) 
            {
                targets[LEFTSIDE].setTargetUnselected();
                targets[RIGHTSIDE].setTargetSelected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
            }
            // left hand
            else if (diffHandShoulderLeft < THRESH && HandLeftPoint.X < ElbowLeftPoint.X)
            {
                targets[LEFTSIDE].setTargetSelected();
                targets[RIGHTSIDE].setTargetUnselected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
            }
            // neither right nor left hand
            else
            {
                targets[LEFTSIDE].setTargetUnselected();
                targets[RIGHTSIDE].setTargetUnselected();
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            // setup targets
            /*
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
            targets[LEFTSIDE].setTargetUnselected();
            targets[LEFTSIDE].showTarget();
            targets[RIGHTSIDE].setTargetUnselected();
            targets[RIGHTSIDE].showTarget();
             */
        }
    }
}
