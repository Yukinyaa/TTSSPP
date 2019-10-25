using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class GA_HJ
    {

        public int iteration { get; private set; } = 0;
        TSPSet nodes;
        SortedDictionary<float, AdjRep> thisgeneration;
        System.Random rng = new System.Random();
        public List<Node> Best { get { return thisgeneration.First().Value.ToList(); } }
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
                int a = DistRng(prevGen.Count - 1), b = DistRng(prevGen.Count - 1);
                while(a==b) b = DistRng(prevGen.Count - 1);
                var mixed = AdjRep.Mix(
                  prevGen.ElementAt(a).Value,
                  prevGen.ElementAt(b).Value,
                  rng
                  );
                try
                {
                thisgeneration.Add(mixed.Eval(), mixed);
                } catch (ArgumentException) { }
                
            }
            //for (int i = 0; i < 100; i++) {
            //    try
            //    {
            //        var mixed = AdjRep.SelfMix(
            //      prevGen.ElementAt(DistRng(prevGen.Count - 1)).Value,
            //      rng
            //      ); thisgeneration.Add(mixed.Eval(), mixed);
            //    }
            //    catch (Exception) { }
            //}
            
            foreach(var a in prevGen)
                try { thisgeneration.Add(a.Key, a.Value); } catch (ArgumentException) { }
            
            while (thisgeneration.Count > 3000)//pool max
            {
                var rndnum = thisgeneration.Count - 1 - DistRng(thisgeneration.Count - 2);
                thisgeneration.Remove(thisgeneration.ElementAt(
                        rndnum
                    ).Key);//always leave rank 0
                //Console.Write("kill: {0} ", rndnum);
            }
            int prntcnt = 0;
            foreach (var a in thisgeneration)
            {
                if (prntcnt++ > 20) break;
                Console.Write("{0} ", a.Key);
            }
               
            Console.WriteLine();
        }
        //random 0~range, 0 has higher chance
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
