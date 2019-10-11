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
        List<Node> result;
        HillClimbing hc;
        public Form1()
        {
            InitializeComponent();
            result = new TSP.Greedy_v4_Simple().Algo(read);
            hc = new HillClimbing(result, read);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            List<Node> result_ = new List<Node>(result);
            Node prev = result_[0];
            result_.Remove(prev);
            float score = 0;
            int count = 0;

            foreach (var node in result_)
            {
                count++;
                float percnetage = (float)count/read.CopySet().Count;
                Node a = prev, b = node;
                float red = Math.Max(0, percnetage * 2 - 1);
                float green = Math.Min(percnetage * 2, 2 - percnetage * 2);
                float blue = Math.Max(0, 1 - percnetage * 2);
                g.DrawLine(new Pen(Color.FromArgb(150, (int)(255 * red), (int)(255 * green),(int)(255 * blue))),
                                         new Point(a.X * pw / gw + bd, a.Y * ph / gh + bd), new Point(b.X * pw / gw + bd, b.Y * ph / gh + bd));

                score += read.EucDist(prev, node);
                prev = node;

            }
            foreach (var node in read.CopySet())
            {
                Node b = node;
                int x = b.X * pw / gw + bd, y = b.Y * ph / gh + bd;
                g.DrawRectangle(Pens.Red, x - 1, y - 1, 2 , 2);
            }
            if (hc.changed != null)
            {

                int x = hc.changed.Value.X * pw / gw + bd, y = hc.changed.Value.Y * ph / gh + bd;
                g.DrawRectangle(Pens.Blue, x - 1, y - 1, 2, 2);
                g.DrawRectangle(Pens.Red, x - 2, y - 2, 4, 4);

            }


            g.DrawString(hc.IterMsg+"\nScore: " + score,
                 new Font("Arial", 10), System.Drawing.Brushes.Blue, new Point(30, 30));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0;i<10;i++)
                result = hc.Iteration();
            panel1.Refresh();
        }
    }
}
