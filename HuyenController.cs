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
        public Point curMenuHandPoint, lastMenuHandPoint;
        public int menuCount = 0;
        public int selectCount = 0;
        double Thresh = 0.05;
        double JetPackThresh = 0.08;

        public HuyenController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            //detectPullMenu(skeleton, targets);
            detectJetPackUp(skeleton, targets);

            
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {
           
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
            double rightShoulderHandDiffX= Math.Abs(rightShoulderPosition.X - rightHandPosition.X);

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
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);

            }


        }

        private void detectPullMenu(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            if (targets[4].isSelected())
            {
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
                Joint rightHand = skeleton.Joints[JointID.HandRight];
                Joint headJoint = skeleton.Joints[JointID.Head];
                Joint neckJoint = skeleton.Joints[JointID.Spine];
                Joint shoulderJoint = skeleton.Joints[JointID.ShoulderRight];
                Joint elbowJoint = skeleton.Joints[JointID.ElbowRight];

                Point handPosition = new Point(rightHand.Position.X, rightHand.Position.Y);
                Point headPosition = new Point(headJoint.Position.X, headJoint.Position.Y);
                Point elbowPosition = new Point(elbowJoint.Position.X, elbowJoint.Position.Y);
                Point shoulderPosition = new Point(shoulderJoint.Position.X, shoulderJoint.Position.Y);

                double diffHandElbow = Math.Abs(elbowPosition.X - handPosition.X);

                if (targets[2].isSelected()) // menu gesture has been initiated
                {
                    if (diffHandElbow < Thresh)
                    {
                        if (lastMenuHandPoint == null) lastMenuHandPoint = handPosition;
                        curMenuHandPoint = handPosition;

                        if (curMenuHandPoint.Y < lastMenuHandPoint.Y)
                        {
                            menuCount++; //swiping
                        }

                        if (menuCount > 15) //swipe complete
                        {
                            targets[4].setTargetSelected();
                            targets[4].showTarget();
                            targets[5].showTarget();
                            targets[2].setTargetUnselected();
                            // reset the count
                            menuCount = 0;
                        }
                    }
                    else //gesture deviation, stop
                    {
                        targets[2].setTargetUnselected();
                        menuCount = 0;
                    }
                }
                else
                {
                    // check for menu gesture initiation
                    if (handPosition.Y > elbowPosition.Y && elbowPosition.Y > headPosition.Y && diffHandElbow < Thresh)
                    {
                        targets[2].setTargetSelected();
                    }
                    else
                    {
                        targets[2].setTargetUnselected();
                    }
                }
            }
        }
    }
}
