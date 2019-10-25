using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace TSP
{
    public partial class TSP
    {
        static TSPSet read = new TSPSet();

        static void Main(string[] args)
        {
            run();
        }
        static void run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int tcount = 4;
            List<Thread> threads = new List<Thread>();


            for (int i = 0; i < tcount; i++) threads.Add(new Thread(GreedyRun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(50000);
            lock (bestLock) lock (seedLock)
                    foreach (var t in threads) t.Abort();
            File.WriteAllText("./GreedyResult.txt", greedyBest.@out);
            Console.WriteLine("Greedy finished at: {0}", stopwatch.Elapsed);

            threads.Clear();
            for (int i = 0; i < tcount; i++) threads.Add(new Thread(HCRun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(50000);
            lock (bestLock) lock (seedLock)
                    foreach (var t in threads) t.Abort();
            File.WriteAllText("./HCResult.txt", hcBest.@out);
            Console.WriteLine("HC finished at: {0}", stopwatch.Elapsed);

            threads.Clear();
            for (int i = 0; i < tcount; i++) threads.Add(new Thread(SARun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(50000);
            lock (bestLock) lock (seedLock)
                    foreach (var t in threads) t.Abort();
            File.WriteAllText("./SAResult.txt", saBest.@out);
            Console.WriteLine("SA finished at: {0}", stopwatch.Elapsed);

            threads.Clear();


            threads.Add(new Thread(GARun));
            foreach (var t in threads) t.Start();
            Thread.Sleep(50000);

            lock (bestLock) lock (seedLock) lock (gaRun.killLock)
                        foreach (var t in threads) t.Abort();
            File.WriteAllText("./GAResult.txt", Evaluate(gaRun.Best,read).@out);
            Console.WriteLine("GA finished at: {0}", stopwatch.Elapsed);

        }
        static void testGA()
        {
            Stopwatch sw = new Stopwatch();

            sw.Reset(); sw.Start();
            int tcount = 4;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < tcount; i++) threads.Add(new Thread(GreedyIteration));
            foreach (var t in threads) t.Start();
            foreach (var t in threads)
            { while (t.IsAlive) Thread.Sleep(10); }
            threads.Clear();

            var ga = new TSP_YJ(greedyResultsForSA.Values, read);
            for (int i = 0; i < 100; i++) ga.Iteration();
            sw.Stop();
            Console.WriteLine(Evaluate(ga.Best, read).@out);
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }
        static void test()
        {


            Stopwatch sw = new Stopwatch();

            sw.Reset(); sw.Start();
            var result = new SimpleGreedy().Algo(read);
            Console.WriteLine(Evaluate(result, read).@out);
            var hc = new HillClimbing(result, read);
            try
            {
                for (int i = 0; i < 10000; i++)
                {
                    result = hc.Iteration();
                    //Console.WriteLine(hc.IterMsg);
                    if (i % 100 == 0) Console.WriteLine(">>>>>>>>>>>>>>>ev: {0}", Evaluate(result, read).score);
                }
            }
            catch (InvalidOperationException) { /*Console.WriteLine("Too much taboo - \n" + hc.IterMsg);*/ }
            sw.Stop();
            Console.WriteLine(Evaluate(result, read).@out);
            Console.WriteLine("Elapsed={0}", sw.Elapsed);

        }


        static object bestLock = new object();
        static SortedList<float, List<Node>> greedyResultsForHC = new SortedList<float, List<Node>>();
        static SortedList<float, List<Node>> greedyResultsForSA = new SortedList<float, List<Node>>();
        static List<List<Node>> allResiults = new List<List<Node>>();
        static EvalF greedyBest = new EvalF() { score = float.MaxValue };
        static EvalF hcBest = new EvalF() { score = float.MaxValue };
        static EvalF saBest = new EvalF() { score = float.MaxValue };

        static object seedLock = new object();
        static System.Random rng = new System.Random();
        static List<int?> gunbonseeds = new List<int?> { null, };//1961, 409, 741, 879, 2095, 9, 438, 0x1234, 77, 0xabcd, 1852992319, 1897423924, 526, 468, 557, 1326, 2061, 38, 37, 40, 173, 177, 187, 279, 408, 


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
            if (seed % 10 == 0) Console.WriteLine("greedy start with seed : " + seed);
            var result = new SimpleGreedy().Algo(read, seed);

            var greedyEval = Evaluate(result, read);
            greedyEval.seed = seed;

            lock (bestLock)
            {
                if (greedyBest.score > greedyEval.score)
                {
                    greedyBest = greedyEval;
                    Console.WriteLine("greedy new best(" + seed + ":" + greedyEval.score + ")");
                }
                allResiults.Add(result);
                greedyResultsForHC.Add(greedyEval.score, result);
                greedyResultsForSA.Add(greedyEval.score, result);
            }

        }

        static void GreedyLikeRandomIteration()
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
            if (seed % 10 == 0) Console.WriteLine("greedy start with seed : " + seed);
            var result = new Random().Algo(read, seed ?? 0);

            var greedyEval = Evaluate(result, read);
            greedyEval.seed = seed;

            lock (bestLock)
            {
                if (greedyBest.score > greedyEval.score)
                {
                    greedyBest = greedyEval;
                    Console.WriteLine("greedy new best(" + seed + ":" + greedyEval.score + ")");
                }
                greedyResultsForSA.Add(greedyEval.score, result);
            }

        }

        static int hcCnt = 0;
        static void HillClimbingIteration()
        {
            List<Node> result;
            int hcno;

            if (greedyResultsForHC.Count == 0) GreedyIteration();
            lock (seedLock)
            {
                result = greedyResultsForHC.ElementAt(0).Value;
                greedyResultsForHC.RemoveAt(0);
                hcno = hcCnt++;
            }

            Console.WriteLine("hc start : " + hcno);

            var hc = new HillClimbing(result, read);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                for (int i = 0; i < 1000 && sw.ElapsedTicks < 30000; i++)
                {
                    result = hc.Iteration();
                }
            }
            catch (InvalidOperationException) { /*Console.WriteLine("Too much taboo - \n" + hc.IterMsg);*/ }

            var eval = Evaluate(result, read);

            lock (bestLock)
            {
                if (hcBest.score > eval.score)
                {
                    hcBest = eval;
                    Console.WriteLine("hc new best : " + hcno + ", " + hcBest.score);

                    allResiults.Add(result);
                }
            }
        }

        static int saCount = 0;
        static void SARun() { for (; ; ) SAIteration(); }
        static void SAIteration()
        {
            List<Node> result;
            int hcno;

            if (greedyResultsForSA.Count == 0) GreedyIteration();
            lock (seedLock)
            {
                if (greedyResultsForSA.Count == 0) return;
                result = greedyResultsForSA.ElementAt(0).Value;
                greedyResultsForSA.RemoveAt(0);
                hcno = saCount++;
            }

            Console.WriteLine("hc start : " + hcno);

            var hc = new HillClimbing(result, read);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                for (int i = 0; i < 1000 && sw.ElapsedTicks < 30000; i++)
                {
                    result = hc.Iteration();
                }
            }
            catch (InvalidOperationException) { /*Console.WriteLine("Too much taboo - \n" + hc.IterMsg);*/ }

            var eval = Evaluate(result, read);

            lock (bestLock)
            {
                if (saBest.score > eval.score)
                {
                    saBest = eval;
                    Console.WriteLine("hc new best : " + hcno + ", " + hcBest.score);

                    allResiults.Add(result);
                }
            }
        }

        static TSP_YJ gaRun;
        static void GARun()
        {
            gaRun = new TSP_YJ(allResiults, read);
            for (; ; ) gaRun.Iteration();
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
