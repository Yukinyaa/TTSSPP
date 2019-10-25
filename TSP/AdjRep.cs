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
        public List<int> reversePath { get; private set; }
        public AdjRep(TSPSet set, List<Node> list)
        {
            this.set = set;
            path = new List<int>(Enumerable.Repeat(-1, list.Count));
            reversePath = new List<int>(Enumerable.Repeat(-1, list.Count));
            for (int i = 0; i < set.size - 1; i++)
            {
                path[list[i].No - 1] = list[i+1].No - 1;
                reversePath[list[i + 1].No - 1] = list[i].No - 1;
            }
        }
        public AdjRep(TSPSet set, List<int> list, List<int> reversePath)
        {
            this.set = set;
            path = list;
            this.reversePath = reversePath;
        }

        static public AdjRep Mix(AdjRep a, AdjRep b, System.Random rng, double bratio = 0.3, double mutateRatio = 0.000001)
        {
            var path = new List<int>(a.path);
            var reversepath = new List<int>(a.reversePath);
            int swp = (int)(bratio * path.Count * rng.NextDouble() * 2);
            for (int i = 0; i < a.set.size; i++)
            {
                bool twoopt = false;
                int twoptFrom = b.reversePath[i], twooptTo = b.path[i];
                swp--;

                if (bratio < rng.NextDouble())
                {
                    if (b.path[i] == -1)
                        twoptFrom = path.IndexOf(b.path[i]);
                    else
                        twoptFrom = reversepath[b.path[i]];
                    twooptTo = b.path[i];
                    twoopt = true;
                }
                else if (rng.Next() < mutateRatio) //random mutation
                {
                    twoptFrom = rng.Next(0, path.Count);
                    twooptTo = rng.Next(0, path.Count);
                    twoopt = true;
                }
                 if (twoopt)
                {
                    //two-opt b.rvp[i]~next, p[i]~next

                    if (twoptFrom != -1)
                        path[twoptFrom] = path[i];
                    if(path[i] != -1)
                        reversepath[path[i]] = twoptFrom;

                    path[i] = twooptTo;
                    if (twooptTo != -1)
                        reversepath[twooptTo] = i;
                }
            }
            return new AdjRep(a.set, path, reversepath);
        }
        static public AdjRep SelfMix(AdjRep a, System.Random rng, double mutateRatio = 0.0001)
        {
            var path = new List<int>(a.path);
            var reversepath = new List<int>(a.reversePath);
            for (int i = 0; i < a.set.size; i++)
            {
                if (rng.Next() < mutateRatio)//random mutation
                {
                    int twoptFrom = rng.Next(0, path.Count);
                    int twooptTo = rng.Next(0, path.Count);
                    path[twoptFrom] = path[i];
                    if (path[i] != -1)
                        reversepath[path[i]] = twoptFrom;

                    path[i] = twooptTo;
                    if (twooptTo != -1)
                        reversepath[twooptTo] = i;
                }
            }
            return new AdjRep(a.set, path, reversepath);
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
        public List<Node> ToList()
        {
            List<Node> a = new List<Node>();
            var q = set.CopySet();

            int next = -1;
            for (; ; )
            {
                next = path.IndexOf(next);
                if (next == -1) break;
                a.Add(q[next]);
            }

            a.Reverse();
            return a;
        }
    }
}
