using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class GA_HJ
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
                    path[list[i].No - 1] = list[i + 1].No - 1;
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
                        if (path.IndexOf(b.path[i]) != -1)
                        {
                            path[path.IndexOf(b.path[i])] = path[i];
                            path[i] = b.path[i];
                        }
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

        public int iteration { get; private set; } = 0;
        TSPSet nodes;
        SortedDictionary<float, AdjRep> thisgeneration;
        System.Random rng = new System.Random();
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

        public GA_HJ(IList<List<Node>> startCond, TSPSet nodes)
        {
            thisgeneration = new SortedDictionary<float, AdjRep>();
            foreach (var a in startCond)
            {
                var rep = new AdjRep(nodes, a);
                thisgeneration.Add(rep.Eval(), rep);
                Console.Write(rep.Eval() + " ");
            }
            Console.WriteLine();
            this.nodes = nodes;
        }
        public void Iteration()
        {
            var prevGen = thisgeneration;
            thisgeneration = new SortedDictionary<float, AdjRep>();
            for (int i = 0; i < prevGen.Count; i++)
            {
                var mixed = AdjRep.Mix(
                    prevGen.ElementAt(DistRng(prevGen.Count - 1)).Value,
                    prevGen.ElementAt(DistRng(prevGen.Count - 1)).Value,
                    rng
                    );
                try { thisgeneration.Add(mixed.Eval(), mixed); } catch (ArgumentException) { }
                
            }
            foreach(var a in prevGen)
                try { thisgeneration.Add(a.Key, a.Value); } catch (ArgumentException) { }
            
            while (thisgeneration.Count > 100)
            {
                thisgeneration.Remove(thisgeneration.ElementAt(
                        thisgeneration.Count - 2 - DistRng(thisgeneration.Count - 2)
                    ).Key);//always leave rank 0
            }
            int prntcnt = 0;
            foreach (var a in thisgeneration)
            {
                if (prntcnt++ > 20) break;
                Console.Write("{0} ", a.Key);
            }
               
            Console.WriteLine();
        }
        //random 0~range, 0 has higher
        int DistRng(int range)
        {
            double u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return Math.Min((int)Math.Truncate(Math.Abs(0.3 * range * randStdNormal)), range); //random normal(mean,stdDev^2)
        }
    }
}
