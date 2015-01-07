using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DebugShellTest
{
    /// <summary>
    /// Interaction logic for LiveView.xaml
    /// </summary>
    public partial class LiveView : Window
    {
        public LiveView()
        {
            InitializeComponent();
            web = new webClass();
        }

        webClass web;

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            //Get the name of the camera
            this.Title = "loading...";
            this.Title = Regex.Split(Regex.Split(web.GET("get_caminfo.cgi"), "<model>")[1], "</model>")[0];

            //start the video stream
            web.GET("switch_cammode.cgi?mode=play");
            web.GET("switch_cammode.cgi?mode=rec&lvqty=0320x0240");
            web.GET("exec_takemisc.cgi?com=startliveview&port=28488");

            //display the video stream
            UDPInputStream stream = new UDPInputStream(28488);
            stream.NewPictureReceived += Stream_NewPictureReceived;
            stream.startStream();
        }

        private void Stream_NewPictureReceived(object sender, BitmapImage data)
        {
            image.Source = data;
        }

        private void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            web.GET("exec_takemisc.cgi?com=stopliveview");
        }
    }
}
