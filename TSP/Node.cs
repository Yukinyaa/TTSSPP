namespace TSP
{
    struct Node
    {
        public Node(int no, int x, int y)
        {
            No = no;
            X = x;
            Y = y;
        }
        public int No { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }
}
