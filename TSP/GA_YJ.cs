using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class GA_YJ
    {
        List<Node> result;
        SortedDictionary<float, List<Node>> thisGeneration;
        TSPSet nodes;


        System.Random rng = new System.Random();
        public List<Node> Best { get { return thisGeneration.First().Value.ToList(); } }
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
        
        
        
        public GA_YJ(SortedDictionary<float, List<Node>> startCond, TSPSet nodes)
        {
            thisGeneration = new SortedDictionary<float, List<Node>>(startCond);
            Console.WriteLine();
            this.nodes = nodes;
        }

        public object killLock = new object();
        int generation = 0;
        public List<Node> CreateChild()
        {
            int momindex = DistRng(thisGeneration.Count - 1), dadindex = 0;
            while (momindex == dadindex) dadindex = DistRng(thisGeneration.Count - 1);
            List<Node> mom = thisGeneration.ElementAt(momindex).Value,
            dad = thisGeneration.ElementAt(dadindex).Value;
            
            List<Node> child = new List<Node>(mom);

            Dictionary<int, Node> momDict = new Dictionary<int, Node>();
            foreach (var node in mom)
                momDict.Add(node.No - 1, node);

            List<Node> factor = new List<Node>();
            List<Node> change = new List<Node>();
            // List<int> numChange = new List<int>();
            List<Node> difFact = new List<Node>();
            List<int> remove = new List<int>();
            List<Node> difChange = new List<Node>();
            List<int> insert = new List<int>();

            int rndm = rng.Next(0, nodes.size - 32);
            int dice = rng.Next(2, 32);

            for (int i = 0; i < dice; i++)
                factor.Add(dad[rndm + i]);

            int front = mom.IndexOf(factor[0]);

            int back = mom.IndexOf(factor[dice - 1]);

            int order = 1;
            if (front > back)
                order = -1;
            if (order == -1)
            {
                for (int i = 0; i < front - back + 1; i++)
                    change.Add(mom[back + i]);
                change.Reverse();
            }
            else
            {
                for (int i = 0; i < back - front + 1; i++)
                    change.Add(mom[front + i]);
            }
            //if(change.Count > child.Count/2)

            foreach (var gene in factor)
                if (change.IndexOf(gene) == -1)
                    difFact.Add(gene);
            //difChange = chang - fact

            foreach (var set in momDict)
                if (difFact.IndexOf(set.Value) != -1)
                    remove.Add(set.Key);
            //remove = mom기준 절대좌표로 뺄것들? 아무튼

            foreach (var gene in change)
                if (factor.IndexOf(gene) == -1)
                    difChange.Add(gene);
            //difFact = fact - change




            foreach (var set in momDict)
                if (difChange.IndexOf(set.Value) != -1)
                    insert.Add(set.Key);
            //

            foreach (var gene in change)
                child.Remove(gene);


            int ii = 0;


            if (order == -1)
                foreach (var gene in factor)
                    child.Insert(back + ii++, gene);
            else
                foreach (var gene in factor)
                    child.Insert(front + ii++, gene);


            ii = 0;
            foreach (var num in remove)
            {
                child.Remove(momDict[num]);
                ii += 1;
            }

            ii = 0;
            foreach (var num in insert)
            {
                var number = num + ii;
                child.Insert(number, difChange[ii]);
                ii += 1;
            }
            var hc = new HillClimbing(child, nodes);
            for (int i = 0; i < 50; i++) hc.Iteration();
            return hc.Iteration();

        }
        public void Iteration()
        {
            generation++;
            var newGen = new SortedDictionary<float, List<Node>>();
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    var mixed = CreateChild();
                    newGen.Add(TSP.Evaluate(mixed, nodes).score, mixed);
                }
                catch (Exception) { continue; }

            }

            foreach (var a in thisGeneration)
                try { newGen.Add(a.Key, a.Value); } catch (ArgumentException) { }

            lock(killLock)
                thisGeneration = newGen;

            while (thisGeneration.Count > 8)//pool max
            {
                var rndnum = thisGeneration.Count - 1 - DistRng(thisGeneration.Count - 2);
                thisGeneration.Remove(thisGeneration.ElementAt(
                        rndnum
                    ).Key);//always leave rank 0
                //Console.Write("kill: {0} ", rndnum);
            }
            int prntcnt = 0;
            Console.Write("Gen: {0} - ", generation);
            foreach (var a in thisGeneration)
            {
                if (prntcnt++ > 10) break;
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
