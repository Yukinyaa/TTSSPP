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
            var greedtRestult = new Greedy().Algo(read);
            Evaluate(greedtRestult, read);
        }

        public static int Evaluate(List<Node> result, TSPSet input)
        {
            List<Node> pool = input.CopySet();
            Node prev = result[0];
            result.Remove(prev);
            pool.Remove(prev);
            float score = 0;
            foreach (var node in result)
            {
                pool.Remove(node);
                score += input.EucDist(prev, node);

                Console.WriteLine("score : " + score + "\tnode : " + node.No);
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
            return pool.Count;
        }
    }
}
