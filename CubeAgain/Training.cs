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
        public const double DiscountCoeff = 0.95;
        public const double LearningRate = 0.01;
        public const double RegulCoeff = 0.001;
        public const double Epsilon = 0.000001;

        public const double UnsolvedEvaluation = -1;
        public const double SolvedEvaluation = 100;
        public const int CorrectionIfRepeat = -1;
        public const int MaxNodes = 1024;

        private static readonly int NumOfTurns = Enum.GetValues(typeof(Turns)).Length;

        private static readonly Dictionary<Position, Dataset> Base = new Dictionary<Position, Dataset>();
        // 1 - количество блоков, 2 - нейроны в блоке, 3 - веса конкретного нейрона.
        //private static double[][][] NetWeights { get; set; }                    
        //public static double[][] PolicyHWeights { get; private set; }
        //public static double[] EvalHWeights { get; private set; }
        /// <summary>
        /// Метод, выбирающий лучший ход из распределения.
        /// Ход с максимальным количеством посещений.
        /// </summary>
        /// <param name="somePolicy"> входной массив</param>
        /// <returns> Ход, соответствующий индексу максимального элемента </returns>
        public static Turns GetTurnByMax(double[] somePolicy)
        {
            CheckPolicyLength(somePolicy);
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
        /// Метод, выбирающий ход из распределения случайным образом.
        /// </summary>
        /// <param name="somePolicy"></param>
        /// <returns></returns>
        public static Turns GetTurnByDistrib(double[] somePolicy)
        {
            CheckPolicyLength(somePolicy);
            double[] temp = Normalize(somePolicy);
            Turns result = Turns.R;
            var rnd = new Random().NextDouble();
            double sum = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                sum += temp[i];
                if (sum > rnd)
                {
                    result = (Turns)i;
                    break;
                }
            }
            return result;
        }
        private static void CheckPolicyLength(double[] somePolicy)
        {
            if (somePolicy.Length != NumOfTurns)
            {
                throw new Exception("Количество элементов Policy не соответствует количеству возможных ходов.");
            }
        }
        private static double[] Normalize(double[] array)
        {
            double sum = array.Sum();
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i] / sum;
            }
            return result;
        }
        /// <summary>
        /// Запоминает текущие веса нейросети в полях Training,
        /// чтобы использовать их при дальнейшей корректировке весов.
        /// </summary>
        //internal static void SaveNetWeights()
        //{
        //    NetWeights = new double[NumBlocks][][];
        //    for (int i = 0; i < NumBlocks; i++)
        //    {
        //        NetWeights[i] = new double[Blocks[i].FCL.NumNeurons][];
        //        for (int j = 0; j < Blocks[i].FCL.NumNeurons; j++)
        //        {
        //            int max = Blocks[i].FCL.NumInputs;
        //            NetWeights[i][j] = new double[max + 1];
        //            Blocks[i].FCL.Neurons[j].Weights.CopyTo(NetWeights[i][j], 0);
        //            NetWeights[i][j][max] = Blocks[i].FCL.Neurons[j].Bias;
        //        }
        //    }
        //    PolicyHWeights = new double[HeadPolicy.NumNeurons][];
        //    for (int i = 0; i < HeadPolicy.NumNeurons; i++)
        //    {
        //        int max = HeadPolicy.NumInputs;
        //        PolicyHWeights[i] = new double[max + 1];
        //        HeadPolicy.Neurons[i].Weights.CopyTo(PolicyHWeights[i], 0);
        //        PolicyHWeights[i][max] = HeadPolicy.Neurons[i].Bias;
        //    }
        //    EvalHWeights = new double[HeadEval.NumInputs + 1];
        //    HeadEval.Weights.CopyTo(EvalHWeights, 0);
        //    EvalHWeights[HeadEval.NumInputs] = HeadEval.Bias;
        //}
        /// <summary>
        /// Получение тупла по позиции
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Новый или существующий тупл, согласно позиции</returns>
        public static Dataset DatasetByPos(Position position)
        {
            if (!Base.ContainsKey(position))
            {
                AddDataset(position);
            }
            return Base[position];
        }
        /// <summary>
        /// Добавляет новый тупл в базу.
        /// </summary>
        /// <param name="position"></param>
        public static void AddDataset(Position position)
        {
            // Код ниже работает правильно. Передача чисел командой "CopyTo" проверена в "песочнице".
            if (Base.ContainsKey(position) && !position.Equals(Program.CurrPos))
            {
                throw new Exception("Такой набор уже существует! Набор не был добавлен.");
            }
            else
            {
                Dataset newSet = new Dataset();
                Blocks[0].FCL.Inputs.CopyTo(newSet.NetInput, 0);
                Policy.CopyTo(newSet.NetPolicy, 0);
                newSet.NetEval = position.Evaluation;
                Base.Add(position, newSet);
            }
        }
        // *** Корректировка весов ***
        // TODO: Finish that block. Should be in NN.
        public static void CorrectNetWeights(Dataset[] miniBatch)
        {
            foreach (Dataset trainSet in miniBatch)
            {
                //HeadEval.CorrectWeights(trainSet.BlockOutputs[NumBlocks - 1], trainSet.GradV);
                //HeadPolicy.CorrectWeights();

                //double diff = 0.0;
                //double VLoss = trainSet.VLoss;
                //double RLoss = trainSet.RLoss;
                //double PLoss = trainSet.PLoss;
                //double Loss = trainSet.Loss;



                for (int i = NumBlocks - 1; i >= 0; i--)
                {
                    //Blocks[i]
                }
            }
            Base.Clear();
        }
    }
}
