using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageEnlarger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full file path of your Eclipse configuration file:\n");
            var dir = @Console.ReadLine();
            var ext = new List<string> { ".jpg", ".gif", ".png" };
            var myFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
               .Where(s => ext.Any(e => s.EndsWith(e))).ToList<string>();

            int height, width;
            try {
                Console.WriteLine("Enter Resize Height\t");
                height = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Resize Width\t");
                width = int.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("You must enter a valid integer for Height and Width");
                Console.ReadLine();
                return;
            }
            int count = 1;
            List<string> missed = new List<string>();
            foreach (string file in myFiles)
            {
                Console.Clear();
                Console.Write((((double)count++ / (double)myFiles.Count())*100).ToString("##.##") + "%");
                try
                {
                    using (FileStream bitmapFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        Image loaded = new Bitmap(bitmapFile);
                        Bitmap b = ResizeImage(loaded, height, width);
                        b.Save("temp");
                    }

                    File.Delete(file);

                    using (Image i = Bitmap.FromFile("temp"))
                    {
                        i.Save(file);
                    }

                    File.Delete("temp");
                }
                catch { missed.Add(file); }
            }
            Console.WriteLine("\nError resizing:\n");
            Console.WriteLine(string.Join("\n", missed));
            Console.ReadLine();
        }
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
