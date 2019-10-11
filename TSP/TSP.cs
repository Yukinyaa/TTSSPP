using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

namespace TSP
{
    public partial class TSP
    {
        static TSPSet read = new TSPSet();

        static void Main(string[] args)
        {
            int tcount = 12;
            List<Thread> threads = new List<Thread>();

            for(int i=0;i<tcount;i++)threads.Add(new Thread(GreedyRun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(int.MaxValue);
            lock(bestLock)lock(seedLock)
                foreach (var t in threads) t.Abort();
            File.WriteAllText("./GreedyResult.txt", greedyBest.@out);

            threads = new List<Thread>();
            for (int i = 0; i < tcount; i++) threads.Add(new Thread(HCRun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(50000);
            lock (bestLock) lock (seedLock)
                    foreach (var t in threads) t.Abort();
            File.WriteAllText("./HCResult.txt", hcBest.@out);

        }


        static object bestLock = new object();
        static List<List<Node>> greedyResiults = new List<List<Node>>();
        static EvalF greedyBest = new EvalF() { score = float.MaxValue };
        static EvalF hcBest = new EvalF() { score = float.MaxValue };

        static object seedLock = new object();
        static System.Random rng = new System.Random();
        static List<int> gunbonseeds = Enumerable.Range(0, 3000).ToList();
            //new List<int?> { null, 0x1234, 77, 0xabcd, 1852992319, 1897423924, 526};


        static void GreedyRun()
        {
            while (true)
            { GreedyIteration(); }
        }
        static void HCRun()
        {
            while (true)
            { HillClimbingIteration(); }
        }
        static void GreedyIteration()
        {
            int? seed;
            lock (seedLock)
            {
                if (gunbonseeds.Count == 0)
                    seed = rng.Next();
                else
                {
                    seed = gunbonseeds[0];
                    gunbonseeds.RemoveAt(0);
                }
            }
            Console.WriteLine("greedy start with seed : " + seed);
            var result = new Greedy_v4_Simple().Algo(read, seed);

            var greedyEval = Evaluate(result, read);
            greedyEval.seed = seed;

            lock (bestLock)
            {
                if (greedyBest.score > greedyEval.score)
                {
                    greedyBest = greedyEval;
                    Console.WriteLine("greedy new best(" + seed + ":" + greedyEval.score + ")");
                }
                greedyResiults.Add(result);
            }

        }

        int greedyCount = 0;
        static void HillClimbingIteration()
        {
            List<Node> result;
            lock (seedLock)
            {
                result = greedyResiults[0];
                greedyResiults.RemoveAt(0);
            }
            Console.WriteLine("hc start : " + result.GetHashCode());

            var hc = new HillClimbing(result, read);
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    result = hc.Iteration();
                }
            }
            catch (InvalidOperationException) { /*Console.WriteLine("Too much taboo - \n" + hc.IterMsg);*/ }

            var greedyEval = Evaluate(result, read);

            lock (bestLock)
            {
                if (hcBest.score > greedyEval.score)
                    hcBest = greedyEval;
            }
        }
        public struct EvalF
        {
            public int? seed;
            public float score;
            public int validation;
            public string @out;
            public override string ToString()
            {
                return @out;
            }
        }
        public static EvalF Evaluate(List<Node> result_, TSPSet input)
        {
            List<Node> pool = input.CopySet();
            var result = new List<Node>(result_);
            Node prev = result[0];
            var first = prev;
            EvalF returnno = new EvalF();

            result.Remove(prev);
            pool.Remove(prev);
            
            float score = 0;

            StringBuilder sb = new StringBuilder();
            foreach (var node in result)
            {
                pool.Remove(node);  
                score += input.EucDist(prev, node);
                
                prev = node;
            }
            sb.AppendFormat("score : {0}\n", returnno.score = score);
            sb.AppendFormat("validation : {0}\n", returnno.validation = pool.Count);
            sb.Append(first.No.ToString());
            foreach (var node in result)
            {
                sb.AppendFormat(" {0}",node.No);
            }
            returnno.@out = sb.ToString();
            return returnno;
        }
    }
}
