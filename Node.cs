using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CubeAgain
{
    public class Node// : IEquatable<Node>
    {
        private Dictionary<Turns, Step> steps { get; set; }
        public ImmutableDictionary<Turns, Step> Steps => steps.ToImmutableDictionary();
        public bool WasVisited { get; set; }
        public Position Position { get; }
        public Node(Position position)      // TODO: Обдумать как быть с закрытым словарём, Step-ами и обновлением пути - BackPropagation (2021-04-03)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            steps = Enum.GetValues(typeof(Turns)).Cast<Turns>().ToDictionary(val => val, val => new Step(new Move(), null));
            ImmutableDictionary<Turns, Step> Steps = steps.ToImmutableDictionary();
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
            steps[turn] = step;
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
            foreach (Turns turn in steps.Keys)
            {
                steps[turn].Move.Policy = NeuralNetwork.Policy[(int)turn];
            }
        }
        public void WinRateCorrection(Turns turn, double correction)
        {
            steps[turn].Move.WinRate += correction;
        }
        public void VisitCorrection(Turns turn, int correction)
        {
            steps[turn].Move.Visit += correction;
        }
    }
}
