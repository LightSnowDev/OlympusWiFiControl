using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DebugShellTest
{
    class PictureAnalysis
    {

        static byte[] imageData = new byte[0];
        static int counter = 0;
        static List<byte[]> headerArray = new List<byte[]>();
        static byte imagePacketNumberCounter = 0;

        public static void input(byte[] data)
        {
            imagePacketNumberCounter++;
            if (imagePacketNumberCounter == 255)
                imagePacketNumberCounter = 0;
            if (imagePacketNumberCounter != data[3])
            {
                imageData = new byte[0];
                imagePacketNumberCounter = data[3];
            }

            if (data[0] == 0x90 && data[1] == 0x60 && imageData.Length == 0)
            {
                //MainWindow.thisWindow.textBlock.Text = "Start";
                headerArray = new List<byte[]>();
                initDataToImage(data);
            }
            else if (data[0] == 0x80 && data[1] == 0x60 && imageData.Length != 0)
            {
                // MainWindow.thisWindow.textBlock.Text = "middle";
                addDataToImage(data);
            }
            else if (data[0] == 0x80 && data[1] == 0xe0 && imageData.Length != 0)
            {
                // MainWindow.thisWindow.textBlock.Text = "end";
                addFinalDataToImage(data);
                string x = "";
                foreach (byte[] b in headerArray)
                {
                    string toAdd = "\r\n";
                    foreach(byte sb in b)
                    {
                        toAdd += " " + ((sb)).ToString();
                    }
                    x += toAdd;
                }
                System.IO.File.WriteAllText(@"C:\Users\Jonathan\ima\file" + counter.ToString() + ".txt", x);
                imageData = new byte[0];
            }
            else
                imageData = new byte[0];
        }

        private static void initDataToImage(byte[] data)
        {
            int i;
            for (i = 0; i < data.Length - 2; i++)
            {
                if (data[i] == 0xFF && data[i + 1] == 0xD8)
                {
                    imageData = removeFirstBytes(data, i);
                    break;
                }
            }
        }
        private static void addFinalDataToImage(byte[] data)
        {
            addDataToImage(data);


            try
            {
                counter++;
                File.WriteAllBytes(@"C:\Users\Jonathan\ima\file" + counter.ToString() + ".jpg", imageData);

                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    //image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    //image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                MainWindow.thisWindow.imageMain.Source = image;
            }
            catch(Exception ex)
            {

            }
        }

        private static void addDataToImage(byte[] data)
        {
            byte[] smallerImageData = removeFirstBytes(data,12);
        }

        public static byte[] removeFirstBytes(byte[] input, int bytesToRemove)
        {
            byte[] smallerData = new byte[input.Length - bytesToRemove];
            byte[] removedData = new byte[bytesToRemove];
            for (int i = 0; i < smallerData.Length; i++)
            {
                if(i < bytesToRemove)
                    removedData[i] = input[i];
                smallerData[i] = input[i + bytesToRemove];
            }
            headerArray.Add(removedData);
            return smallerData;
        }


    }
}
