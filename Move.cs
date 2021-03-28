namespace CubeAgain
{
    public class Move
    {
        // Вероятность выполнения этого хода, выданная NN.
        public double Policy { get; set; }
        // Количество проходов по ребру.
        public int Visit { get; set; }
        // Оценка выигрыша данным ходом.
        public double WinRate { get; set; }
        // Общее качество хода.
        private double quality;
        public double Quality
        {
            get
            { return quality; }
            private set
            {
                double dTemp = WinRate / Visit;
                quality = dTemp;
            }
        }
        //public Move() : this(0.0, 0, 0.0)
        //{ }
        public Move(double policy, int visit, double winrate)
        {
            Policy = policy;
            Visit = visit;
            WinRate = winrate;
        }
    }
}