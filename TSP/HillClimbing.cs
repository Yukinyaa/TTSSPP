using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    public class HillClimbing
    {
        public int iteration { get; private set; } = 0;
        TSPSet nodes;
        List<Node> map;
        int?[] taboo;

        List<float> rmcost;

        public HillClimbing(List<Node> startCond, TSPSet nodes)
        {
            map = new List<Node>(startCond);
            taboo =  new int?[startCond.Count+1];
            for (int i = 0; i < taboo.Length ; i++)
                taboo[i] = -99999;
            rmcost = new List<float>();
            this.nodes = nodes;
        }

        void Select(Node a)
        {
            map.Add(a);
        }
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
        public float CalcRmCost(Node n)
        {
            return CalcRmCost(map.IndexOf(n));
        }
        public float CalcRmCost(int nIndexInMap)
        {
            if (nIndexInMap == 0) return nodes.EucDist(map[0], map[1]);
            else if (nIndexInMap == map.Count - 1) return nodes.EucDist(map[map.Count - 2], map[map.Count - 1]);
            return nodes.EucDist(map[nIndexInMap - 1], map[nIndexInMap]) + nodes.EucDist(map[nIndexInMap], map[nIndexInMap + 1]) - nodes.EucDist(map[nIndexInMap - 1], map[nIndexInMap + 1]);
        }
        public float CalcInsertCost(Node n, int where)
        {
            if (where == 0) return nodes.EucDist(map[0], n);
            else if (where == map.Count) return nodes.EucDist(map[map.Count - 1], n);
            return nodes.EucDist(n, map[where]) + nodes.EucDist(n, map[where - 1]) - nodes.EucDist(map[where], map[where - 1]);
        }

        const int tabooTimeout = 100;
        public StringBuilder IterMsg = new StringBuilder("iteration: 0");
        public Node? changed = null;
        int nested = 0;
        public List<Node> Iteration()
        {
            if (nested > 100) throw new InvalidOperationException();
            iteration++;
            IterMsg.Clear();

            

            IterMsg.Append("Iteration: " + iteration + "\n");
            //IterMsg.Append("taboo: " + (taboo.Count / (float)map.Count) + "\n");
            //if (taboo.Count >= map.Count)
            //    return map;


            var ogLoc = Enumerable.Range(0, map.Count - 1).Aggregate((a, b) => taboo[map[a].No-1] + tabooTimeout >= iteration ? b :
                                                                               taboo[map[b].No-1] + tabooTimeout >= iteration ? a :
                                                                               CalcRmCost(a) > CalcRmCost(b) ? a : b);

            var  sel = map[ogLoc];
            //var sel = map.Aggregate((a, b) => taboo.Exists(t => t.Item2.Equals(a)) ? b :
            //                                  taboo.Exists(t => t.Item2.Equals(b)) ? a :
            //                                  CalcRmCost(a) > CalcRmCost(b) ? a : b); // todo: optimize to number base

            changed = sel;
            var rmCost = CalcRmCost(ogLoc);
            map.RemoveAt(ogLoc);
            var pos = Enumerable.Range(0, map.Count - 1).Aggregate((a, b) => CalcInsertCost(sel, a) > CalcInsertCost(sel, b) ? b : a);
            var calculatedCost = CalcInsertCost(sel, pos);
            if (calculatedCost >= rmCost)
            {
                map.Insert(pos, sel);
                taboo[sel.No-1] = iteration;
                IterMsg.AppendLine("tabooed dun dun");
                iteration--;
                nested++;
                Iteration();
            }
            else
            {
                nested = 0;
                map.Insert(pos, sel);
            }
            IterMsg.AppendLine("Improved by: " + (rmCost - calculatedCost) );
            return map;
        }
        public List<Node> Algo()
        {
            return Iteration();
        }
    }

}
