using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    public class Random
    {
        public List<Node> Algo(TSPSet nodes, int seed)
        {
            var pool = nodes.CopySet();
            System.Random rnd = new System.Random(seed);
            for (int i = 0; i < pool.Count; i++)
            {
                var rand = rnd.Next() % pool.Count;
                var tmp = pool[i];
                pool[i] = pool[rand];
                pool[rand] = tmp;
            }

            return pool;
        }
    }

}
