namespace CubeAgain
{
    public class Step
    {
        public Move Move { get; set; }
        public Node NextNode { get; set; }
        public Step(Move move, Node nextnode)
        {
            Move = move;
            NextNode = nextnode;
        }
    }
}