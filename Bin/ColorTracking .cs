using System;
using System.Collections.Generic;
using System.Drawing;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Forms;

namespace Bin
{
    public class ColorTracking : IDisposable
    {

        #region Members

        protected VideoCaptureDevice FinalVideoSource;
        protected FilterInfoCollection VideoCaptuerDevices;

        #endregion

        #region Static Properties

        /// <summary>
        /// return system moniter Width pixel
        /// </summary>
        public static int ScreenWidth
        {
            get { return Screen.PrimaryScreen.Bounds.Width; }
        }

        /// <summary>
        /// return system moniter Height pixel
        /// </summary>
        public static int ScreenHeight
        {
            get { return Screen.PrimaryScreen.Bounds.Height; }
        }

        #endregion

        #region Events

        public event TrackingDelegate OnTracking;
        public event RenderFramegDelegate OnRenderFrame;


        #endregion

        #region Properties

        /// <summary>
        /// Connected Web Cam(USB) List
        /// </summary>
        public List<WebCam> WebCamList { get; private set; }

        //IntRange _filter;
        ///// <summary>
        ///// Filter Color
        ///// </summary>
        //public IntRange FilterColor
        //{
        //    get { return _filter; }
        //    set
        //    {
        //        _filter = value;
        //    }
        //}

        WebCam _selected;
        /// <summary>
        /// Selected WebCam
        /// </summary>
        public WebCam SelectedWebCam
        {
            get { return _selected; }
            set
            {
                _selected = value;
            }
        }


        RGB _filterColorRGB = new RGB(0, 162, 232);
        /// <summary>
        /// 
        /// </summary>
        public RGB FilterColorRGB
        {
            get { return _filterColorRGB; }
            set
            {
                _filterColorRGB = value;
            }
        }

        int _frame;
        /// <summary>
        /// WebCam Frame
        /// </summary>
        public int Frame
        {
            get { return _frame; }
            set
            {
                _frame = value;
            }
        }

        Size _frameSize;
        /// <summary>
        /// Screen Resolution 
        /// Defualt Value = Full Size
        /// </summary>
        public Size FrameSize
        {
            get { return _frameSize; }
            set
            {
                _frameSize = value;
            }
        }

        #endregion

        #region Singelton

        private static ColorTracking _instance;
        public static ColorTracking Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ColorTracking();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ColorTracking()
        {
            FrameSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Frame = 30;
            WebCamList = GetWebCams();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Open the image starts to take the camera connection
        /// </summary>
        /// <returns>bool</returns>
        public bool OpenCam()
        {
            if (SelectedWebCam == null)
                throw new NullReferenceException("SelectedWebCam is null");
            return OpenCam(SelectedWebCam);
        }

        /// <summary>
        /// Open the image starts to take the camera connection
        /// </summary>
        /// <param name="webCam"></param>
        /// <returns>bool</returns>
        public bool OpenCam(WebCam webCam)
        {
            if (webCam == null)
                throw new ArgumentNullException("webcam param is null");
            try
            {
                SelectedWebCam = webCam;
                FinalVideoSource = new VideoCaptureDevice(webCam.MonikerString);
                FinalVideoSource.NewFrame += new NewFrameEventHandler(FinalVideoSource_NewFrame);

                FinalVideoSource.DesiredFrameRate = Frame;
                FinalVideoSource.DesiredFrameSize = FrameSize;

                FinalVideoSource.Start();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The given frame finds the objects and puts them into the frame
        /// </summary>
        /// <param name="frame"></param>
        public void Tracke(Bitmap frame)
        {
            EuclideanColorFiltering Filter = new EuclideanColorFiltering();
            Filter.CenterColor = FilterColorRGB;
            Filter.Radius = 1;
            Filter.ApplyInPlace(frame);

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 2;
            blobCounter.MinHeight = 2;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            //Bitmap grayImage = grayFilter.Apply(image);
            blobCounter.ProcessImage(frame);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Rectangle objectRect = new Rectangle();
            foreach (Rectangle recs in rects)
            {
                objectRect = recs;
                Graphics g = Graphics.FromImage(frame);
                using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                {
                    g.DrawRectangle(pen, objectRect);
                }
                g.Dispose();
            }

            OnRenderFrame?.Invoke(frame, new EventArgs());
            OnTracking?.Invoke(objectRect, new EventArgs());
        }


        /// <summary>
        /// Scan Connected Web Cam(USB)
        /// </summary>
        private List<WebCam> GetWebCams()
        {
            List<WebCam> webCams = new List<WebCam>();
            try
            {

                VideoCaptuerDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                WebCamList = new List<WebCam>();
                foreach (FilterInfo device in VideoCaptuerDevices)
                {
                    webCams.Add(new WebCam(device.Name, device.MonikerString));
                }
            }
            catch
            {
                webCams = new List<WebCam>();
            }
            return webCams;
        }


        #endregion

        #region Private Methods


        /// <summary>
        /// image from the WebCam is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void FinalVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Tracke(image);

            //EuclideanColorFiltering Filter = new EuclideanColorFiltering();
            //Filter.CenterColor = FilterColorRGB;
            //Filter.Radius = 1;
            //Filter.ApplyInPlace(image);

            //BlobCounter blobCounter = new BlobCounter();
            //blobCounter.MinWidth = 2;
            //blobCounter.MinHeight = 2;
            //blobCounter.FilterBlobs = true;
            //blobCounter.ObjectsOrder = ObjectsOrder.Size;
            ////Bitmap grayImage = grayFilter.Apply(image);
            //blobCounter.ProcessImage(image);
            //Rectangle[] rects = blobCounter.GetObjectsRectangles();
            //Rectangle objectRect = new Rectangle();
            //foreach (Rectangle recs in rects)
            //{
            //    if (rects.Length > 0)
            //    {
            //        objectRect = rects[0];
            //        Graphics g = Graphics.FromImage(image);
            //        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
            //        {
            //            g.DrawRectangle(pen, objectRect);
            //        }
            //        g.Dispose();

            //    }
            //}

            //Cursor.Position = new System.Drawing.Point(objectRect.X + (objectRect.Width / 2), objectRect.Y + (objectRect.Height / 2));


        }

        /// <summary>
        /// Locate the object gets into the square
        /// </summary>
        /// <param name="image"></param>
        private void DrawingBorder(Bitmap image)
        {

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 2;
            blobCounter.MinHeight = 2;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            //Bitmap grayImage = grayFilter.Apply(image);
            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Rectangle objectRect = new Rectangle();
            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                {
                    objectRect = rects[0];
                    Graphics g = Graphics.FromImage(image);
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                    g.Dispose();

                }
            }

            //Cursor.Position = new System.Drawing.Point(objectRect.X + (objectRect.Width / 2), objectRect.Y + (objectRect.Height / 2));

        }

        /// <summary>
        /// Set Mouse Position
        /// </summary>
        /// <param name="X">X pixel</param>
        /// <param name="Y">Y pixel</param>
        private void MouseLocation(int X, int Y)
        {
            Cursor.Position = new System.Drawing.Point(X, Y);

        }

        #endregion

        /// <summary>
        /// Form Closed Run method
        /// </summary>
        public void Dispose()
        {

            if (FinalVideoSource != null && FinalVideoSource.IsRunning)
            {
                FinalVideoSource.SignalToStop();
                FinalVideoSource = null;
            }
        }
    }
}
