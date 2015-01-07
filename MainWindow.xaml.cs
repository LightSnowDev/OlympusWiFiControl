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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow thisWindow;

        public MainWindow()
        {
            InitializeComponent();
            thisWindow = this;
        }

        public string make3(double x)
        {
            string output = x.ToString();
            if (output.Length == 1)
                return "0" + output;
            else
                return output;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int lineNumber = 0;
            foreach (string line in lines)
            {
                lineNumber++;
                analyseLine(lineNumber, line);
                double percent = Math.Round((((double)lineNumber / (double)lines.Length) * 100), 0);
                progressBar.Value = percent;
                textBlock.Text = "Line " + lineNumber.ToString() + " / " + lines.Length.ToString() + " Lines - " + make3(percent) + "%";
                //InvokeGUI();
            }
        }


        private void analyseLine(int lineNumber, string line)
        {
            //textBox2.Text += "\r\n" + webClass.GET(line); //

            textBox2.CaretIndex = textBox2.Text.Length;
            var rect = textBox2.GetRectFromCharacterIndex(textBox2.CaretIndex);
            textBox2.ScrollToHorizontalOffset(rect.Right);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button1_Click(object s, RoutedEventArgs eventsargs)
        {
            bool done = false;
            int port = 28488;//28488 18543
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("192.168.0.10"), port);
            string received_data;
            byte[] receive_byte_array;
            try
            {
                while (!done)
                {
                    receive_byte_array = listener.Receive(ref groupEP);
                    //textBox2.Text = BitConverter.ToString(receive_byte_array);
                    PictureAnalysis.input(receive_byte_array);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
        }

    }
}