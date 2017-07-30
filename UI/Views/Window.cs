using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace UI.Views
{ 
    /// <summary>
    /// A custom window implementation
    /// </summary>
    [TemplatePart(Name = "PART_Minimize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Maximize", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Titlebar", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ResizeLeft", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ResizeRight", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ResizeBottom", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ResizeTop", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ResizeBottomRight", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
    public class FxWindow : Window
    {
        private const string CloseButtonPart = "PART_Close";
        private const string MaximizeButtonPart = "PART_Maximize";
        private const string MinimizeButtonPart = "PART_Minimize";
        private const string TitleBarPart = "PART_Titlebar";
        private const string ResizeTopPart = "PART_ResizeTop";
        private const string ResizeLeftPart = "PART_ResizeLeft";
        private const string ResizeRightPart = "PART_ResizeRight";
        private const string ResizeBottomPart = "PART_ResizeBottom";
        private const string ResizeBottomRightPart = "PART_ResizeBottomRight";
        private const string TitlePart = "PART_Title";
        private const string AboutButtonPart = "PART_About";

        private bool isResizing;

        /// <summary>
        /// Initializes the metadata for the window
        /// </summary>
        static FxWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FxWindow),
                new FrameworkPropertyMetadata(typeof(FxWindow)));
        }

        /// <summary>
        /// On Closed trigger
        /// </summary>
        /// <param name="e">Event argument</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowManager.Instance.WindowClosed(this);
        }

        /// <summary>
        ///  On activated trigger
        /// </summary>
        /// <param name="e">Event argument</param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            WindowManager.Instance.CurrentDialog = this;
        }

        #region Initialization logic

        /// <summary>
        /// Applies the control template to the window
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button minimizeButton = (Button)GetTemplateChild(MinimizeButtonPart);
            Button maximizeButton = (Button)GetTemplateChild(MaximizeButtonPart);
            Button closeButton = (Button)GetTemplateChild(CloseButtonPart);
            Button aboutButton = (Button)GetTemplateChild(AboutButtonPart);

            //TextBlock title = (TextBlock)GetTemplateChild(TitlePart); //  PART_Title

            Control titlebar = (Control)GetTemplateChild(TitleBarPart);

            titlebar.MouseDown += OnTitleBarMouseDown;
            //title.MouseDown += OnTitleBarMouseDown;
            titlebar.MouseDoubleClick += OnTitleBarDoubleClick;

            closeButton.Click += OnCloseButtonClick;
            minimizeButton.Click += OnMinimizeClick;
            maximizeButton.Click += OnMaximizeClick;

            AttachResizeRegions();
        }

        /// <summary>
        /// Attaches the eventhandlers to the resize helper regions
        /// </summary>
        private void AttachResizeRegions()
        {
            Rectangle resizeTop = (Rectangle)GetTemplateChild(ResizeTopPart);
            Rectangle resizeLeft = (Rectangle)GetTemplateChild(ResizeLeftPart);
            Rectangle resizeRight = (Rectangle)GetTemplateChild(ResizeRightPart);
            Rectangle resizeBottom = (Rectangle)GetTemplateChild(ResizeBottomPart);
            Rectangle resizeBottomRight = (Rectangle)GetTemplateChild(ResizeBottomRightPart);

            resizeTop.MouseDown += OnResizeRectMouseDown;
            resizeTop.MouseMove += OnResizeRectMouseMove;
            resizeTop.MouseUp += OnResizeRectMouseUp;

            resizeLeft.MouseDown += OnResizeRectMouseDown;
            resizeLeft.MouseMove += OnResizeRectMouseMove;
            resizeLeft.MouseUp += OnResizeRectMouseUp;

            resizeRight.MouseDown += OnResizeRectMouseDown;
            resizeRight.MouseMove += OnResizeRectMouseMove;
            resizeRight.MouseUp += OnResizeRectMouseUp;

            resizeBottom.MouseDown += OnResizeRectMouseDown;
            resizeBottom.MouseMove += OnResizeRectMouseMove;
            resizeBottom.MouseUp += OnResizeRectMouseUp;

            resizeBottomRight.MouseDown += OnResizeRectMouseDown;
            resizeBottomRight.MouseMove += OnResizeRectMouseMove;
            resizeBottomRight.MouseUp += OnResizeRectMouseUp;
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
        }

        #endregion

        #region Resize logic

        /// <summary>
        /// Handles the mouse up event for a resize helper region
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResizeRectMouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRectangle = (Rectangle)sender;
            senderRectangle.ReleaseMouseCapture();

            isResizing = false;
        }

        /// <summary>
        /// Handles the mouse move event for a resize helper region
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResizeRectMouseMove(object sender, MouseEventArgs e)
        {
            Rectangle senderRectangle = (Rectangle)sender;

            if (!isResizing) return;
            if (senderRectangle == null) return;

            switch (senderRectangle.Name)
            {
                case ResizeLeftPart:
                    ResizeFromLeft(senderRectangle, e);
                    break;
                case ResizeRightPart:
                    ResizeFromRight(senderRectangle, e);
                    break;
                case ResizeBottomPart:
                    ResizeFromBottom(senderRectangle, e);
                    break;
                case ResizeTopPart:
                    ResizeFromTop(senderRectangle, e);
                    break;
                case ResizeBottomRightPart:
                    ResizeFromBottomRight(senderRectangle, e);
                    break;
            }
        }

        /// <summary>
        /// Resize the window from the bottom-right corner of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeFromBottomRight(Rectangle sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            sender.CaptureMouse();

            double newHeight = Height + (mousePosition.Y - Height);
            double newWidth = Width + (mousePosition.X - Width);

            BeginInit();

            if (newHeight <= MaxHeight)
            {
                Height = newHeight;
            }
            else
            {
                Height = MaxHeight;
            }

            if (newWidth <= MaxWidth && newWidth > MinWidth)
            {
                Width = newWidth;
            }
            else if (newWidth > MaxWidth)
            {
                Width = MaxWidth;
            }

            EndInit();
        }

        /// <summary>
        /// Resizes from the bottom edge of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeFromBottom(Rectangle sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            sender.CaptureMouse();

            double newHeight = Height + (mousePosition.Y - Height);

            if (newHeight <= MaxHeight)
            {
                Height = newHeight;
            }
            else
            {
                Height = MaxHeight;
            }
        }

        /// <summary>
        /// Resizes from the right edge of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeFromRight(Rectangle sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            sender.CaptureMouse();

            double newWidth = Width + (mousePosition.X - Width);

            if (newWidth <= MaxWidth && newWidth > MinWidth)
            {
                Width = newWidth;
            }
            else if (newWidth > MaxWidth)
            {
                Width = MaxWidth;
            }
        }

        /// <summary>
        /// Resize from the top edge of the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeFromTop(Rectangle sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);

            sender.CaptureMouse();

            double newHeight = Height - mousePosition.Y;

            if (newHeight <= MaxHeight && newHeight > MinHeight)
            {
                Point absoluteMousePosition = PointToScreen(mousePosition);

                Height = newHeight;
                Top = absoluteMousePosition.Y;
            }
        }

        private void ResizeFromLeft(Rectangle sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);

            sender.CaptureMouse();

            double newWidth = Width - mousePosition.X;

            if (newWidth > MinWidth)
            {
                Point absoluteMousePosition = PointToScreen(mousePosition);

                Left = absoluteMousePosition.X;
                Width = newWidth;
            }
        }

        /// <summary>
        /// Handles the mouse down event of a resize helper region
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResizeRectMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isResizing = true;

                Rectangle senderRectangle = sender as Rectangle;
                senderRectangle.CaptureMouse();
            }
        }

        #endregion

        #region Window chrome eventhandlers

        /// <summary>
        /// Handles the double click event of the titlebar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTitleBarDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == ResizeMode.NoResize ||
                ResizeMode == ResizeMode.CanMinimize)
            {
                return;
            }

            WindowState = WindowState == WindowState.Maximized ?
                WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Handles the mouse down event of the titlebar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Handles the click event of the maximize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ?
                WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Handles the click event of the minimize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the click event of the close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            bool hideClose = (bool)this.GetValue(AttachedProperties.FrameworkElementProperties.HideCloseProperty);
            if (hideClose)
                Hide();
            else
                Close();
        }

        /// <summary>
        /// Handles the click event of the about button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// an event occurs while windows closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            bool hideClose = (bool)this.GetValue(AttachedProperties.FrameworkElementProperties.HideCloseProperty);
            if (hideClose)
                e.Cancel = true;
            base.OnClosing(e);
        }


        #endregion
    }
}

