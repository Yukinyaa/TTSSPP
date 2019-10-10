using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class Greedy_v2
    {
        List<Node> result;
        List<Node> pool;

        public Greedy_v2()
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

        enum Direction { up, down }
        public List<Node> Algo(TSPSet nodes)
        {

            result = new List<Node>();
            pool = nodes.CopySet();

            Direction direction = Direction.down;
            Select(pool.Aggregate((x, y) => nodes.EucDist(x, 0, 0) < nodes.EucDist(y, 0, 0) ? x : y));
            
            while (pool.Count != 0)
            {
                Node current = result[result.Count - 1];
                List<Node> dists;
                int range = 50;

                do {
                    dists = pool.Where(node => Math.Abs(node.X - current.X) <= range && Math.Abs(node.Y - current.Y) <= range).ToList();
                    range *= 2;
                }
                while (dists.Count == 0);

                var minX = dists.Min(a => a.X);

                if (direction == Direction.up)
                    if (!dists.Exists(a => a.Y > current.Y))
                        direction = Direction.down;
                else //if direction == Direction.down
                    if (!dists.Exists(a => a.Y < current.Y))
                        direction = Direction.up;

                var next = dists.Where(a => a.X == minX).Aggregate((a, b) =>
                                                                        (
                                                                        ( (direction == Direction.up ? (a.Y > b.Y) : (a.Y < b.Y)) )
                                                                        ^
                                                                        (minX - current.X >= 0)
                                                                        ) ? a : b
                                                                );
                
                Select(next);
            }

            return result;
        }
    }

}
