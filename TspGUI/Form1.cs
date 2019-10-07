using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSP;

namespace TspGUI
{
    public partial class Form1 : Form
    {
        TSPSet read = new TSPSet();
        List<Node> greedyRestult;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             greedyRestult = new Greedy().Algo(read);
            //TSP.TSP.Evaluate(greedtRestult, read);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int bd = 10;
            // Draw a string on the PictureBox.
            var pw = panel1.Width - bd * 2;
            var ph = panel1.Height - bd * 2;

            var gw = read.MaxX;
            var gh = read.MaxY;
            List<Node> result = new List<Node>(greedyRestult);
            Node prev = result[0];
            result.Remove(prev);
            float score = 0;
            int count = 0;

            foreach (var node in result)
            {
                count++;
                float percnetage = (float)count/read.CopySet().Count;
                Node a = prev, b = node;
                g.DrawLine(new Pen(Color.FromArgb(255, 0, (int)(255 - 255 * percnetage), 0)), new Point(a.X * pw / gw + bd, a.Y * ph / gh + bd), new Point(b.X * pw / gw + bd, b.Y * ph / gh + bd));

                score += read.EucDist(prev, node);
                prev = node;

            }
            foreach (var node in read.CopySet())
            {
                Node b = node;
                int x = b.X * pw / gw + bd, y = b.Y * ph / gh + bd;
                g.DrawRectangle(Pens.Red, x - 1, y - 1, 3 , 3);
            }

            g.DrawString("Score: " + score,
                 new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(30, 30));

        }

    }
}
