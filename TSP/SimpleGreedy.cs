using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class SimpleGreedy
    {
        List<Node> result;
        List<Node> pool;

        public SimpleGreedy()
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

        public List<Node> Algo(TSPSet nodes, int? startpoint = null)
        {

            result = new List<Node>();
            pool = nodes.CopySet();

            if (startpoint == null)
                Select(pool.Aggregate((x, y) => nodes.EucDist(x, 0, 0) < nodes.EucDist(y, 0, 0) ? x : y));
            else
                Select(pool[(startpoint ?? 0) % pool.Count]);

            while (pool.Count != 0)
            {
                Node current = result[result.Count - 1];
                
                var minval = pool.Aggregate((x, y) => nodes.EucDist(x, current) < nodes.EucDist(y, current) ? x : y);
                Select(minval);
            }

            return result;
        }
    }

}
