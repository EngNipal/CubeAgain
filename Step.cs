namespace CubeAgain
{
    public class Step
    {
        public Move Move { get; set; }
        public Node Node { get; set; }

        public Step(Move move, Node node)
        {
            Move = move;
            Node = node;
        }
    }
}