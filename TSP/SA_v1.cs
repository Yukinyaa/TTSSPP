using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{ 
    public class SA_v1
    {
        public int iteration { get; private set; } = 0;
        TSPSet nodes;
        List<Node> map;
        System.Random rng = new System.Random();
        int?[] taboo;

        List<float> rmcost;

        public SA_v1(List<Node> startCond, TSPSet nodes)
        {
            map = new List<Node>(startCond);
            taboo = new int?[startCond.Count + 1];
            rmcost = new List<float>();
            this.nodes = nodes;
            ResetTaboo();
        }
        int tabooResetCount = 0;
        void ResetTaboo()
        {
            tabooResetCount++;
            if (tabooResetCount > 5)
                throw new InvalidOperationException();
            for (int i = 0; i < taboo.Length; i++)
                taboo[i] = -99999;
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

        int tabooTimeout = 2;
        float prevImp = 100;
        public StringBuilder IterMsg = new StringBuilder("iteration: 0");
        public Node? changed = null;
        int nested = 0;
        public float Temperture { get { return prevImp * 2; } }

        public List<Node> Iteration()
        {
            if (nested > nodes.size / 10) { ResetTaboo(); nested = 0; }
            iteration++;
            IterMsg.Clear();

            tabooTimeout = (int)(prevImp * 5 * (10 - tabooResetCount));

            IterMsg.Append("Iteration: " + iteration + "\n");
            //IterMsg.Append("taboo: " + (taboo.Count / (float)map.Count) + "\n");
            //if (taboo.Count >= map.Count)
            //    return map;

            int ogPos;
            if (rng.Next() > (1 / Temperture) * int.MaxValue) ogPos = rng.Next()%map.Count;
            else ogPos = Enumerable.Range(0, map.Count - 1).Aggregate((a, b) => taboo[map[a].No - 1] + tabooTimeout >= iteration ? b :
                                                                           taboo[map[b].No - 1] + tabooTimeout >= iteration ? a :
                                                                           CalcRmCost(a) > CalcRmCost(b) ? a : b);


            var sel = map[ogPos];
            //var sel = map.Aggregate((a, b) => taboo.Exists(t => t.Item2.Equals(a)) ? b :
            //                                  taboo.Exists(t => t.Item2.Equals(b)) ? a :
            //                                  CalcRmCost(a) > CalcRmCost(b) ? a : b); // todo: optimize to number base
            bool badMove = false;
            changed = sel;
            var rmCost = CalcRmCost(ogPos);
            map.RemoveAt(ogPos);
            var pos = Enumerable.Range(0, map.Count - 1).Aggregate((a, b) => b == ogPos ? a : CalcInsertCost(sel, a) > CalcInsertCost(sel, b) ? b : a);
            var calculatedCost = CalcInsertCost(sel, pos);
            
            while (rmCost - calculatedCost <= 0)
            {
                badMove = true;
                if (rmCost - calculatedCost != 0)
                    if (rng.Next() > (1 / -(rmCost - calculatedCost) * Temperture) * int.MaxValue )
                    break;   
                map.Insert(ogPos, sel);
                taboo[sel.No - 1] = iteration;
                IterMsg.AppendLine("tabooed_" + "(" + tabooResetCount + ", " + prevImp + ")");
                iteration--;
                nested++;
                Iteration();
                return map;
            }
            //else
            {
                
                map.Insert(pos, sel);

                float lpsFactor = 0.05f;
                if(!badMove) prevImp = (rmCost - calculatedCost) * lpsFactor + prevImp * (1 - lpsFactor);
                IterMsg.AppendLine("Improved by: " + (rmCost - calculatedCost) + "(" + Temperture + ", " + tabooTimeout + ")");
                if(badMove)IterMsg.Append("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                nested = 0;
            }
            return map;
        }
        public List<Node> Algo()
        {
            return Iteration();
        }
    }
}
