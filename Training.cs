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
        public const double DiscountCoeff = 0.90;
        public const double LearningRate = 0.01;

        internal const double EvaluationOfSolvedPosition = 100;                 // TODO: Ещё раз обдумать этот параметр (2021-01-18).
        internal const double CorrectionIfPositionRepeats = -1;
        internal const int MaxNodes = 1024;

        private static Dictionary<Position, TrainTuple> DataBase = new Dictionary<Position, TrainTuple>();
        // 1 - количество блоков, 2 - нейроны в блоке, 3 - веса конкретного нейрона.
        private static double[][][] NetWeights { get; set; }                    
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
        /// Запоминает текущие веса нейросети в полях Training,
        /// чтобы использовать их при дальнейшей корректировке весов.
        /// </summary>
        internal static void SaveNetWeights()
        {
            NetWeights = new double[NumBlocks][][];
            for (int i = 0; i < NumBlocks; i++)
            {
                NetWeights[i] = new double[Blocks[i].FCL.NumNeurons][];
                for (int j = 0; j < Blocks[i].FCL.NumNeurons; j++)
                {
                    int max = Blocks[i].FCL.NumInputs;
                    NetWeights[i][j] = new double[max + 1];
                    Blocks[i].FCL.Neurons[j].Weights.CopyTo(NetWeights[i][j], 0);
                    NetWeights[i][j][max] = Blocks[i].FCL.Neurons[j].Bias;
                }
            }
            PolicyHeadWeights = new double[HeadPolicy.NumNeurons][];
            for (int i = 0; i < HeadPolicy.NumNeurons; i++)
            {
                int max = HeadPolicy.NumInputs;
                PolicyHeadWeights[i] = new double[max + 1];
                HeadPolicy.Neurons[i].Weights.CopyTo(PolicyHeadWeights[i], 0);
                PolicyHeadWeights[i][max] = HeadPolicy.Neurons[i].Bias;
            }
            EvaluationHeadWeights = new double[HeadEval.InputQuantity + 1];
            HeadEval.Weights.CopyTo(EvaluationHeadWeights, 0);
            EvaluationHeadWeights[HeadEval.InputQuantity] = HeadEval.Bias;
        }
        /// <summary>
        /// Получение тупла по позиции
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Новый или существующий тупл, согласно позиции</returns>
        public static TrainTuple GetTupleByPos(Position position)
        {
            if (!DataBase.ContainsKey(position))
            {
                AddTuple(position);
            }
            return DataBase[position];
        }
        /// <summary>
        /// Добавляет новый тупл в базу.
        /// </summary>
        /// <param name="position"></param>
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
                    newTuple.StDev[i] = Blocks[i].BNL.StandDeviation;
                }
                Policy.CopyTo(newTuple.SourcePolicy, 0);
                newTuple.Score = position.Evaluation;
                newTuple.Reward = 0;                                                    // TODO: определиться с Reward-ом.
                DataBase.Add(position, newTuple);
            }
        }
        // TODO: Доработать метод BatchNormDerivation (2021-01-17).
        public static double[] BatchNormDerivation(double[] inputs)
        {
            double[] result = new double[inputs.Length];

            return result;
        }
    }
}
