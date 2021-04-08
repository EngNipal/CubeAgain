using System;
using System.Collections.Generic;
using System.Linq;
using static CubeAgain.Environment;
using static CubeAgain.Training;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    public class Node : ICloneable
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
        public void Expand(Path path)
        {
            for (Turns turn = Turns.R; turn <= Turns.F2; turn++)
            {
                Position childPos = Position.PosAfterTurn(turn);
                Node childNode = NodeByPosition(childPos, out bool NodeExists);
                if (NodeExists && path.Contains(childPos))
                {
                    Steps[turn].Move.WinRate += CorrectionIfRepeat;
                }
                else
                {
                    double moveWinRate = Solved.Equals(childPos)
                        ? SolvedEvaluation
                        : childPos.Evaluation;
                    Move currMove = new Move(Policy[(int)turn], 0, moveWinRate);
                    Step currStep = new Step(currMove, childNode);
                    AddStep(turn, currStep);
                }
            }
        }
        public void Visit()
        {
            WasVisited = true;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
