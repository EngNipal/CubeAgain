using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeAgain
{
    static class NeuralNetwork
    {
        public static double Evaluation { get; private set; }
        public static double[] Policy { get; private set; }
        public static int NumInputs { get; private set; }
        public static int NumBlocks { get; private set; }
        public static FCL_BNL_Block[] Blocks { get; set; }
        public static FCLayer HeadPolicy { get; private set; }
        public static Neuron HeadEval { get; private set; }
        public delegate void ActivationFunction(double[] data);
        private static ActivationFunction[] Activate;
        public delegate void MethodContainer(Position position);
        public static event MethodContainer Analyzed;
        /// <summary>
        /// Создание структуры нейросети.
        /// </summary>
        public static void SetNetworkStructure()                    // TODO: Обдумать возможность задавать параметры объектом или массивом значений.
        {
            const int nBlocks = 3;
            int[] listNeur = new int[nBlocks] { 64, 64, 64 };
            Activate = new ActivationFunction[nBlocks];
            PresetStruct(Environment.Cells, nBlocks, listNeur);
        }
        private static void PresetStruct(int numInputs, int numBlocks, int[] numNeurons)
        {
            Random Rnd = new Random();
            NumInputs = numInputs;
            NumBlocks = numBlocks;
            int TurnsCount = Enum.GetValues(typeof(Turns)).Length;
            Blocks = new FCL_BNL_Block[numBlocks];
            Blocks[0] = new FCL_BNL_Block(new FCLayer(numInputs, numNeurons[0]));
            for (int i = 1; i < numBlocks; i++)
            {
                Blocks[i] = new FCL_BNL_Block(new FCLayer(numNeurons[i - 1], numNeurons[i]));
            }
            for (int i = 0; i < numBlocks; i++)
            {
                Activate[i] += Blocks[i].RELU;
            }
            double[] InitWeights = new double[numNeurons[numBlocks - 1]];
            HeadPolicy = new FCLayer(InitWeights.Length, TurnsCount);
            for (int i = 0; i < InitWeights.Length; i++)
            {
                InitWeights[i] = Rnd.NextDouble();
            }
            HeadEval = new Neuron(InitWeights.Length);
            Policy = new double[TurnsCount];
        }
        /// <summary>
        /// Метод анализа заданной позиции нейросетью.
        /// </summary>
        /// <param name="position"></param>
        public static void Analyze(Position position)
        {
            Blocks[0].FCL.Inputs = Preprocessing(position.State);
            for (int i = 0; i < NumBlocks; i++)
            {
                Blocks[i].BNL.Inputs = Blocks[i].FCL.Outputs;
                Activate[i](Blocks[i].BNL.Normalization());
                if (i < NumBlocks - 1)
                {
                    Blocks[i + 1].FCL.Inputs = Blocks[i].Outputs;
                }
            }
            HeadPolicy.Inputs = Blocks[NumBlocks - 1].Outputs;
            HeadPolicy.Outputs.CopyTo(Policy, 0);
            Policy = SoftMax(Policy);
            HeadEval.Inputs = Blocks[NumBlocks - 1].Outputs;
            Evaluation = HeadEval.Output;
            position.Evaluation = Evaluation;
            Analyzed?.Invoke(position);
        }
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
        private static double[] Preprocessing(int[] SomeParams)
        {
            double[] result = new double[SomeParams.Length];
            double Avg = SomeParams.Average();
            double StandDev = 0.0;
            foreach (int element in SomeParams)
            {
                StandDev += (element - Avg) * (element - Avg);
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
        public static void CalculateGradients(double ProperEvaluation, double[] ProperPolicy )
        {
            Analyze(Program.CurrPos);                                       // - Обязательно для правильного вычисления градиентов.
            double EvalGrad = 2 * (HeadEval.Output - ProperEvaluation);     // - Верно.
            HeadEval.SetGradients(EvalGrad);

            // TODO: Подсчитать градиент от Softmax-a и передать его в HeadPolicy.

            HeadPolicy.SetGradToInput(ProperPolicy);

            // TODO: Градиент к последнему блоку равен сумме градиентов, идущих от "голов".
        }
    }
}
