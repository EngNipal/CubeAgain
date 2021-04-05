using System;
using System.Collections.Generic;
using System.Linq;
using static CubeAgain.Environment;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    class Program
    {
        private const double Zero = 0.0;
        /// <summary>
        /// Main Logic
        /// 0. Set position by scrambling.
        /// 1. Evaluate position by neural network.
        /// 2. Store the evaluation in training set.
        /// 3. Receive improved evaluations after tree search.
        /// 4. Store new evaluations in training set separate from previous evaluations.
        /// 5. Select best move and make it - get new position.
        /// 6. Repeat steps 1-5 until training set is full.
        /// 7. Correct weights in neural network using training set.
        /// 8. Repeat steps 0-7 until we've got best NN.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IEnumerable<int> CurrState = SetSolved();
            SetNetworkStructure();
            Training.SaveNetWeights();
            Analyzed += Training.AddTuple;
            Random Rnd = new Random();
            SetScramble(CurrState, Rnd.Next(1, 16), out Turns[] NewScramble);
            Position CurrentPos = new Position(CurrState);
            Node CurrentNode = new Node(CurrentPos);
            Path GamePath = new Path(CurrentNode);
            Graph MainGraph = new Graph();
            const int TrainDataSetVolume = 128;
            TrainTuple[] MiniBatch = new TrainTuple[TrainDataSetVolume];
            for (int Tau = 0; Tau < TrainDataSetVolume; Tau++)
            {
                double[] ImprovedPolicy = GetProbDistrib(CurrentNode, MainGraph);
                TrainTuple CurrentTuple = Training.GetTupleByPos(CurrentPos);
                ImprovedPolicy.CopyTo(CurrentTuple.ImprovedPolicy, 0);              // TODO: Разобраться почему приходится отдельно задавать ImprovedPolicy для тупла
                CurrentTuple.PathLength = GamePath.Length;                          // TODO: Разобраться почему приходится отдельно задавать длину пути для тупла.
                MiniBatch[Tau] = CurrentTuple;
                Turns BestTurn = Training.Argmax(ImprovedPolicy);
                GamePath.AddStep(CurrentNode.Steps[BestTurn]);
                CurrentPos = CurrentPos.PosAfterTurn(BestTurn);
                CurrentNode = MainGraph.NodeFromPosition(CurrentPos);
                if (Solved.Equals(CurrentPos))
                {
                    CurrentTuple.PathLength++;
                    CurrentTuple.Reward = 1;                                        // TODO: определиться с Reward-ом.
                    CurrState = SetSolved();
                    SetScramble(CurrState, Rnd.Next(1, 16));
                    CurrentPos = new Position(CurrState);
                    Analyze(CurrentPos);
                    CurrentNode = MainGraph.NodeFromPosition(CurrentPos);
                    GamePath.Clear();
                    GamePath.Start = CurrentNode;
                }
            }
            #region TODO: Finish that block
            // *** Корректировка весов ***
            const double RegulCoeff = 0.001;                                            // Гиперпараметр регуляризации.
            // Подсчитываем Лосс-функцию.
            foreach (TrainTuple tuple in MiniBatch)
            {
                double z = GamePath.Length - tuple.PathLength;                    // z - Количество ходов, которое реально прошло до решенной позиции
                double v = tuple.Score;                                               // v - Оценка нейросети сколько ходов ещё до конца из этой позиции.
                double VLoss = (z - v) * (z - v);                                       // Квадрат разности между этими величинами - есть VLoss.
                double RLoss = Zero;                                                     // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
                RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
                RLoss *= RegulCoeff;
                double PLoss = Zero;                                                     // PLoss - cross-entropy loss.
                for (int i = 0; i < tuple.ImprovedPolicy.Length; i++)
                {
                    PLoss += tuple.ImprovedPolicy[i] * (Zero - Math.Log(tuple.SourcePolicy[i]));
                }
                double FullLoss = VLoss + PLoss + RLoss;

                for (int i = NumBlocks - 1; i >= 0; i--)
                {
                    //Blocks[i]
                }
            }
            #endregion
            WriteState(CurrState);
        }
        /// <summary>
        /// Realizes MCTS logic. MCTS = Monte Carlo Tree Search.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="graph"></param>
        /// <returns>Improved probability distribution for received node.</returns>
        private static double[] GetProbDistrib(Node currentNode, Graph graph)
        {
            Path SearchPath = new Path(currentNode);
            for (int i = 0; i < Training.MaxNodes; i++)
            {
                if (currentNode.WasVisited)
                {
                    Turns BestTurn = currentNode.GetBestTurn();
                    SearchPath.AddStep(currentNode.Steps[BestTurn]);
                    currentNode = graph.NodeFromPosition(currentNode.Steps[BestTurn].NextNode.Position);
                }
                else
                {
                    currentNode.WasVisited = true;
                    ExpandNode(currentNode, graph, SearchPath);
                    SearchPath.BackPropagate();
                }
            }
            double[] result = new double[HeadPolicy.NumNeurons];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = currentNode.Steps[(Turns)i].Move.Visit;                     // TODO: Уточнить как именно выбирается ход.
            }
            return result;
        }
        private static void ExpandNode(Node NewNode, Graph graph, Path path)
        {
            for (Turns turn = Turns.R; turn <= Turns.F2; turn++)
            {
                Position childPos = NewNode.Position.PosAfterTurn(turn);
                Node childNode = graph.NodeFromPosition(childPos, out bool NodeExists);
                if (NodeExists && path.Contains(childPos))  // Если сделан возвратный ход или произошло зацикливание...
                {
                    NewNode.WinRateCorrection(turn, Training.CorrectionIfPositionRepeats);
                }
                else // Такого узла ещё не было.
                {
                    double moveWinRate = Solved.Equals(childPos)
                        ? Training.EvaluationOfSolvedPosition
                        : childPos.Evaluation; // * Math.Pow(Training.DiscountCoeff, path.Length);
                    Move currMove = new Move(Policy[(int)turn], 0, moveWinRate);
                    Step currStep = new Step(currMove, childNode);
                    NewNode.AddStep(turn, currStep);
                }
            }
        }
    }
}
