using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    partial class TSP
    {
        class Greedy
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
            
            public List<Node> Algo(TSPSet nodes)
            {
                result = new List<Node>();
                pool = nodes.CopySet();

                Select(pool[0]);

                while (pool.Count != 0)
                {
                    List<Node> dists = new List<Node>(pool);
                    Node current = result[result.Count - 1];

                    pool.Sort(Comparer<Node>.Create( // todo: get this organized
                                (a,b) =>
                                (nodes.EucDist(a, current) == nodes.EucDist(b, current))?
                                0:
                                (nodes.EucDist(a,current) > nodes.EucDist(b,current))?1:-1
                            ));
                    var best5 = dists.ToList().GetRange(0,Math.Min(dists.Count,5));

                    best5.Sort(Comparer<Node>.Create(
                            (a, b) => (
                            (a.X==b.X)?
                                        ((nodes.EucDist(a, current) == nodes.EucDist(b, current)) ? 
                                            0 
                                            : 
                                            (nodes.EucDist(a, current) > nodes.EucDist(b, current)?1:0))
                            :
                            ((a.X>b.X)
                            ?1:-1)))
                    );
                    var minval = best5[0];
                    Select(minval);
                }
                
                return result;
             }
        }
    }
}
