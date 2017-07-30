using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace UI
{
    public class WindowManager : INotifyPropertyChanged
    {
        private static WindowManager instance = null;

        /// <summary>
        /// Singleton window manager
        /// </summary>
        public static WindowManager Instance
        {
            get
            {
                if (instance == null) instance = new WindowManager();
                return instance;
            }
        }

        private Dictionary<IntPtr, List<Window>> dialogStack = new Dictionary<IntPtr, List<Window>>();
        private Window currentWindow;

        /// <summary>
        /// UI Dispatcher
        /// </summary>
        public Dispatcher UIDispatcher
        {
            get { return App.Current.Dispatcher; }
        }

        /// <summary>
        /// Current active window
        /// </summary>
        public Window CurrentDialog
        {
            get
            {
                if (currentWindow == null)
                    currentWindow = App.Current.MainWindow;
                return currentWindow;
            }
            set { currentWindow = value; }
        }

        private static IntPtr GetWindowHandle(Window dialog)
        {
            if (dialog == null) return IntPtr.Zero;
            return new WindowInteropHelper(dialog).Handle;
        }

        /// <summary>
        /// Sets initial dialog for window manager
        /// </summary>
        /// <param name="initial">Initial dialog</param>
        public void SetInitialWindow(Window initial)
        {
            dialogStack.Add(GetWindowHandle(initial), new List<Window>(new Window[] { initial }));
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentDialog"));
        }

        /// <summary>
        /// Shows new dialog over last one (modal dialog)
        /// </summary>
        /// <param name="dialog">Dialog instance</param>
        internal void ShowNewModalDialog(Window dialog)
        {
            Window current = (Window)App.Current.Dispatcher.Invoke(new Func<Window>(() => CurrentDialog));
            dialog.Dispatcher.Invoke(new Action<Window>((Window l) => dialog.Owner = l), current);
            dialog.Dispatcher.Invoke(new Action<WindowStartupLocation>((WindowStartupLocation wsl) => dialog.WindowStartupLocation = wsl), WindowStartupLocation.CenterOwner);
            dialog.Dispatcher.Invoke(new Action<WindowState>((WindowState ws) => dialog.WindowState = ws), WindowState.Maximized);
            Window parent = current;
            while (parent.Owner != null)
                parent = parent.Owner;
            IntPtr handle = (IntPtr)current.Dispatcher.Invoke(new Func<IntPtr>(() => GetWindowHandle(parent)));
            if (dialogStack.ContainsKey(handle))
            {
                dialogStack[handle].Add(dialog);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentDialog"));
                //if (!(dialog is Views.NotificationView ))
                //    current.Hide();
                dialog.Dispatcher.Invoke(new Func<bool?>(dialog.ShowDialog));
                dialogStack[handle].Remove(dialog);
            }
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentDialog"));
        }

        /// <summary>
        /// Shows new dialog
        /// </summary>
        /// <param name="dialog">Dialog instance</param>
        internal void ShowNewDialog(Window dialog)
        {
            //: set location screen center
            dialog.Dispatcher.Invoke(new Action<WindowStartupLocation>((WindowStartupLocation wsl) => dialog.WindowStartupLocation = wsl), WindowStartupLocation.CenterScreen);

            //: show dialog modalless
            dialog.Dispatcher.Invoke(new Action(dialog.Show));

            //: add to dialog stack
            dialogStack.Add(GetWindowHandle(dialog), new List<Window>(new Window[] { dialog }));
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentDialog"));
        }

        /// <summary>
        /// Updates window state of current open dialog
        /// </summary>
        /// <param name="state">New window state</param>
        public void UpdateState(WindowState state)
        {
            if (CurrentDialog != null)
                CurrentDialog.Dispatcher.Invoke(new Action<Window>((Window dialog) => dialog.WindowState = state), CurrentDialog as Window);
        }

        /// <summary>
        /// Gets current window's state
        /// </summary>
        public WindowState WindowState
        {
            get { return (WindowState)CurrentDialog.Dispatcher.Invoke(new Func<WindowState>(() => CurrentDialog.WindowState)); }
        }

        /// <summary>
        /// Trigger on window closed event so that we can remove window from window manager
        /// </summary>
        /// <param name="w">Closed window</param>
        public void WindowClosed(Window w)
        {
            if (dialogStack.Count == 0)
                return;
            if (dialogStack.Values.Last().Last() == w)
            {
                dialogStack.Values.Last().Remove(w);

                if (dialogStack.Values.Last().Count == 0)
                    dialogStack.Remove(dialogStack.Keys.Last());
                //if new dialog append in stack  previous dialog exists in previous list
                if (dialogStack.Count > 0)
                {
                    if (dialogStack.Values.Last().Last() == w)
                        dialogStack.Values.Last().Remove(w);
                    currentWindow = dialogStack.Values.Last().Last();
                    currentWindow.Show();
                    currentWindow.Focus();
                }
                return;
            }
            IntPtr handle = GetWindowHandle(w);
            if (dialogStack.ContainsKey(handle))
            {
                dialogStack.Remove(handle);
                return;
            }

            //: invalid case, owner null means dialogStack should contain it
            if (w.Owner == null)
                return;

            Window parent = w, topMost = null;
            do
            {
                topMost = parent;
                parent = topMost.Owner;
            }
            while (parent != null);

            handle = GetWindowHandle(topMost);
            //: dialog stack should contain the top most window
            if (dialogStack.ContainsKey(handle))
                dialogStack[handle].Remove(w);
        }

        /// <summary>
        /// Checks if the given window is one of the top most windows
        /// </summary>
        /// <param name="window">Window instance</param>
        /// <returns>True if one of the topmost windows, false otherwise</returns>
        public bool IsTopMostWindow(Window window)
        {
            foreach (List<Window> windows in dialogStack.Values)
            {
                if (windows.Count == 0) continue;
                if (window == windows[windows.Count - 1])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Event for notify property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
