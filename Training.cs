using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    internal static class Training
    {
        public const double DiscountCoeff = 0.90;                                // Коэффициент для discounted reward.

        internal const double EvaluationOfSolvedPosition = 100;
        internal const double CorrectionIfPositionRepeats = -1;
        internal const int MaxNodes = 1024;

        private const double LearningRate = 0.01;
        internal static Dictionary<Position, TrainTuple> DataBase { get; }
        private static double[][][] NetWeights { get; set; }                    // 1 - количество блоков, 2 - нейроны в блоке, 3 - веса конкретного нейрона.
        public static double[][] PolicyHeadWeights { get; private set; }
        public static double[] EvaluationHeadWeights { get; private set; }
        /// <summary>
        /// Метод, возвращающий индекс максимального элемента
        /// </summary>
        /// <param name="somePolicy"> входной массив</param>
        /// <returns> Ход, соответствующий индексу максимального элемента </returns>
        internal static Turns Argmax(double[] somePolicy)
        {
            Turns result = Turns.R;
            double max = double.MinValue;
            for (int i = 0; i < somePolicy.Length; i++)
            {
                if (max < somePolicy[i])
                {
                    max = somePolicy[i];
                    result = (Turns)i;
                }
            }
            return result;
        }
        /// <summary>
        /// Запоминание текущих весов нейросети.
        /// </summary>
        internal static void SaveNetWeights()
        {
            NetWeights = new double[NumBlocks][][];
            for (int i = 0; i < NumBlocks; i++)
            {
                for (int j = 0; j < Blocks[i].FCL.NumNeurons; j++)
                {
                    int max = Blocks[i].FCL.Neurons[j].InputQuantity;
                    Blocks[i].FCL.Neurons[j].Weights.CopyTo(NetWeights[i][j], 0);
                    NetWeights[i][j][max] = Blocks[i].FCL.Neurons[j].Bias;
                }
            }
            PolicyHeadWeights = new double[HeadPolicy.NumNeurons][];
            for (int i = 0; i < HeadPolicy.NumNeurons; i++)
            {
                HeadPolicy.Neurons[i].Weights.CopyTo(PolicyHeadWeights[i], 0);
                int max = HeadPolicy.Neurons[i].InputQuantity;
                PolicyHeadWeights[i][max] = HeadPolicy.Neurons[i].Bias;
            }
            EvaluationHeadWeights = new double[HeadEval.InputQuantity];
            HeadEval.Weights.CopyTo(EvaluationHeadWeights, 0);
            EvaluationHeadWeights[HeadEval.InputQuantity] = HeadEval.Bias;
        }
        // Метод, добавляющий новый тупл в базу.
        public static void AddTuple(Position position)
        {
            // Код ниже работает правильно. Передача чисел командой "CopyTo" проверена в "песочнице".
            if (DataBase.ContainsKey(position))
            {
                throw new Exception("Такой набор уже существует! Набор не был добавлен.");
            }
            else
            {
                TrainTuple newTuple = new TrainTuple();
                Blocks[0].FCL.Inputs.CopyTo(newTuple.NetInput, 0);
                for (int i = 0; i < Blocks.Length; i++)
                {
                    Blocks[i].Outputs.CopyTo(newTuple.InternalNetOutputs[i], 0);
                    newTuple.StDev[i] = Blocks[i].BNL.StandDev;
                }
                Policy.CopyTo(newTuple.SourcePolicy, 0);
                newTuple.Score = position.Evaluation;
                newTuple.Reward = 0;                                                    // TODO: определиться с Reward-ом.
                DataBase.Add(position, newTuple);
            }
        }
        // Метод корректировки весов нейронов слоя.
        // !!!!! Нужно очень грамотно собрать вектор градиента !!!!!
        internal static void CorrectWeights(double[] gradfrom, FCLayer layer)
        {
            int i = 0;
            foreach(Neuron neuron in layer.Neurons)
            {
                // Для функции сигмоида.
                //for (int j = 0; j < neuron.InputQuantity; j++)
                //{
                //    neuron.Weights[j] -= layer.Inputs[j] * neuron.Output * (1 - neuron.Output) * gradfrom[i] * LearningRate;
                //    // Ниже - ещё один способ записать корректировку.
                //    //neuron.Weights[j] -= layer.Inputs[j] * neuron.Output * (1 - neuron.Output) * gradfrom[Array.IndexOf(layer.Neurons, neuron)] * LearningRate;
                //}

                // Для функции RELU.
                for (int j = 0; j < neuron.InputQuantity; j++)
                {
                    if (neuron.Output >= 0)
                    {
                        neuron.Weights[j] -= layer.Inputs[j] * neuron.Output * gradfrom[i] * LearningRate;
                    }
                }
                i++;
            }
        }
        public static double[] BatchNormDerivation(double[] inputs)                 // TODO: Доработать метод (17.01.2021)
        {
            double[] result = new double[inputs.Length];

            return result;
        }
    }
}
