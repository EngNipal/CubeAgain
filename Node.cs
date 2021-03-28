using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CubeAgain
{
    public class Node// : IEquatable<Node>
    {
        protected readonly Dictionary<Turns, Step> steps;
        public ImmutableDictionary<Turns, Step> Steps { get; }
        public bool WasVisited { get; set; }                        // Признак посещения узла.
        public Position Position { get; }                           // Позиция, определяющая узел.
        public Node(Position position)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            steps = Enum.GetValues(typeof(Turns)).Cast<Turns>().ToDictionary(val => val, val => (Step)null); //new Step(new Move(), null));
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
        public void AddStep(Turns moves, Step step)
        {
            steps[moves] = step;
        }
        /// <summary>
        /// Метод, выбирающий лучший ход.
        /// </summary>
        /// <returns></returns>
        public Turns GetBestTurn()
        {
            Turns BestTurn = Turns.R;
            double Max = double.MinValue;
            foreach (Turns turn in Steps.Keys)                          // Определяем Turn с максимальным значением (Q + U) для соответствующего ему Move.
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
        
    }
}
