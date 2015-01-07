using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DebugShellTest
{
    class Picture
    {
        int imageNumber;
        public byte[] imageData = new byte[0];
        public bool imageCompleted;

        public Picture(int imageNumber)
        {
            this.imageNumber = imageNumber;
        }

        /// <summary>
        /// The picture recives the data here.
        /// </summary>
        /// <param name="input">
        /// The raw input for the data comming from the UDP stream.
        /// </param>
        public void addData(byte[] input)
        {
            switch (getDataPackageType(input))
            {
                case "1":
                    addInitialDataToImage(input);
                    break;
                case "2":
                    addDataToImage(input);
                    break;
                case "3":
                    addDataToImage(input);
                    imageCompleted = true;
                    break;
                default:
                    imageData = new byte[0];
                    break;
            }
        }

        /// <summary>
        /// This devides the incomming UDP package in 3 types:
        /// 1. The starting package
        /// 2. The middle package(s)
        /// 3. The end package
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string getDataPackageType(byte[] input)
        {
            if (input[0] == 0x90 && input[1] == 0x60 && imageData.Length == 0)
                return "1";
            else if (input[0] == 0x80 && input[1] == 0x60 && imageData.Length != 0)
                return "2";
            else if (input[0] == 0x80 && input[1] == 0xe0 && imageData.Length != 0)
                return "3";
            else
                return "error";
        }

        private void addInitialDataToImage(byte[] input)
        {
            int i;
            for (i = 0; i < input.Length - 2; i++)
            {
                if (input[i] == 0xFF && input[i + 1] == 0xD8)
                {
                    imageData = removeFirstBytes(input, i);
                    break;
                }
            }
        }

        private void addDataToImage(byte[] data)
        {
            byte[] smallerImageData = removeFirstBytes(data, 12);
            var merged = new byte[imageData.Length + smallerImageData.Length];
            imageData.CopyTo(merged, 0);
            smallerImageData.CopyTo(merged, imageData.Length);
            imageData = merged;
        }

        public static byte[] removeFirstBytes(byte[] input, int bytesToRemove)
        {
            byte[] smallerData = new byte[input.Length - bytesToRemove];
            for (int i = 0; i < smallerData.Length; i++)
            {
                smallerData[i] = input[i + bytesToRemove];
            }
            return smallerData;
        }

        public BitmapImage getImage()
        {
            try
            {
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
                return image;
            }
            catch
            {
               return null;
            }
        }
    }
}
