using System;
using System.Collections.Generic;
using static CubeAgain.Environment;
using static CubeAgain.NeuralNetwork;
using static CubeAgain.Training;

namespace CubeAgain
{

    class Program
    {
        // TODO: Write tests for solution methods (2021-04-16).
        private static readonly Random Rnd = new Random();
        internal static Position CurrPos { get; set; }
        private static Node CurrNode { get; set; }
        private static Path GamePath { get; set; }
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
            //SaveNetWeights();
            Analyzed += AddDataset;
            SetScramble(CurrState, Rnd.Next(MinScrLength, MaxScrLength), out Turns[] NewScramble);
            CurrPos = new Position(CurrState);
            CurrNode = new Node(CurrPos);
            GamePath = new Path(CurrNode);
            const int TrainDatasetVolume = 128;
            Dataset[] MiniBatch = new Dataset[TrainDatasetVolume];
            for (int i = 0; i < TrainDatasetVolume; i++)
            {
                double[] ImprovedPolicy = ProbDistrib(CurrNode);
                Dataset CurrDataset = DatasetByPos(CurrPos);
                CurrDataset.CompleteUnsolved(ImprovedPolicy, GamePath.Length);
                Turns BestNetworkTurn = GetTurnByDistrib(ImprovedPolicy);
                GamePath.AddStep(CurrNode.Steps[BestNetworkTurn]);
                CurrPos = CurrPos.PosAfterTurn(BestNetworkTurn);
                CurrNode = NodeByPosition(CurrPos);
                if (Solved.Equals(CurrPos))
                {
                    // TODO: Обновить все датасеты, согласно GamePath - указать истинное кол-во ходов, которое потребовалось для достижения solved position (2021-04-12).
                    CurrDataset.CompleteSolved();
                    CurrState = SetSolved();
                    SetScramble(CurrState, Rnd.Next(MinScrLength, MaxScrLength));
                    CurrPos = new Position(CurrState);
                    Analyze(CurrPos);
                    CurrNode = NodeByPosition(CurrPos);
                    GamePath.Clear();
                    GamePath.Start = CurrNode;
                }
                MiniBatch[i] = (Dataset)CurrDataset.Clone();
            }

            CorrectNetWeights(MiniBatch);
            
            //WriteState(CurrState);
        }
        /// <summary>
        /// Realizes MCTS logic. MCTS = Monte Carlo Tree Search.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="graph"></param>
        /// <returns>Improved probability distribution for received node.</returns>
        private static double[] ProbDistrib(Node startNode)
        {
            Node currentNode = (Node)startNode.Clone();
            Path SearchPath = new Path(currentNode);
            for (int i = 0; i < MaxNodes; i++)
            {
                if (currentNode.WasVisited)
                {
                    Turns BestSearchTurn = currentNode.GetBestTurn();
                    SearchPath.AddStep(currentNode.Steps[BestSearchTurn]);
                    currentNode = NodeByPosition(currentNode.Steps[BestSearchTurn].NextNode.Position);
                }
                else
                {
                    currentNode.Visit();
                    currentNode.Expand(SearchPath);
                    SearchPath.BackPropagate();
                    SearchPath.Clear();
                }
            }
            double[] result = new double[HeadPolicy.NumNeurons];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = startNode.Steps[(Turns)i].Move.Visit;       // TODO: Уточнить как именно считается распределение "пи" Лекция 14 и статья.
            }
            return result;
        }
    }
}
