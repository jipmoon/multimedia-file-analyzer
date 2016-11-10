using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaeProject
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Bitmap membmp;
        List<Bitmap> imagesCollected = new List<Bitmap>(NUMBER_OF_FILES);
        List<Bitmap> imagesProcessed = new List<Bitmap>(NUMBER_OF_FILES);

        Bitmap example = new Bitmap("Images/dog.jpg");

        String[] files = new String[NUMBER_OF_FILES];
        int currentFile = 0;
        const int NUMBER_OF_FILES = 728;
        const int WIDTH = 176;
        const int HEIGHT = 144;

        int N = 1 + (int)Math.Log(HEIGHT, 2);


        Thread t;
        System.Windows.Forms.Timer UpdateTimer;
        delegate void PixelFunc(int x, int y, Color c);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            files = Directory.GetFiles("OriginalVideoFrames/")
                                    .Select(path => Path.GetFileName(path))
                                    .ToArray();

            for (int image = 0; image < files.Length; image++)
            {
                imagesCollected.Add(new Bitmap("OriginalVideoFrames/" + files[image]));
            }

        }





        private void button1_Click(object sender, EventArgs e)
        {

            for(int i =0; i< 176; i++)
            {
                imagesProcessed[i].Save(Application.StartupPath + "/ProcessedVideo/img" + i + ".png" , 
                                        ImageFormat.Png);

            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            //int imageCounter = 0;

            for (int column = 0; column < 176; column++)
            {
                Bitmap imgLocal = new Bitmap(758, 144);

                for (int line = 0; line < 144; line++)
                {
                    for (int image = 0; image < files.Length; image++)
                    {
                        imgLocal.SetPixel(image, line, imagesCollected[image].GetPixel(column, line));
                    }
                }
                imagesProcessed.Add(imgLocal);
                
            }



            //pictureBox1.Image = imagesProcessed[0];
            //pictureBox2.Image = imagesProcessed[20];
            //pictureBox3.Image = imagesProcessed[49];

            /*for(int i=0; i<NUMBER_OF_FILES;i++)
            {
                pictureBox1.Image = imagesProcessed[i];

            }*/
        }

        private void convert2ndMethod_Click(object sender, EventArgs e)
        {
            
            int[ ,] oldHistogram = new int[N+1 , N+1];
            Bitmap imgResult = new Bitmap( files.Length, WIDTH + 1 );

            for (int image = 0; image < files.Length; image++)
            {

                for (int column = 0; column < 176; column++)
                {
                    int[,] histogram = new int[N + 1, N + 1];

                    for (int line = 0; line < 144; line++)
                    {
                        Bitmap imgLocal = imagesCollected[image];

                        Color pixel = imgLocal.GetPixel(column, line);


                       
                      
                        if ((int)pixel.R + (int)pixel.G + (int)pixel.B == 0)
                        {
                            histogram[0, 0]++;
                        }
                        else
                        {
                            double valueR = ((double)pixel.R / (pixel.R + pixel.G + pixel.B)) *N;
                            double valueG = ((double)pixel.G / (pixel.R + pixel.G + pixel.B)) * N;

                            int pixelR = (int) valueR ;
                            int pixelG = (int) valueG ;

                            histogram[pixelR, pixelG]++;
                        }
                        //imgLocal.SetPixel(image, line, pixel);
                    }

                    if (column == 0)
                    {
                        oldHistogram = histogram;
                    }

                    //Intersecting the two histograms
                    double summedNumber = 0;
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < N; j++)
                        {
                            summedNumber += (double)(Math.Min(histogram[i, j], oldHistogram[i, j])) / WIDTH;
                        }

                    }

                    oldHistogram = histogram;

                    Color finalPixel;
                    if (summedNumber == 0)
                    {
                        finalPixel = Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        int summedNumberInt = (int)Math.Floor(summedNumber * 255);
                        if (summedNumberInt > 255)
                        {
                            summedNumberInt = 255;
                        }
                        finalPixel = Color.FromArgb(summedNumberInt, summedNumberInt, summedNumberInt);
                    }

                    imgResult.SetPixel(image, column, finalPixel);
                }


            }
            imgResult.Save(Application.StartupPath + "/ProcessedVideo/part2.png",
                                        ImageFormat.Png);


            for(int i=0; i<files.Length; i++)
            {
                for(int j=0;j<WIDTH + 1; j++)
                {
                    if(imgResult.GetPixel(i,j).B>150)
                    {
                        Color thresholdPixel = Color.FromArgb(255, 255, 255);
                        imgResult.SetPixel(i, j, thresholdPixel);
                    }
                    else
                    {
                        Color thresholdPixel = Color.FromArgb(0, 0, 0);
                        imgResult.SetPixel(i, j, thresholdPixel);
                    }
                }
            }

            imgResult.Save(Application.StartupPath + "/ProcessedVideo/part2Threshold.png",
                            ImageFormat.Png);
        }

    }
}
