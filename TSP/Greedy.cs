using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class Greedy
    {
        List<Node> result;
        List<Node> pool;

        public Greedy()
        {
        }

        void Select(Node a)
        {
            result.Add(a);
            pool.Remove(a);
        }
        class EucComparer : IComparer<Node>
        {
            public EucComparer(TSPSet nodes, Node current)
            {
                this.nodes = nodes;
                this.current = current;
            }
            TSPSet nodes;
            Node current;
            public int Compare(Node x, Node y)
            {
                float a = nodes.EucDist(x, current), b = nodes.EucDist(y, current);
                if (a == b) return 0;
                if (a > b) return 1;
                else return -1;
            }
        }

        class AxisPriorityComparer : IComparer<Node>
        {
            public AxisPriorityComparer(TSPSet nodes, Node current, Priority priority)
            {
                this.nodes = nodes;
                this.current = current;
                this.priority = priority;
            }
            TSPSet nodes;
            Node current;
            Priority priority;
            public enum Priority
            {
                XSmall,// Default Priority
                XBig, // Reversed Priority
                YSmall,
                YBig
            }
            public int Compare(Node a, Node b)
            {
                float axd = Math.Abs(a.X - current.X), bxd = Math.Abs(b.X - current.X);
                float ayd = Math.Abs(a.Y - current.Y), byd = Math.Abs(b.Y - current.Y);
                switch (priority)
                {
                    case Priority.XSmall:
                        if (axd == bxd)
                        {
                            if (ayd == byd) return 0;
                            else return ayd > byd ? 1 : -1;
                        }
                        else return axd > bxd ? 1 : -1;

                    case Priority.XBig:
                        if (axd == bxd)
                        {
                            if (ayd == byd) return 0;
                            else return ayd > byd ? 1 : -1;
                        }
                        else return axd < bxd ? 1 : -1;

                    case Priority.YSmall:
                        if (ayd == byd)
                        {
                            if (axd == bxd) return 0;
                            else return axd > bxd ? 1 : -1;
                        }
                        else return ayd > byd ? 1 : -1;
                    case Priority.YBig:
                        if (ayd == byd)
                        {
                            if (axd == bxd) return 0;
                            else return axd > bxd ? 1 : -1;
                        }
                        else return ayd < byd ? 1 : -1;
                }
                return 0;
            }
        }

        public List<Node> Algo(TSPSet nodes)
        {
            
            result = new List<Node>();
            pool = nodes.CopySet();

            Select(pool[0]);

            while (pool.Count != 0)
            {
                List<Node> dists = new List<Node>(pool);
                Node current = result[result.Count - 1];

                dists.Sort(new EucComparer(nodes, current));
                var best5 = dists.ToList().GetRange(0,Math.Min(dists.Count,5));

                best5.Sort(new AxisPriorityComparer(nodes, current, AxisPriorityComparer.Priority.XSmall));
                var minval = best5[0];
                Select(minval);
            }
                
            return result;
            }
    }

}
