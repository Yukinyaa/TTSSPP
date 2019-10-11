using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    public class Greedy_v3
    {
        List<Node> result;
        List<Node> pool;

        public Greedy_v3()
        {
        }

        void Select(Node a)
        {
            result.Add(a);
            pool.Remove(a);
        }
        void SelectAt(Node a, int pos)
        {
            result.Insert(pos, a);
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

        enum Direction { up, down }
        public List<Node> Algo(TSPSet nodes, int seed = 0)
        {

            result = new List<Node>();
            pool = nodes.CopySet();

            Direction direction = Direction.down;
            if (seed == 0)
            {
                Select(pool.Aggregate((x, y) => nodes.EucDist(x, 0, 0) < nodes.EucDist(y, 0, 0) ? x : y));
                Select(pool.Aggregate((x, y) => nodes.EucDist(x, 0, 0) < nodes.EucDist(y, 0, 0) ? x : y));
            }
            else
            {
                Node se;
                Select(se = pool[seed%pool.Count]);
                Select(pool.Aggregate((x, y) => nodes.EucDist(x, se) < nodes.EucDist(y, se) ? x : y));

            }

            while (pool.Count != 0)
            {
                var next = pool[0];
                int at = Enumerable.Range(0, result.Count - 1).Aggregate((a, b) => CalcInsertCost(next, a, nodes) < CalcInsertCost(next, b, nodes) ? a : b);

                SelectAt(next,at);
            }

            return result;
        }
        public float CalcInsertCost(Node n, int where, TSPSet nodes)
        {
            if (where == 0) return nodes.EucDist(result[0], n);
            else if (where == result.Count) return nodes.EucDist(result[result.Count - 1], n);
            return nodes.EucDist(n, result[where]) + nodes.EucDist(n, result[where - 1]) - nodes.EucDist(result[where], result[where - 1]);
        }
    }
}

