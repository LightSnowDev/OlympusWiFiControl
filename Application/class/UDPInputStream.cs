using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DebugShellTest
{
    class UDPInputStream
    {

        Picture pic;
        int picCounter = 0;
        int port = 0;
        public bool doneStreaming = false;

        /// <summary>
        /// UDPackageNumberCounter was made, because sometimes UDP packages
        /// don't come in the same order as they are send. So we remove the 
        /// ones which are not in order. 
        /// </summary>
        byte UDPackageNumberCounter = 0;

        /// <summary>
        /// custom event if a picture is correctly recieved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"> Bitmap Image</param>
        public delegate void newPictureEventHandler(object sender, BitmapImage data);
        public event newPictureEventHandler NewPictureReceived;

        public UDPInputStream(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// This starts the connection to the camera.
        /// </summary>
        public void startStream()
        {
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("192.168.0.10"), port);
            byte[] receivedRawData;
            try
            {
                while (!doneStreaming)
                {
                    receivedRawData = listener.Receive(ref groupEP);
                    analyseData(receivedRawData);
                    InvokeGUI();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
        }

        /// <summary>
        /// The 4. byte in an UDP package is a counting number.
        /// We check if this is the case with the following packages.
        /// 
        /// If everything is ok, a "Picture" is created
        /// </summary>
        /// <param name="input"></param>
        private void analyseData(byte[] input)
        {
            if (pic == null)
            {
                pic = new Picture(++picCounter);
            }

            UDPackageNumberCounter++;
            if (UDPackageNumberCounter == 255)
                UDPackageNumberCounter = 0;
            if (UDPackageNumberCounter != input[3])
            {
                //wrong package
                pic = null;
                UDPackageNumberCounter = input[3];
            }
            else
            {
                //correct package
                pic.addData(input);
                if (pic.imageCompleted)
                {
                    NewPictureReceived(this, pic.getImage());
                    pic = null;
                }
            }
        }

        private void InvokeGUI()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
            }
            catch
            { }
        }
    }
}
