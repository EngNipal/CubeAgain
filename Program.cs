using System;
using System.Collections.Generic;
using System.Linq;
using static CubeAgain.Environment;
using static CubeAgain.NeuralNetwork;
using static CubeAgain.Training;

namespace CubeAgain
{
    class Program
    {
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
            SaveNetWeights();
            Analyzed += AddDataset;
            Random Rnd = new Random();
            SetScramble(CurrState, Rnd.Next(MinScrLength, MaxScrLength), out Turns[] NewScramble);
            Position CurrPos = new Position(CurrState);
            Node CurrNode = new Node(CurrPos);
            Path GamePath = new Path(CurrNode);
            const int TrainDatasetVolume = 128;
            Dataset[] MiniBatch = new Dataset[TrainDatasetVolume];
            for (int i = 0; i < TrainDatasetVolume; i++)
            {
                double[] ImprovedPolicy = GetProbDistrib(CurrNode);
                Dataset CurrDataset = GetDatasetByPos(CurrPos);
                ImprovedPolicy.CopyTo(CurrDataset.ImprovedPolicy, 0);
                CurrDataset.PathLength = GamePath.Length;
                CurrDataset.Reward = Math.Pow(DiscountCoeff, CurrDataset.PathLength) * UnsolvedEvaluation;

                Turns BestNetworkTurn = Argmax(ImprovedPolicy);
                GamePath.AddStep(CurrNode.Steps[BestNetworkTurn]);
                CurrPos = CurrPos.PosAfterTurn(BestNetworkTurn);
                CurrNode = NodeFromPosition(CurrPos);
                if (Solved.Equals(CurrPos))
                {
                    CurrDataset.PathLength++;
                    CurrDataset.Reward = Math.Pow(DiscountCoeff, CurrDataset.PathLength) * SolvedEvaluation;
                    CurrState = SetSolved();
                    SetScramble(CurrState, Rnd.Next(MaxScrLength, MaxScrLength));
                    CurrPos = new Position(CurrState);
                    Analyze(CurrPos);
                    CurrNode = NodeFromPosition(CurrPos);
                    GamePath.Clear();
                    GamePath.Start = CurrNode;
                }
                MiniBatch[i] = (Dataset)CurrDataset.Clone();
            }

            WeightsCorrection(MiniBatch);
            
            WriteState(CurrState);
        }
        /// <summary>
        /// Realizes MCTS logic. MCTS = Monte Carlo Tree Search.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="graph"></param>
        /// <returns>Improved probability distribution for received node.</returns>
        private static double[] GetProbDistrib(Node startNode)
        {
            Node currentNode = (Node)startNode.Clone();
            Path SearchPath = new Path(currentNode);
            for (int i = 0; i < MaxNodes; i++)
            {
                if (currentNode.WasVisited)
                {
                    Turns BestSearchTurn = currentNode.GetBestTurn();
                    SearchPath.AddStep(currentNode.Steps[BestSearchTurn]);
                    currentNode = NodeFromPosition(currentNode.Steps[BestSearchTurn].NextNode.Position);
                }
                else
                {
                    currentNode.WasVisited = true;
                    ExpandNode(currentNode, SearchPath);
                    SearchPath.BackPropagate();
                }
            }
            double[] result = new double[HeadPolicy.NumNeurons];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = currentNode.Steps[(Turns)i].Move.Visit;                     // TODO: Уточнить как именно выбирается ход. Лекция 14 в 
            }
            return result;
        }
        private static void ExpandNode(Node NewNode, Path path)
        {
            for (Turns turn = Turns.R; turn <= Turns.F2; turn++)
            {
                Position childPos = NewNode.Position.PosAfterTurn(turn);
                Node childNode = NodeFromPosition(childPos, out bool NodeExists);
                if (NodeExists && path.Contains(childPos))  // Если сделан возвратный ход или произошло зацикливание...
                {
                    NewNode.WinRateCorrection(turn, CorrectionIfPositionRepeats);
                }
                else // Такого узла ещё не было.
                {
                    double moveWinRate = Solved.Equals(childPos)
                        ? SolvedEvaluation
                        : childPos.Evaluation;
                    Move currMove = new Move(Policy[(int)turn], 0, moveWinRate);
                    Step currStep = new Step(currMove, childNode);
                    NewNode.AddStep(turn, currStep);
                }
            }
        }
    }
}
