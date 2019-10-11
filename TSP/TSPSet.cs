using System;
using System.Collections.Generic;
using System.IO;

namespace TSP
{
    public class TSPSet
    {
        private List<Node> dataSet;
        private float?[,] distCache;
        internal int size;

        private Node this[int i]
        {
            get { return dataSet[i]; }
        }
        public List<Node> CopySet()
        {
            return new List<Node>(dataSet);
        }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }

        public TSPSet(string path = "./rbx711.tsp.txt")
        {
            string name;
            int? dimension = null;
            MaxX = -1; MaxY = -1;
            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    var split = s.Split(new string[]{" : "}, StringSplitOptions.None);
                    switch (split[0].ToLower())
                    {
                        case "name":
                            Console.WriteLine("name: " + split[1]);
                            name = split[1];
                            break;
                        case "dimension":
                            Console.WriteLine("dimension: " + split[1]);
                            dimension = int.Parse(split[1]);
                            break;
                        default:
                            Console.WriteLine("skip: " + s);
                            break;
                    }
                    if (dimension != null) break;
                }
                dataSet = new List<Node>(dimension ?? -1); // should not be -1

                s = sr.ReadLine(); Console.WriteLine("skip: " + s);
                s = sr.ReadLine(); Console.WriteLine("skip: " + s);

                while (dataSet.Count < dimension)
                {
                    s = sr.ReadLine();
                    var split = s.Split(' ');
                    var newNode = new Node(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                    dataSet.Add(newNode);
                    MaxX = Math.Max(MaxX, newNode.X); // hack: optimizable
                    MaxY = Math.Max(MaxY, newNode.Y);
                }
            }
            size = dimension ?? -1;
            distCache = new float?[dimension ?? -1, dimension ?? -1];
        }


        public float EucDist(Node a, int x, int y)
        {
            float xd = a.X - x, yd = a.Y - y;
            return (float)Math.Sqrt(xd * xd + yd * yd);
        }
        public float EucDist(Node a, Node b)
        {
            return EucDist(a.No - 1, b.No - 1);
        }
        public float EucDist(int a, int b)
        {
            if (distCache[a, b] != null)
                return (float)distCache[a, b];// is never null here

            float xd = dataSet[a].X - dataSet[b].X, yd = dataSet[a].Y - dataSet[b].Y;
            distCache[a, b] = (float)Math.Sqrt(xd * xd + yd * yd);
            return (float)distCache[a, b];
        }
    }
}