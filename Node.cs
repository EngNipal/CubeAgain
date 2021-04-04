using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeAgain
{
    public class Node
    {
        public readonly Dictionary<Turns, Step> Steps = new Dictionary<Turns, Step>();
        public bool WasVisited { get; set; }
        public Position Position { get; }
        public Node(Position position)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Steps = Enum.GetValues(typeof(Turns)).Cast<Turns>().ToDictionary(val => val, val => new Step(new Move(), null));
            WasVisited = false;
        }
        #region НЕ УДАЛЯТЬ ЭТОТ ЗАКОММЕНТИРОВАННЫЙ БЛОК!
        //public override int GetHashCode() => Position.GetHashCode();
        //public override bool Equals(object obj) => Equals(obj as Node);
        //public bool Equals(Node other) => other != null && other.Position.Equals(Position);
        #endregion
        /// <summary>
        /// Метод добавления нового шага к данному узлу.
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="step"></param>
        public void AddStep(Turns turn, Step step)
        {
            Steps[turn] = step;
        }
        /// <summary>
        /// Метод, выбирающий лучший ход.
        /// </summary>
        /// <returns></returns>
        public Turns GetBestTurn()  
        {
            Turns BestTurn = Turns.R;
            double Max = double.MinValue;
            foreach (Turns turn in Steps.Keys)
            {
                double Q = Steps[turn].Move.Quality;
                double P = Position.Evaluation;
                int N = Steps[turn].Move.Visit;
                double U = P / (1 + N);                                 // TODO: Уточнить эту функцию вычисления U.
                double UCB = Q + U;
                if (UCB > Max)
                {
                    Max = UCB;
                    BestTurn = turn;
                }
            }
            return BestTurn;
        }
        public void SetMovesPolicy()
        {
            foreach (Turns turn in Steps.Keys)
            {
                Steps[turn].Move.Policy = NeuralNetwork.Policy[(int)turn];
            }
        }
        public void WinRateCorrection(Turns turn, double correction)
        {
            Steps[turn].Move.WinRate += correction;
        }
        public void VisitCorrection(Turns turn, int correction)
        {
            Steps[turn].Move.Visit += correction;
        }
    }
}
