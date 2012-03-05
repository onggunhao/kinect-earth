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

    class CustomController : SkeletonController
    {
        private MainWindow window;

        // Variables to keep "time"
        public int selectCount = 0;
        int forwardCount = 0;

        public CustomController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            
            if (targets[4].isSelected())
            {
                // Birdwatcher Menu (targets 4, 5) is displayed
                Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);

                //Calculate how far our right hand is from target 5 in both x and y directions
                double deltaX_right = Math.Abs(rightHand.Position.X - targets[5].getXPosition());
                double deltaY_right = Math.Abs(rightHand.Position.Y - targets[5].getYPosition());

                //If we have a hit in a reasonable range, highlight the target5
                if (deltaX_right < 20 && deltaY_right < 20)
                {
                    targets[5].setTargetSelected();
                    selectCount++;
                    if (selectCount > 10)
                    {
                        // target 5 selected, clear menu and return to Navigation (Targets 1,2,3)
                        // hide Targets 4,5
                        targets[4].setTargetUnselected();
                        targets[4].hideTarget();
                        targets[5].setTargetUnselected();
                        targets[5].hideTarget();

                        // show Targets 1,2,3
                        //targets[1].setTargetUnselected();
                        //targets[2].setTargetUnselected();
                        //targets[3].setTargetUnselected();
                        targets[1].showTarget();
                        targets[2].showTarget();
                        targets[3].showTarget();
                        targets[6].showTarget();
                        targets[7].showTarget();

                        // reset the count
                        selectCount = 0;
                    }
                }
                else
                {
                    targets[5].setTargetUnselected();
                    selectCount = 0; // reset the count
                }
            }
            else
            {
                // Birdwatcher Menu is NOT up. Detect Navigation.
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
                    // move forward (highlight 1)
                    targets[2].setTargetUnselected();
                    targets[3].setTargetUnselected();
                    targets[1].setTargetSelected();
                    //InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W); // regular
                    
                    if (feetDifferential > 0.8)
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

                // Detect left-right shoulder navigation
                // Gets right shoulder position
                Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
                Point rightNav = new Point(rightShoulder.Position.X, rightShoulder.Position.Z);

                Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];
                Point leftNav = new Point(leftShoulder.Position.X, leftShoulder.Position.Z);

                double shoulderDifferential = leftNav.Y - rightNav.Y;

                if (shoulderDifferential > 0.05)       // Right
                {
                    targets[6].setTargetSelected();
                    targets[7].setTargetUnselected();
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                }
                else if (shoulderDifferential < -0.05)       // Left
                {
                    targets[6].setTargetUnselected();
                    targets[7].setTargetSelected();
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                }
                else
                {
                    targets[6].setTargetUnselected();
                    targets[7].setTargetUnselected();
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                }

                // Detect Birdwatcher Gesture
                // Get head position
                Point headPosition;
                Joint head = skeleton.Joints[JointID.Head];
                headPosition = new Point(head.Position.X, head.Position.Y);

                // Get right hand position
                Point rightHandPosition;
                Joint rightHand = skeleton.Joints[JointID.HandRight];
                rightHandPosition = new Point(rightHand.Position.X, rightHand.Position.Y);

                 Point rightElbowPosition;
                Joint rightElbow = skeleton.Joints[JointID.ElbowRight];
                rightElbowPosition = new Point(rightElbow.Position.X, rightElbow.Position.Y);

                // Get right shoulder position
                Point rightShoulderPosition;
                //Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
                rightShoulderPosition = new Point(rightShoulder.Position.X, rightShoulder.Position.Y);
                
                //Calculate how far our right hand is from target 5 in both x and y directions
                double deltaX = Math.Abs(rightHandPosition.X - headPosition.X);
                double deltaY = Math.Abs(rightHandPosition.Y - headPosition.Y);

                double headelbowXDifferential = rightElbowPosition.X - headPosition.X;

                // y increases towards top
                // hand > elbow > shoulder

                if (deltaY < 0.03
                        && rightHandPosition.Y > rightElbowPosition.Y
                        && rightElbowPosition.Y > rightShoulderPosition.Y
                        && deltaX < 0.3
                        && headelbowXDifferential > 0.2)
                {
                    // Birdwatcher! (highlight 4 and show 5)
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_Y); // show augmented reality
                    targets[4].showTarget();
                    targets[4].setTargetSelected();
                    targets[5].showTarget();

                    // hide navigation targets 1,2,3
                    targets[1].setTargetUnselected();
                    targets[2].setTargetUnselected();
                    targets[3].setTargetUnselected();
                    targets[1].hideTarget();
                    targets[2].hideTarget();
                    targets[3].hideTarget();
                    targets[6].setTargetUnselected();
                    targets[7].setTargetUnselected();
                    targets[6].hideTarget();
                    targets[7].hideTarget();
                }
            }
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
            //targets[1].setTargetUnselected();
            //targets[1].showTarget();
            //targets[1].setTargetPosition(262, 30);
            //targets[2].setTargetUnselected();
            //targets[2].showTarget();
            //targets[2].setTargetPosition(262, 150);
            //targets[3].setTargetUnselected();
            //targets[3].showTarget();
            //targets[3].setTargetPosition(262, 270);
            //targets[4].setTargetUnselected();
            //targets[4].hideTarget();
            //targets[4].setTargetPosition(400, 30);
            //targets[5].setTargetUnselected();
            //targets[5].hideTarget();
            //targets[5].setTargetPosition(400, 150);

            //// show left-right directional targets
            //targets[6].setTargetUnselected();
            //targets[6].showTarget();
            //targets[7].setTargetUnselected();
            //targets[7].showTarget();
        }
    }
}
