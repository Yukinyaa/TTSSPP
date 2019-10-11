using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace TSP
{
    public partial class TSP
    {
        static void Main(string[] args)
        {
            var read = new TSPSet();
            var result = new Greedy_v4_Simple().Algo(read);

            Evaluate(result, read);
            var hc = new HillClimbing(result, read);
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    result = hc.Iteration();
                }
            }
            catch (InvalidOperationException) { Console.WriteLine("Too much taboo - \n"+hc.IterMsg); }
            Evaluate(result, read);
        }

        public static int Evaluate(List<Node> result_, TSPSet input)
        {
            List<Node> pool = input.CopySet();
            var result = new List<Node>(result_);
            Node prev = result[0];
            
            result.Remove(prev);
            pool.Remove(prev);
            float score = 0;
            foreach (var node in result)
            {
                pool.Remove(node);  
                score += input.EucDist(prev, node);

                //Console.WriteLine("score : " + score + "\tnode : " + node.No);
                prev = node;
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("score : " + score);
            Console.WriteLine("validation : " + pool.Count);

            Console.Write("1 ");
            foreach (var node in result)
            {
                Console.Write(node.No + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("score : " + score);
            Console.WriteLine("validation : " + pool.Count);
            return pool.Count;
        }
    }
}
