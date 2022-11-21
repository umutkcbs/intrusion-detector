using OpenCvSharp.Extensions;
using OpenCvSharp;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Point = OpenCvSharp.Point;
using Size = System.Drawing.Size;
//using CefSharp.DevTools.CSS;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace wake
{
    public partial class Form1 : Form
    {
        public int counter = 0;
        public List<string> wakeList = new List<string>();
        public List<string> filenameList = new List<string>();
        public Form1()
        {
            InitializeComponent();

            System.IO.Directory.CreateDirectory("-----CAPTURES-----");
            SystemEvents.PowerModeChanged += OnPowerChange;
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    VideoCapture capture;
                    Mat frame;
                    Bitmap image;

                    frame = new Mat();
                    capture = new VideoCapture(0);
                    capture.Open(0);

                    if (capture.IsOpened())
                    {
                        capture.Read(frame);
                        image = BitmapConverter.ToBitmap(frame);
                        
                        Mat imgcv = BitmapConverter.ToMat(image);
                        string wakeTime = DateTime.Now.ToString("h:mm:ss tt");
                        imgcv.PutText(wakeTime, new OpenCvSharp.Point(50, 50), 
                            OpenCvSharp.HersheyFonts.HersheySimplex, 1, OpenCvSharp.Scalar.Red);

                        image = BitmapConverter.ToBitmap(imgcv);
                        string filename = "-----CAPTURES-----/intrusion" + DateTime.Now.Ticks.ToString() + ".jpg";
                        image.Save(filename);
                        filenameList.Add(filename);

                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Image.Dispose();
                        }
                        pictureBox1.Image = image;
                        capture.Release();
                        counter += 1;

                        wakeList.Add(wakeTime);
                        richTextBox1.Text = counter.ToString();
                        listBox1.Items.AddRange(wakeList.ToArray());
                    }
                    break;
                case PowerModes.Suspend:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image = Image.FromFile(filenameList[listBox1.SelectedIndex]);
            }
            catch
            {
                pictureBox1.Image = Image.FromFile("msn.jpg");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @Application.StartupPath);
        }
    }
}