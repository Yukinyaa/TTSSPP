using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class AdjRep
    {
        TSPSet set;
        public List<int> path { get; private set; }
        public AdjRep(TSPSet set, List<Node> list)
        {
            this.set = set;
            path = new List<int>(Enumerable.Repeat(-1, list.Count));
            for (int i = 0; i < set.size - 1; i++)
            {
                path[list[i].No - 1] = list[i+1].No - 1;
            }
        }
        public AdjRep(TSPSet set, List<int> list)
        {
            this.set = set;
            path = list;
        }

        static public AdjRep Mix(AdjRep a, AdjRep b, System.Random rng, double bratio = 0.3)
        {
            var path = new List<int>(a.path);
            for (int i = 0; i < a.set.size; i++)
            {
                if (rng.NextDouble() < bratio)
                {
                    path[path.IndexOf(b.path[i])] = path[i];
                    path[i] = b.path[i];
                }
            }
            return new AdjRep(a.set, path);
        }
        public float Eval() 
        {
            float sum = 0;
            for (int i = 0; i < set.size; i++)
            {
                if (path[i] == -1) continue;
                sum += set.EucDist(path[i], i);
            }
            return sum;
        }
    }
}
