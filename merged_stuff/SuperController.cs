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
        //int forwardCount = 0;

        Boolean AugmentedRealityOn = false;
        double THRESH = 0.3;
        double JetPackThresh = 0.06;

        public SuperController(MainWindow win)
            : base(win)
        {
            window = win;
        }

        public override void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {
            // Get Skeleton Data
            Joint head = skeleton.Joints[JointID.Head];

            Joint rightShoulder = skeleton.Joints[JointID.ShoulderRight];
            Joint leftShoulder = skeleton.Joints[JointID.ShoulderLeft];

            Joint rightHand = skeleton.Joints[JointID.HandRight];
            Joint rightElbow = skeleton.Joints[JointID.ElbowRight];
           
            Joint leftHand = skeleton.Joints[JointID.HandLeft];
            Joint leftElbow = skeleton.Joints[JointID.ElbowLeft];

            Joint rightFoot = skeleton.Joints[JointID.FootRight];
            Joint leftFoot = skeleton.Joints[JointID.FootLeft];

            Joint centerShoulder = skeleton.Joints[JointID.ShoulderCenter];

            // Detect gestures
            detectWalking(rightFoot, leftFoot);
            //detectShoulderTurning(rightShoulder, leftShoulder);
            detectJetPackUp(rightHand, rightElbow, rightShoulder, leftHand, leftElbow, leftShoulder);
            //detectBirdwatcher(head, rightHand, rightElbow, rightShoulder);
        }

        public override void controllerActivated(Dictionary<int, Target> targets)
        {

        }

        private void detectWalking(Joint rightFoot, Joint leftFoot) {
          
            double feetDifferential = leftFoot.Position.Z - rightFoot.Position.Z;

            // Move front
            if (feetDifferential > 0.1)
            {
                if (feetDifferential > 0.5)
                {
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
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_S);
            }
            else
            {
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_W)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_S)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_2)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_2);
                //InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                //InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                //InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_2);
            }
        }

        private Boolean detectShoulderTurning(Joint rightShoulder, Joint leftShoulder)
        {
            double shoulderDepthDifferential = leftShoulder.Position.Z - rightShoulder.Position.Z;

            if (shoulderDepthDifferential > 0.08)
            {
                //targets[6].setTargetHighlighted();
                //targets[7].setTargetUnselected();
                if (shoulderDepthDifferential > 0.2)
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_COMMA);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                }
                return true;
            }
            else if (shoulderDepthDifferential < -0.08)
            {
                //targets[6].setTargetUnselected();
                //targets[7].setTargetHighlighted();
                if (shoulderDepthDifferential < -0.2)
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_PERIOD);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                    
                }
                return true;
            }
            else
            {
                //targets[6].setTargetUnselected();
                //targets[7].setTargetUnselected();
                if (InputSimulator.IsKeyDown(VirtualKeyCode.RIGHT)) InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.LEFT)) InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_COMMA)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_COMMA);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_PERIOD)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_PERIOD);
                return false;
            }
        }

        private Boolean detectArmTurning(Joint shoulder, Joint rightHand, Joint rightElbow, Joint leftHand, Joint leftElbow)
        {
          
            double diffHandShoulderRight = Math.Abs(rightHand.Position.Y - shoulder.Position.Y);

            double diffHandShoulderLeft = Math.Abs(leftHand.Position.Y - shoulder.Position.Y);

            // right hand
            if (diffHandShoulderRight < THRESH && rightHand.Position.X > rightElbow.Position.X)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_PERIOD);
                return true;
            }
            // left hand
            else if (diffHandShoulderLeft < THRESH && leftHand.Position.X < leftElbow.Position.X)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_COMMA);
                return true;
            }
            // neither right nor left hand
            else
            {
                
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_PERIOD)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_PERIOD);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_COMMA)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_COMMA);
                return false;
            }
        }

        private void detectBirdwatcher(Joint head, Joint rightHand, Joint rightElbow, Joint rightShoulder)
        {
            //Calculate how far our right hand is from target 5 in both x and y directions
            double deltaX = Math.Abs(rightHand.Position.X - head.Position.X);
            double deltaY = Math.Abs(rightHand.Position.Y - head.Position.Y);

            double headelbowXDifferential = rightElbow.Position.X - head.Position.X;

            // y increases towards top
            // hand > elbow > shoulder

            if (deltaY < 0.03
                    && rightHand.Position.Y > rightElbow.Position.Y
                    && rightElbow.Position.Y > rightShoulder.Position.Y
                    && deltaX < 0.3
                    && headelbowXDifferential > 0.2)
            {
                // Birdwatcher! (highlight 4 and show 5)
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_Y); // show augmented reality
                //AugmentedRealityOn = true;
          
            }
            else
            {
                /*
                if (AugmentedRealityOn)
                {
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_Y);
 
                    AugmentedRealityOn = false;
                }
                 */
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_Y)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_Y);
            }
        }

        private void detectJetPackUp(Joint rightHand, Joint rightElbow, Joint rightShoulder, 
                                        Joint leftHand, Joint leftElbow, Joint leftShoulder)
        {

            // right shoulder elbow x difference
            double rightShoulderElbowDiffX = Math.Abs(rightShoulder.Position.X - rightElbow.Position.X);

            // right elbow hand y difference
            double rightElbowHandDiffY = Math.Abs(rightElbow.Position.Y - rightHand.Position.Y);

            // left shoulder elbow x difference
            double leftShoulderElbowDiffX = Math.Abs(leftShoulder.Position.X - leftElbow.Position.X);

            // left elbow hand y difference
            double leftElbowHandDiffY = Math.Abs(leftElbow.Position.Y - leftHand.Position.Y);

            if (rightShoulderElbowDiffX < JetPackThresh
                && leftShoulderElbowDiffX < JetPackThresh)
            {
                if (rightElbowHandDiffY < JetPackThresh
                    && leftElbowHandDiffY < JetPackThresh) {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_U)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_J)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_J);

                } else if (rightHand.Position.Y < rightElbow.Position.Y && leftHand.Position.Y < leftElbow.Position.Y) {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);
                } else {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_U);
                }
            }

        }
    }
}
