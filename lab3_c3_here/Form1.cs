using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using triangle;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab3_c3_here
{
    public partial class Form1 : Form
    {
        public static Bitmap image;

        public class Vertex
        {
            public double x;
            public double y;
            public Color color;

            public Vertex(double X, double Y, Color Color)
            {
                x = X;
                y = Y; 
                color = Color;
            }
        }

        List<Vertex> vertexes = new List<Vertex>();

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.BackColor = Color.White;
            pictureBox1.Image = image;
        }
        
        public void Rasterization()
        {
            for (int y = 0; y < pictureBox1.Height; y++)
                for (int x = 0; x < pictureBox1.Width; x++)
                    image.SetPixel(x, y, Color.FromArgb((int)ShadeBackgroundPixel(x, y)));
            vertexes.Clear();
        }

        public static uint ColorToUint(Color c)
        {
            uint u = (UInt32)c.A << 24;
            u += (UInt32)c.R << 16;
            u += (UInt32)c.G << 8;
            u += c.B;
            return u;
        }

        public UInt32 ShadeBackgroundPixel(int x, int y)
        {
            double x1 = vertexes[0].x;
            double y1 = vertexes[0].y;
            double x2 = vertexes[1].x;
            double y2 = vertexes[1].y;
            double x3 = vertexes[2].x;
            double y3 = vertexes[2].y;

            UInt32 colorA = ColorToUint(vertexes[0].color);
            UInt32 colorB = ColorToUint(vertexes[1].color);
            UInt32 colorC = ColorToUint(vertexes[2].color);

            double l1, l2, l3;
            int i;
            UInt32 currentColor = 0xFFFFFFFF;
            l1 = ((y2 - y3) * ((double)(x) - x3) + (x3 - x2) * ((double)(y) - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            l2 = ((y3 - y1) * ((double)(x) - x3) + (x1 - x3) * ((double)(y) - y3)) /  ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            l3 = 1 - l1 - l2;

            if ((l1 >= 0 && l1 <= 1) && (l2 >= 0 && l2 <= 1) && (l3 >= 0 && l3 <= 1))
            {
                currentColor = (UInt32)0xFF000000 |
                    ((UInt32)(l1 * ((colorA & 0x00FF0000) >> 16) + l2 * ((colorB & 0x00FF0000) >> 16) + l3 * ((colorC & 0x00FF0000) >> 16)) << 16) |
                    ((UInt32)(l1 * ((colorA & 0x0000FF00) >> 8) + l2 * ((colorB & 0x0000FF00) >> 8) + l3 * ((colorC & 0x0000FF00) >> 8)) << 8) |
                    (UInt32)(l1 * (colorA & 0x000000FF) + l2 * (colorB & 0x000000FF) + l3 * (colorC & 0x000000FF));
            }
            return currentColor;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs && vertexes.Count < 3)
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    image.SetPixel(((MouseEventArgs)e).X - 1, ((MouseEventArgs)e).Y - 1, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y + 1, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y - 1, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X + 1, ((MouseEventArgs)e).Y, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X - 1, ((MouseEventArgs)e).Y, colorDialog1.Color);
                    image.SetPixel(((MouseEventArgs)e).X + 1, ((MouseEventArgs)e).Y + 1, colorDialog1.Color);

                    vertexes.Add(new Vertex(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y, colorDialog1.Color));
                }
                pictureBox1.Image = image;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (vertexes.Count > 0)
            {
                Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));

                Rasterization();
                pictureBox1.Image = image;
            }
        }
    }
}