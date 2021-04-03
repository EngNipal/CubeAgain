using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeAgain
{
    static class NeuralNetwork
    {
        public static double Evaluation { get; private set; }
        public static double[] Policy { get; private set; }
        internal static int NumBlocks { get; private set; }                                 // Количество блоков.
        internal static FCL_BNL_Block[] Blocks { get; set; }                                // Промежуточные блоки сети.
        public static FCLayer HeadPolicy { get; private set; }                              // Отдельный слой для Policy.
        public static Neuron HeadEval { get; private set; }                                 // Отдельный нейрон для Evaluation.
        public delegate void ActivationFunction(double[] data);
        private static ActivationFunction[] Activation;
        public delegate void MethodContainer(Position position);            // Делегат метода создания тренировочного набора (тупла).
        public static event MethodContainer Analyzed;                       // Событие проведения оценки новой позиции.
        // Создание структуры нейросети.
        public static void SetNetworkStructure()
        {
            const int nBlocks = 3;
            int[] listNeur = new int[nBlocks] { 64, 64, 64 };
            PresetStruct(24, nBlocks, listNeur);
            //for (int i = 0; i < nBlocks; i++)
            //{
            //    Activation[i] = Blocks[i].RELU;
            //}
        }
        private static void PresetStruct(int numInputs, int numBlocks, int[] numNeurons)
        {
            Random Rnd = new Random();
            NumBlocks = numBlocks;
            Blocks = new FCL_BNL_Block[NumBlocks];
            Blocks[0] = new FCL_BNL_Block(new FCLayer(numInputs, numNeurons[0]));
            Activation[0] += Blocks[0].RELU;
            for (int i = 1; i < NumBlocks; i++)
            {
                Blocks[i] = new FCL_BNL_Block(new FCLayer(numNeurons[i - 1], numNeurons[i]));
                Activation[i] += Blocks[i].RELU;
            }
            HeadPolicy = new FCLayer(numNeurons[numBlocks - 1], 9);
            double[] InitWeights = new double[numInputs];
            for (int j = 0; j < numInputs; j++)
            {
                InitWeights[j] = Rnd.NextDouble();
            }
            HeadEval = new Neuron(numNeurons[numBlocks - 1], InitWeights, Rnd.NextDouble());
            Policy = new double[numNeurons[numNeurons.Length - 1] - 1];
        }
        public static void Analyze(Position position)
        {
            Blocks[0].FCL.Inputs = Preprocessing(position.State);               // Подаём на вход нулевого блока нормализованные данные.
            for (int i = 0; i < NumBlocks; i++)                                 // Для всех блоков...
            {
                Blocks[i].BNL.Inputs = Blocks[i].FCL.GetOutput();               // Выходы FCL передаем на вход BNL.
                Activation[i](Blocks[i].BNL.BatchNorm());                       // Проводим Batch Normalization и вычисляем функцию активации.
                if (i < NumBlocks - 1)                                          // И если это не последний блок...
                {
                    Blocks[i + 1].FCL.Inputs = Blocks[i].Outputs;               // Передаём выход блока на вход следующему.
                }
            }
            HeadPolicy.Inputs = Blocks[NumBlocks - 1].Outputs;                  // Передаём выходы последнего блока головам Evaluation и Policy.
            Evaluation = HeadEval.GetOutput(Blocks[NumBlocks - 1].Outputs);     // Выход HeadEval это Evaluation.
            HeadPolicy.GetOutput().CopyTo(Policy, 0);                           // Выход HeadPolicy это Policy.
            Policy = SoftMax(Policy);                                           // Накладываем Soft-max на Policy.
            position.Evaluation = Evaluation;
            Analyzed?.Invoke(position);
        }
        // Функция Soft-max.
        private static double[] SoftMax(double[] SomeParams)
        {
            double sum = SomeParams.Sum(element => Math.Exp(element));
            double[] result = new double[SomeParams.Length];
            for (int i = 0; i < SomeParams.Length; i++)
            {
                result[i] = SomeParams[i] / sum;
            }
            return result;
        }
        // Метод нормализации входных данных.
        private static double[] Preprocessing(int[] SomeParams)
        {
            double[] result = new double[SomeParams.Length];
            double Avg = SomeParams.Average();
            double StandDev = 0.0;
            foreach (int element in SomeParams)
            {
                StandDev += Math.Pow(element - Avg, 2);
            }
            StandDev /= SomeParams.Length - 1;
            StandDev = Math.Sqrt(StandDev);
            if (0 != StandDev)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (SomeParams[i] - Avg) / StandDev;
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = SomeParams[i] - Avg;
                }
            }
            return result;
        }
    }
}
