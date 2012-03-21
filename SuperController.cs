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

    class SuperController
    {
        private MainWindow window;

        // gesture threshhold constants
        double walkThresh = 0.1;
        double runThresh = 0.5;
        double turnSlowThresh = 0.08;
        double turnFastThresh = 0.2;
        double armTurnThresh = 0.3;
        double JetPackThresh = 0.06;

        public SuperController(MainWindow win)
        {
            window = win;
        }

        public virtual void processSkeletonFrame(SkeletonData skeleton)
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
            detectShoulderTurning(rightShoulder, leftShoulder);
            detectJetPackUp(rightHand, rightElbow, rightShoulder, leftHand, leftElbow, leftShoulder);
            detectBirdwatcher(head, rightHand, rightElbow, rightShoulder);
        }

        /// <summary>
        /// Process walking/running. Calculates how far forward the right foot
        /// is from the left foot.
        /// </summary>
        /// <param name="rightFoot"></param>
        /// <param name="leftFoot"></param>
        private void detectWalking(Joint rightFoot, Joint leftFoot) {
          
            double feetDifferential = leftFoot.Position.Z - rightFoot.Position.Z;

            // Move forward
            if (feetDifferential > walkThresh)
            {
                if (feetDifferential > runThresh)
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_2);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_W);
                }

            }
            // Move backward
            else if (feetDifferential < -walkThresh)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_S);
            }
            else
            {
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_W)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_W);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_S)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_S);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_2)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_2);
            }
        }

        /// <summary>
        /// Process shoulder turning. Calculates the depth difference between shoulders.
        /// </summary>
        /// <param name="rightShoulder"></param>
        /// <param name="leftShoulder"></param>
        /// <returns></returns>
        private Boolean detectShoulderTurning(Joint rightShoulder, Joint leftShoulder)
        {
            double shoulderDepthDifferential = leftShoulder.Position.Z - rightShoulder.Position.Z;

            if (shoulderDepthDifferential > turnSlowThresh)
            {
                if (shoulderDepthDifferential > turnFastThresh)
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_COMMA);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_K);
                }
                return true;
            }
            else if (shoulderDepthDifferential < -turnSlowThresh)
            {
                if (shoulderDepthDifferential < -turnFastThresh)
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_PERIOD);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_L); 
                }
                return true;
            }
            else
            {
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_L)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_L);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_K)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_K);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_COMMA)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_COMMA);
                if (InputSimulator.IsKeyDown(VirtualKeyCode.OEM_PERIOD)) InputSimulator.SimulateKeyUp(VirtualKeyCode.OEM_PERIOD);
                return false;
            }
        }

        /// <summary>
        /// Process arm turning. Currently set to fast turn speed only.
        /// </summary>
        /// <param name="shoulder"></param>
        /// <param name="rightHand"></param>
        /// <param name="rightElbow"></param>
        /// <param name="leftHand"></param>
        /// <param name="leftElbow"></param>
        /// <returns></returns>
        private Boolean detectArmTurning(Joint shoulder, Joint rightHand, Joint rightElbow, Joint leftHand, Joint leftElbow)
        {

            double diffHandShoulderRight = Math.Abs(rightHand.Position.Y - shoulder.Position.Y);
            double diffHandShoulderLeft = Math.Abs(leftHand.Position.Y - shoulder.Position.Y);

            // right hand
            if (diffHandShoulderRight < armTurnThresh && rightHand.Position.X > rightElbow.Position.X)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.OEM_PERIOD);
                return true;
            }
            // left hand
            else if (diffHandShoulderLeft < armTurnThresh && leftHand.Position.X < leftElbow.Position.X)
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
            //Calculate how far our right hand is from head in both x and y directions
            double deltaX = Math.Abs(rightHand.Position.X - head.Position.X);
            double deltaY = Math.Abs(rightHand.Position.Y - head.Position.Y);

            double headelbowXDifferential = rightElbow.Position.X - head.Position.X;

            if (deltaY < 0.05
                    && rightHand.Position.Y > rightElbow.Position.Y
                    && rightElbow.Position.Y > rightShoulder.Position.Y
                    && deltaX < 0.3
                    && headelbowXDifferential > 0.2)
            {
                // Birdwatcher!
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_Y); // show augmented reality (twitter layer)
            }
            else
            {
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_Y)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_Y);
            }
        }

        /// <summary>
        /// Process jetpack gesture.
        /// Both hands should be at the same level in the y direction.
        /// Shoulder and elbow must line up in the x direction.
        /// Forearms horizontal to ground = hover
        /// Forearms angled above horizontal = go up
        /// angled below = go down
        /// </summary>
        /// <param name="rightHand"></param>
        /// <param name="rightElbow"></param>
        /// <param name="rightShoulder"></param>
        /// <param name="leftHand"></param>
        /// <param name="leftElbow"></param>
        /// <param name="leftShoulder"></param>
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
                && leftShoulderElbowDiffX < JetPackThresh) // elbows and shoulders aligned along x-axis
            {
                if (rightElbowHandDiffY < JetPackThresh
                    && leftElbowHandDiffY < JetPackThresh) 
                { //hover
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_U)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_J)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_J);

                } else if (rightHand.Position.Y < rightElbow.Position.Y && leftHand.Position.Y < leftElbow.Position.Y) 
                { //down
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_U)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);
                }
                else if (rightHand.Position.Y > rightElbow.Position.Y && leftHand.Position.Y > leftElbow.Position.Y
                          && rightHand.Position.Y < rightShoulder.Position.Y && leftHand.Position.Y < leftShoulder.Position.Y)
                { //up
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_J)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_J);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_U);
                }
                else
                {
                    if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_U)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);
                }
            }
            else
            {   
                if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_U)) InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_U);
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_J);
            }
        }

    }
}
