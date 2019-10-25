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
