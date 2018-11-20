// *************************************************************************************************
// ----------------------------------
// Product:     DockableForm
// Author:      Bentley Wolfe
// Company:     Torbo Software Design (http://www.torbo.us/)
// Purpose:     Enable forms to dock and move with one another.
// ----------------------------------
// 
// Disclaimer:
// This code is released under the Common Public License Version 1.0 that is
// described in the contained file, "License.txt", and was retrieved from this
// website:  http://www.opensource.org/licenses/cpl1.0.php
//
// This code is provided "as is" with no warranty either express or implied.
// The author accepts no liability for any damage or loss of business that
// this product may cause.
//
// Version  Date        Who  Comments
// ============================================================================
// 1.0.0.0  2008/03/01  BWW  Created the code
// 1.0.0.1  2008/05/26  BWW  Small modifications to the way docking is handled.
//                           Also added support for multiple monitors.
// 1.0.1.0  2009/01/01  BWW  Moved form movement code to the mouse move event.
// ============================================================================
//
// Copyright 2009, Torbo Software Design, All rights reserved
//
// *************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Torbo
{
    /// <summary>
    /// This class adds the functionality for forms to "dock" with one another.
    /// </summary>
    /// <remarks>
    /// Forms being able to dock means that if you have one or more DockableForms connected
    /// to each other using the ConnectForm() method of the class, and you move one of
    /// them within the specified docking distance (default = 10) of one of the others,
    /// it will become "docked," and the form that it's docked to becomes its "master."
    /// Then if you move the "master" form, the "docked" form will move along with its
    /// "master."  You can also dock forms to the side of the primary screen's working
    /// area.
    /// </remarks>
    public partial class DockableForm : Form
    {
        #region Properties, Local Variables and Constants

        ArrayList dFormArrayList;
        int myHashCode;
        int designatedDockingGap;
        Point formToCursorDiff;
        Point mouseCurrLoc;
        Point distanceFromMaster;
        bool formInMoveMode;
        bool formInResizeMode;

        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCLBUTTONDOWN = 0xa1;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 2;

        /// <summary>
        /// Indicates whether this is the form that is currently being moved by the user.
        /// </summary>
        public bool FormInMoveMode
        {
            get { return formInMoveMode; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the DockableForm class with the default docking gap of 10.
        /// </summary>
        public DockableForm()
        {
            dFormArrayList = new ArrayList(10);
            myHashCode = this.GetHashCode();
            designatedDockingGap = 10;
            formInMoveMode = false;
            formInResizeMode = false;
            InitializeComponent();

            mouseCurrLoc = Cursor.Position;
            distanceFromMaster = new Point(0, 0);
        }

        /// <summary>
        /// Creates an instance of the DockableForm class with the passed in docking gap.
        /// </summary>
        /// <param name="dockingGap">Distance between two connected DockableForm objects before they dock.</param>
        public DockableForm(int dockingGap) : this()
        {
            designatedDockingGap = dockingGap;
        }

        /// <summary>
        /// Creates an instance of the DockableForm class with the passed in docking gap.
        /// </summary>
        /// <param name="dockingGap">Distance between two connected DockableForm objects before they dock.</param>
        /// <param name="formLocation">Starting location of this instance.</param>
        public DockableForm(int dockingGap, Point formLocation) : this(dockingGap)
        {
            this.SetDesktopLocation(formLocation.X, formLocation.Y);
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// We are overriding the handling of Windows messages so that we can capture the ones
        /// necessary for moving and coordinating this and other connected DockableForm objects.
        /// </summary>
        /// <param name="m">Windows message passed in from the operating system.</param>
        protected override void WndProc(ref Message m)
        {
            Point globalMouseLoc = Cursor.Position;

            // This happens when the user right-clicks on the form's caption and selects "Move"
            if ((m.Msg == WM_SYSCOMMAND) && (m.WParam.ToInt32() == SC_MOVE))
            {
                int xDiff = globalMouseLoc.X - this.Location.X;
                int yDiff = globalMouseLoc.Y - this.Location.Y;
                formToCursorDiff = new Point(xDiff, yDiff);
                mouseCurrLoc = globalMouseLoc;
                formInMoveMode = true;
                this.Capture = true;
                return;
            }

            // This happens when the user clicks the left mouse button on the caption of the form
            if ((m.Msg == WM_NCLBUTTONDOWN) && (m.WParam.ToInt32() == HTCAPTION))
            {
                this.CheckDock();
                int xDiff = globalMouseLoc.X - this.Location.X;
                int yDiff = globalMouseLoc.Y - this.Location.Y;
                formToCursorDiff = new Point(xDiff, yDiff);
                mouseCurrLoc = globalMouseLoc;
                formInMoveMode = true;
                this.Capture = true;
                this.Focus();
                return;
            }

            // This happens when the user releases the left mouse button
            if (m.Msg == WM_LBUTTONUP)
            {
                formInMoveMode = false;
                this.Capture = false;
                DockableForm_Move(this, new EventArgs());
                this.CheckDock();
                this.Refresh();
            }

            // This message is sent by the OS when some of the form's border needs to be repainted
            if (m.Msg == WM_NCPAINT)
            {
                // Repaints the entire border and caption of the form
                m.WParam = new IntPtr(1);
            }

            // Resume normal Windows message handling
            base.WndProc(ref m);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Here is where we move any forms that are docked to this DockableForm object.  This also
        /// creates a chain effect down the line of any connected DockableForm objects docked to
        /// each consecutive form that is moved.
        /// </summary>
        private void DockableForm_Move(object sender, EventArgs e)
        {
            if (formInResizeMode == false)
            {
                ClearDisposedFormsFromArray();

                foreach (DFormAndDockStatus d in dFormArrayList)
                {
                    if (d.DForm.IsFormMyMaster(this))
                    {
                        d.DForm.SetDesktopLocation(this.Location.X + d.DForm.distanceFromMaster.X,
                            this.Location.Y + d.DForm.distanceFromMaster.Y);
                    }
                }
            }
        }

        private void DockableForm_MouseMove(object sender, MouseEventArgs e)
        {
            Point globalMouseLoc = Cursor.Position;

            // Here is the code that manually moves the form when the user drags it, but first taking into
            // account its relative position to all connected DockableForm objects so that it can be 
            // determined whether or not we should "snap" to a position alongside any of them.
            if ((formInMoveMode) && (mouseCurrLoc != globalMouseLoc))
            {
                ClearDisposedFormsFromArray();

                int xNew = globalMouseLoc.X - formToCursorDiff.X;
                int yNew = globalMouseLoc.Y - formToCursorDiff.Y;
                mouseCurrLoc = globalMouseLoc;
                bool formHasMaster = false;
                bool withinDockDistance = false;
                Point newPoint = new Point(xNew, yNew);
                foreach (DFormAndDockStatus d in dFormArrayList)
                {
                    if (formHasMaster == false)
                    {
                        withinDockDistance = false;
                        newPoint = GetNewFormPosition(designatedDockingGap, new Rectangle(newPoint.X, newPoint.Y, this.Width, this.Height), d.DForm, out withinDockDistance);
                        if ((withinDockDistance) && (d.DForm.IsFormMyMaster(this)))
                        {
                            withinDockDistance = false;   // We can't have docking both ways
                            newPoint = new Point(xNew, yNew);
                        }
                        if (withinDockDistance)
                        {
                            // Here we are checking the case that if we dock to this form, we would be creating a loop
                            DockableForm dTemp = d.DForm;
                            bool keepGoing = true;
                            bool masterLoopFound = false;
                            while (keepGoing)
                            {
                                keepGoing = false;
                                foreach (DFormAndDockStatus d2 in dTemp.dFormArrayList)
                                {
                                    if (d2.IsMasterForm)
                                    {
                                        if (d2.DForm == this) masterLoopFound = true;
                                        else keepGoing = true;
                                        dTemp = d2.DForm;
                                    }
                                }
                            }
                            if (masterLoopFound) withinDockDistance = false;
                        }
                        d.IsMasterForm = formHasMaster = withinDockDistance;

                        // We have a new master
                        if (withinDockDistance)
                        {
                            distanceFromMaster = new Point(this.Location.X - d.DForm.Location.X, this.Location.Y - d.DForm.Location.Y);
                        }
                    }
                    else
                    {
                        d.IsMasterForm = false;
                    }
                }
                this.SetDesktopLocation(newPoint.X, newPoint.Y);
            }
        }

        /// <summary>
        /// When this DockableForm is first shown, we check to see if it is within the "dockingGap"
        /// distance of any other connected DockableForm objects.
        /// </summary>
        private void DockableForm_Shown(object sender, EventArgs e)
        {
            CheckDock();
        }

        private void DockableForm_ResizeBegin(object sender, EventArgs e)
        {
            formInResizeMode = true;
        }

        private void DockableForm_ResizeEnd(object sender, EventArgs e)
        {
            formInResizeMode = false;
        }

        /// <summary>
        /// When we close this DockableForm, it is then disposed.
        /// </summary>
        private void DockableForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        #endregion

        #region Local Routines

        /// <summary>
        /// This method is used to check if one DockableForm is within the specified docking distance of another DockableForm.
        /// </summary>
        /// <param name="dockingGap">The distance between two forms before they should become "docked."</param>
        /// <param name="thisFormRect">The location and dimensions of the DockableForm that is being moved.</param>
        /// <param name="otherForm">The DockableForm with which you are testing docking distance from.</param>
        /// <param name="isDocked">Out parameter telling whether the two DockableForm objects are within the docking distance.</param>
        /// <returns>New location of the DockableForm that is being moved after taking docking distance into consideration.</returns>
        public Point GetNewFormPosition(int dockingGap, Rectangle thisFormRect, DockableForm otherForm, out bool isDocked)
        {
            isDocked = false;
            int xNew = thisFormRect.X;
            int yNew = thisFormRect.Y;
            Rectangle screenRect = new Rectangle(0, 0, Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            int leftScreenGap = Math.Abs(thisFormRect.X);
            int rightScreenGap = Math.Abs(screenRect.Width - (thisFormRect.X + thisFormRect.Width));
            int topScreenGap = Math.Abs(thisFormRect.Y);
            int bottomScreenGap = Math.Abs(screenRect.Height - (thisFormRect.Y + thisFormRect.Height));

            if (otherForm != null)
            {
                int[] xRange = new int[2] { otherForm.Location.X, otherForm.Location.X + otherForm.Width };
                int[] yRange = new int[2] { otherForm.Location.Y, otherForm.Location.Y + otherForm.Height };
                int xGap = Math.Abs(thisFormRect.X - otherForm.Location.X);
                int yGap = Math.Abs(thisFormRect.Y - otherForm.Location.Y);
                int leftFormGap = Math.Abs((thisFormRect.X + thisFormRect.Width) - otherForm.Location.X);
                int rightFormGap = Math.Abs(thisFormRect.X - (otherForm.Location.X + otherForm.Width));
                int topFormGap = Math.Abs((thisFormRect.Y + thisFormRect.Height) - otherForm.Location.Y);
                int bottomFormGap = Math.Abs(thisFormRect.Y - (otherForm.Location.Y + otherForm.Height));

                if ((leftFormGap <= dockingGap) && (((thisFormRect.Y >= yRange[0]) && (thisFormRect.Y <= yRange[1])) ||
                    ((thisFormRect.Y + otherForm.Height >= yRange[0]) && (thisFormRect.Y + otherForm.Height <= yRange[1]))))
                {
                    xNew = otherForm.Location.X - thisFormRect.Width;
                    if (yGap <= dockingGap) yNew = otherForm.Location.Y;
                    isDocked = true;
                }
                if ((rightFormGap <= dockingGap) && (((thisFormRect.Y >= yRange[0]) && (thisFormRect.Y <= yRange[1])) ||
                    ((thisFormRect.Y + otherForm.Height >= yRange[0]) && (thisFormRect.Y + otherForm.Height <= yRange[1]))))
                {
                    xNew = otherForm.Location.X + otherForm.Width;
                    if (yGap <= dockingGap) yNew = otherForm.Location.Y;
                    isDocked = true;
                }
                if ((topFormGap <= dockingGap) && (((thisFormRect.X >= xRange[0]) && (thisFormRect.X <= xRange[1])) ||
                    ((thisFormRect.X + otherForm.Width >= xRange[0]) && (thisFormRect.X + otherForm.Width <= xRange[1]))))
                {
                    yNew = otherForm.Location.Y - thisFormRect.Height;
                    if (xGap <= dockingGap) xNew = otherForm.Location.X;
                    isDocked = true;
                }
                if ((bottomFormGap <= dockingGap) && (((thisFormRect.X >= xRange[0]) && (thisFormRect.X <= xRange[1])) ||
                    ((thisFormRect.X + otherForm.Width >= xRange[0]) && (thisFormRect.X + otherForm.Width <= xRange[1]))))
                {
                    yNew = otherForm.Location.Y + otherForm.Height;
                    if (xGap <= dockingGap) xNew = otherForm.Location.X;
                    isDocked = true;
                }
            }

            if (isDocked == false)
            {
                if (leftScreenGap <= dockingGap)
                {
                    xNew = 0;
                }

                if (rightScreenGap <= dockingGap)
                {
                    xNew = screenRect.Width - thisFormRect.Width;
                }

                if (topScreenGap <= dockingGap)
                {
                    yNew = 0;
                }

                if (bottomScreenGap <= dockingGap)
                {
                    yNew = screenRect.Height - thisFormRect.Height;
                }
            }
            return new Point(xNew, yNew);
        }

        /// <summary>
        /// Attempts to connect two DockableForm objects so that they are able to dock with one another.
        /// </summary>
        /// <param name="passedDForm">DockableForm you want to connect to.</param>
        /// <returns>True if connected, False if not.</returns>
        public bool ConnectForm(DockableForm passedDForm)
        {
            if (this != passedDForm)
            {
                foreach (DFormAndDockStatus d in dFormArrayList)
                {
                    if (d.DForm == passedDForm)
                    {
                        return true;
                    }
                }
                dFormArrayList.Add(new DFormAndDockStatus(passedDForm));
                passedDForm.ConnectForm(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to disconnect two DockableForm objects so they are no longer able to dock with one another.
        /// </summary>
        /// <param name="passedDForm">DockableForm you want to disconnect from.</param>
        /// <returns>True if DockableForm found and removed, False if not found.</returns>
        public bool DisconnectForm(DockableForm passedDForm)
        {
            if (this != passedDForm)
            {
                foreach (DFormAndDockStatus d in dFormArrayList)
                {
                    if (d.DForm == passedDForm)
                    {
                        dFormArrayList.Remove(d);
                        d.DForm.DisconnectForm(this);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to set the value determining whether or not this DockableForm is docked to the
        /// DockableForm passed in to the method.
        /// </summary>
        /// <param name="passedDForm">DockableForm with which we are setting our docking "master" status.</param>
        /// <param name="docked">True if we are docked to the passed in DockableForm, False if not.</param>
        /// <returns>True if we are connected to the passed in DockableForm and the status has been set, False if not.</returns>
        public bool SetFormMasterStatus(DockableForm passedDForm, bool docked)
        {
            if (this != passedDForm)
            {
                foreach (DFormAndDockStatus d in dFormArrayList)
                {
                    if (d.DForm == passedDForm)
                    {
                        d.IsMasterForm = docked;
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether or not we are currently docked to the passed in DockableForm object.
        /// </summary>
        /// <param name="passedDForm">DockableForm with which we are checking docking status.</param>
        /// <returns>True if we are docked to the passed in DockingForm, False if not.</returns>
        public bool IsFormMyMaster(DockableForm passedDForm)
        {
            foreach (DFormAndDockStatus d in dFormArrayList)
            {
                if (d.DForm == passedDForm)
                {
                    return d.IsMasterForm;
                }
            }
            return false;
        }

        /// <summary>
        /// Recursively checks any DockableForm objects docked to me to see if they are still within the specified
        /// docking distance.  If they are not, then their docking status is changed to relect it.
        /// </summary>
        public void CheckDock()
        {
            ClearDisposedFormsFromArray();

            bool formHasMaster = false;
            bool withinDockDistance = false;
            Point newPoint = new Point(this.Location.X, this.Location.Y);
            foreach (DFormAndDockStatus d in dFormArrayList)
            {
                if (formHasMaster == false)
                {
                    withinDockDistance = false;
                    newPoint = GetNewFormPosition(designatedDockingGap, new Rectangle(newPoint.X, newPoint.Y, this.Width, this.Height), d.DForm, out withinDockDistance);
                    if ((withinDockDistance) && (d.DForm.IsFormMyMaster(this)))
                    {
                        withinDockDistance = false;   // We can't have docking both ways
                        newPoint = new Point(this.Location.X, this.Location.Y);
                    }
                    if (withinDockDistance)
                    {
                        // Here we are checking the case that if we dock to this form, we would be creating a loop
                        DockableForm dTemp = d.DForm;
                        bool keepGoing = true;
                        bool masterLoopFound = false;
                        while (keepGoing)
                        {
                            keepGoing = false;
                            foreach (DFormAndDockStatus d2 in dTemp.dFormArrayList)
                            {
                                if (d2.IsMasterForm)
                                {
                                    if (d2.DForm == this) masterLoopFound = true;
                                    else keepGoing = true;
                                    dTemp = d2.DForm;
                                }
                            }
                        }
                        if (masterLoopFound) withinDockDistance = false;
                    }
                    d.IsMasterForm = formHasMaster = withinDockDistance;

                    // We have a new master
                    if (withinDockDistance)
                    {
                        distanceFromMaster = new Point(this.Location.X - d.DForm.Location.X, this.Location.Y - d.DForm.Location.Y);
                    }
                }
                else
                {
                    d.IsMasterForm = false;
                }

                if (d.DForm.IsFormMyMaster(this)) d.DForm.CheckDock();
            }
            this.SetDesktopLocation(newPoint.X, newPoint.Y);
            this.Refresh();
        }

        /// <summary>
        /// Checks if any of the connected DockableForm objects have been closed and removes them from the list
        /// of DockableForm objects connected to me.
        /// </summary>
        public void ClearDisposedFormsFromArray()
        {
            ArrayList newFormArrayList = new ArrayList(10);

            foreach (DFormAndDockStatus d in dFormArrayList)
            {
                if (d.DForm.IsDisposed == false)
                {
                    newFormArrayList.Add(d);
                }
            }

            dFormArrayList = newFormArrayList;
        }

        #endregion

        /// <summary>
        /// This is a helper class used in the DockableForm class to hold a DockableForm object and
        /// a boolean value to show whether or not the current DockableForm is connected to the 
        /// DockableForm contained in this DFormAndDockStatus object instance.
        /// </summary>
        private class DFormAndDockStatus
        {
            DockableForm dForm;
            bool isMasterForm;
            private DFormAndDockStatus() { }
            public DFormAndDockStatus(DockableForm passedInDForm)
            {
                dForm = passedInDForm;
                isMasterForm = false;
            }
            public DockableForm DForm
            {
                get { return dForm; }
                set { dForm = value; }
            }
            public bool IsMasterForm
            {
                get { return isMasterForm; }
                set { isMasterForm = value; }
            }
        }
    }
}
