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

    class SuperController : SkeletonController
    {
        private MainWindow window;

        // Variables to keep "time"
        public int selectCount = 0;
        int forwardCount = 0;

        Boolean AugmentedRealityOn = false;
        double THRESH = 0.3;
        double JetPackThresh = 0.1;

        public SuperController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            // Detect walking navigation.
            detectWalking(skeleton, targets);

            // Detect arm turning navigation (Hyunggu 02/24/2012)
            if (!detectArmTurning(skeleton, targets))
            {
                // Detect left-right shoulder turning navigation
                if (!detectShoulderTurning(skeleton, targets))
                {
                    // Detect JetPackUP
                    detectJetPackUp(skeleton, targets);
                }
            }

            // Detect Birdwatcher Gesture
            detectBirdwatcher(skeleton, targets);
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

        }

        private void detectWalking(SkeletonData skeleton, Dictionary<int, Target> targets) {
            // Get right foot position
            Point rightFootPosition;
            Joint rightFoot = skeleton.Joints[JointID.FootRight];
            rightFootPosition = new Point(rightFoot.Position.X, rightFoot.Position.Z);

            // Get left foot position
            Point leftFootPosition;
            Joint leftFoot = skeleton.Joints[JointID.FootLeft];
            leftFootPosition = new Point(leftFoot.Position.X, leftFoot.Position.Z);

            double feetDifferential = leftFootPosition.Y - rightFootPosition.Y;

            // Move front
            if (feetDifferential > 0.1)
            {
                // move forward slow (highlight 1 = middle)
                targets[2].setTargetUnselected();
                targets[3].setTargetUnselected();
                targets[1].setTargetHighlighted();

                if (feetDifferential > 0.5)
                {
                    // move forward fast (select 1)
                    targets[1].setTargetSelected();
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_2); // faster
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W); // regular
                }

            }
            // Move backward
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
        }

        private Boolean detectShoulderTurning(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            // Gets right shoulder position
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
            Point rightNav = new Point(rightShoulder.Position.X, rightShoulder.Position.Z);

            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];
            Point leftNav = new Point(leftShoulder.Position.X, leftShoulder.Position.Z);

            double shoulderDifferential = leftNav.Y - rightNav.Y;

            if (shoulderDifferential > 0.08)
            {
                targets[6].setTargetHighlighted();
                targets[7].setTargetUnselected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                return true;
            }
            else if (shoulderDifferential < -0.08)
            {
                targets[6].setTargetUnselected();
                targets[7].setTargetHighlighted();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                return true;
            }
            else
            {
                targets[6].setTargetUnselected();
                targets[7].setTargetUnselected();
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                return false;
            }
        }

        private Boolean detectArmTurning(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            // Get right hand position
            Joint HandRight = skeleton.Joints[JointID.HandRight];
            Joint ElbowRight = skeleton.Joints[JointID.ElbowRight];

            Point HandRightPoint = new Point(HandRight.Position.X, HandRight.Position.Y);
            Point ElbowRightPoint = new Point(ElbowRight.Position.X, ElbowRight.Position.Y);

            // Get shoulder position
            //Joint RightShoulder = skeleton.Joints[JointID.ShoulderRight];
            //Point RightShoulderPoint = new Point(RightShoulder.Position.X, RightShoulder.Position.Y);

            //Get shoulder center
            Joint CenterShoulder = skeleton.Joints[JointID.ShoulderCenter];
            Point CenterShoulderPoint = new Point(CenterShoulder.Position.X, CenterShoulder.Position.Y);

            double diffHandShoulderRight = Math.Abs(HandRightPoint.Y - CenterShoulderPoint.Y);

            // Gets left hand position
            Joint HandLeft = skeleton.Joints[JointID.HandLeft];
            Joint ElbowLeft = skeleton.Joints[JointID.ElbowLeft];

            Point HandLeftPoint = new Point(HandLeft.Position.X, HandLeft.Position.Y);
            Point ElbowLeftPoint = new Point(ElbowLeft.Position.X, ElbowLeft.Position.Y);

            //Joint LeftShoulder = skeleton.Joints[JointID.ShoulderLeft];
            //Point LeftShoulderPoint = new Point(LeftShoulder.Position.X, LeftShoulder.Position.Y);

            double diffHandShoulderLeft = Math.Abs(HandLeftPoint.Y - CenterShoulderPoint.Y);

            // right hand
            if (diffHandShoulderRight < THRESH && HandRightPoint.X > ElbowRightPoint.X)
            {
                targets[6].setTargetUnselected();
                targets[7].setTargetSelected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_PERIOD);
                return true;
            }
            // left hand
            else if (diffHandShoulderLeft < THRESH && HandLeftPoint.X < ElbowLeftPoint.X)
            {
                targets[6].setTargetSelected();
                targets[7].setTargetUnselected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_COMMA);
                return true;
            }
            // neither right nor left hand
            else
            {
                targets[6].setTargetUnselected();
                targets[7].setTargetUnselected();
                InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_PERIOD);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_COMMA);
                return false;
            }
        }

        private void detectBirdwatcher(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
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
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
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
                AugmentedRealityOn = true;
                targets[4].setTargetSelected();
            }
            else
            {
                if (AugmentedRealityOn)
                {
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_Y);
                    targets[4].setTargetUnselected();
                    AugmentedRealityOn = false;
                }
            }
        }

        private void detectJetPackUp(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            targets[4].setTargetSelected();

            // right side
            Point rightHandPosition, rightElbowPosition, rightShoulderPosition;
            Joint rightHand = skeleton.Joints[JointID.HandRight];
            rightHandPosition = new Point(rightHand.Position.X, rightHand.Position.Y);
            Joint rightElbow = skeleton.Joints[JointID.ElbowRight];
            rightElbowPosition = new Point(rightElbow.Position.X, rightElbow.Position.Y);
            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
            rightShoulderPosition = new Point(rightShoulder.Position.X, rightShoulder.Position.Y);

            // left side
            Point leftHandPosition, leftElbowPosition, leftShoulderPosition;
            Joint leftHand = skeleton.Joints[JointID.HandLeft];
            leftHandPosition = new Point(leftHand.Position.X, leftHand.Position.Y);
            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft];
            leftElbowPosition = new Point(leftElbow.Position.X, leftElbow.Position.Y);
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];
            leftShoulderPosition = new Point(leftShoulder.Position.X, leftShoulder.Position.Y);

            // shoulder elbow x difference
            // shoulder hand x difference
            double rightShoulderElbowDiffX = Math.Abs(rightShoulderPosition.X - rightElbowPosition.X);
            double rightShoulderHandDiffX = Math.Abs(rightShoulderPosition.X - rightHandPosition.X);

            // elbow hand y difference
            double rightElbowHandDiffY = Math.Abs(rightElbowPosition.Y - rightHandPosition.Y);

            // left side differences
            double leftShoulderElbowDiffX = Math.Abs(leftShoulderPosition.X - leftElbowPosition.X);
            double leftShoulderHandDiffX = Math.Abs(leftShoulderPosition.X - leftHandPosition.X);

            // elbow hand y difference
            double leftElbowHandDiffY = Math.Abs(leftElbowPosition.Y - leftHandPosition.Y);

            if (rightShoulderElbowDiffX < JetPackThresh
                && rightShoulderHandDiffX < JetPackThresh
                && rightElbowHandDiffY < JetPackThresh
                && leftShoulderElbowDiffX < JetPackThresh
                && leftShoulderHandDiffX < JetPackThresh
                && leftElbowHandDiffY < JetPackThresh)
            {
                targets[5].setTargetSelected();
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_U);
            }
            else
            {
                targets[5].setTargetUnselected();
                InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                //InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);
            }


        }
    }
}
