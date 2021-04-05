using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    static class Training
    {
        public const double DiscountCoeff = 0.90;
        public const double LearningRate = 0.01;
        public const double RegulCoeff = 0.001;

        public const double UnsolvedEvaluation = -1;
        public const double SolvedEvaluation = 100;
        public const int CorrectionIfPositionRepeats = -1;
        public const int MaxNodes = 1024;

        private const double Zero = 0.0;
        private static readonly Dictionary<Position, Dataset> DataBase = new Dictionary<Position, Dataset>();
        // 1 - количество блоков, 2 - нейроны в блоке, 3 - веса конкретного нейрона.
        private static double[][][] NetWeights { get; set; }                    
        public static double[][] PolicyHeadWeights { get; private set; }
        public static double[] EvaluationHeadWeights { get; private set; }
        /// <summary>
        /// Метод, возвращающий индекс максимального элемента
        /// </summary>
        /// <param name="somePolicy"> входной массив</param>
        /// <returns> Ход, соответствующий индексу максимального элемента </returns>
        public static Turns Argmax(double[] somePolicy)
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
        public static Dataset GetDatasetByPos(Position position)
        {
            if (!DataBase.ContainsKey(position))
            {
                AddDataset(position);
            }
            return DataBase[position];
        }
        /// <summary>
        /// Добавляет новый тупл в базу.
        /// </summary>
        /// <param name="position"></param>
        public static void AddDataset(Position position)
        {
            // Код ниже работает правильно. Передача чисел командой "CopyTo" проверена в "песочнице".
            if (DataBase.ContainsKey(position))
            {
                throw new Exception("Такой набор уже существует! Набор не был добавлен.");
            }
            else
            {
                Dataset newSet = new Dataset();
                Blocks[0].FCL.Inputs.CopyTo(newSet.NetInput, 0);
                for (int i = 0; i < Blocks.Length; i++)
                {
                    Blocks[i].Outputs.CopyTo(newSet.InternalNetOutputs[i], 0);
                    newSet.StandDev[i] = Blocks[i].BNL.StandDeviation;
                }
                Policy.CopyTo(newSet.SourcePolicy, 0);
                newSet.NetScore = position.Evaluation;
                // TODO: определиться с Reward-ом.
                newSet.Reward = 0;
                DataBase.Add(position, newSet);
            }
        }
        // *** Корректировка весов ***
        // TODO: Finish that block
        public static void WeightsCorrection(Dataset[] miniBatch)
        {
            // Подсчитываем Лосс-функцию.
            foreach (Dataset trainSet in miniBatch)
            {
                // z - Результат игры. Он зависит от длины пути, чем длиннее путь, тем хуже результат.
                double z = 100 / trainSet.PathLength;
                // v - Оценка нейросети сколько ходов ещё до конца из этой позиции.
                double v = trainSet.NetScore;
                // Квадрат разности между этими величинами - есть VLoss.
                double VLoss = z - v;
                VLoss *= VLoss;
                // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
                double RLoss = Zero;
                RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
                RLoss *= RegulCoeff;
                // PLoss - cross-entropy loss.
                double PLoss = Zero;
                for (int i = 0; i < trainSet.ImprovedPolicy.Length; i++)
                {
                    PLoss += trainSet.ImprovedPolicy[i] * (Zero - Math.Log(trainSet.SourcePolicy[i]));
                }
                double FullLoss = VLoss + PLoss + RLoss;

                for (int i = NumBlocks - 1; i >= 0; i--)
                {
                    //Blocks[i]
                }
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
