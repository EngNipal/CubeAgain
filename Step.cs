namespace CubeAgain
{
    public class Step
    {
        public Move Move { get; set; }
        public Position NextPos { get; set; }
        public Step(Move move, Position nextpos)
        {
            Move = move;
            NextPos = nextpos;
        }
    }
}