﻿namespace CubeAgain
{
    public class Move
    {
        public double Policy { get; set; }
        public int Visit { get; set; }
        public double WinRate { get; set; }
        public double Quality { get; private set; }
        public Move() : this(0.0, 0, 0.0)
        { }
        public Move(double policy, int visit, double winrate)
        {
            Policy = policy;
            Visit = visit;
            WinRate = winrate;
            SetQuality();
        }
        public void SetQuality()
        {
            Quality = (Visit == 0) ? double.PositiveInfinity : WinRate / Visit;
        }
    }
}