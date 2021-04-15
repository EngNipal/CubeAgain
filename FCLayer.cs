using System;
using System.Linq;
using System.Threading.Tasks;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class FCLayer
    {
        private double[] inputs { get; set; }
        public double[] Inputs
        {
            get { return inputs; }
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных параметров в FCLayer.Inputs");
                }
                value.CopyTo(inputs, 0);
                SetNeurInputs();
                SetOutputs();
            }
        }
        public double RegSum { get; private set; }
        public int NumNeurons { get; private set; }
        public double[] GradToInputs { get; private set; }
        public double[][] GradToWeights { get; private set; }
        public double[] Outputs { get; private set; }
        private int NumInputs { get; set; }
        private Neuron[] Neurons { get; set; }
        private readonly Random Rnd = new Random();
        public FCLayer(int numInputs, int numNeurons)
        {
            NumInputs = numInputs;
            NumNeurons = numNeurons;
            inputs = new double[numInputs];
            Outputs = new double[numNeurons];
            Neurons = new Neuron[numNeurons];
            SetWeights();
            SetRegsum();
        }
        // Метод корректировки весов нейронов слоя.
        //public void CorrectWeights(double[] Xinputs, double[] gradient)
        //{
        //    GradToInput = new double[NumInputs];
        //    if (gradient.Length != NumNeurons)
        //    {
        //        throw new Exception("Неверная длина градиента для FCLayer.");
        //    }
        //    for (int i = 0; i < NumNeurons; i++)
        //    {
        //        Neurons[i].CorrectWeights(Xinputs, gradient[i]);
        //        for (int j = 0; j < NumInputs; j++)
        //        {
        //            GradToInput[j] += Neurons[i].GradToInput[j];        // Это абсолютно правильно! (2021-04-09).
        //        }
        //    }
        //    ImproveGradient();
        //}
        public void SetGradToInput(double[] gradient)
        {
            GradToInputs = new double[NumInputs];
            if (gradient.Length != NumNeurons)
            {
                throw new Exception("Неверная длина градиента для FCLayer.");
            }
            for (int i = 0; i < NumNeurons; i++)
            {
                for (int j = 0; j < NumInputs; j++)
                {
                    GradToInputs[j] += Neurons[i].GradToInputs[j];        // Это абсолютно правильно! (2021-04-09).
                }
            }
            ImproveGradient(GradToInputs);
        }
        // TODO: Проверить, что градиент собран правильно.
        public void SetGradToWeights(double[] gradient)
        {
            if (gradient.Length != NumNeurons)
            {
                throw new Exception("Неверная длина градиента для FCLayer.");
            }
            GradToWeights = new double[NumNeurons][];
            for (int i = 0; i < NumNeurons; i++)
            {
                GradToWeights[i] = new double[NumInputs];
                for (int j = 0; j < NumInputs; j++)
                {
                    GradToWeights[i][j] = gradient[i] * inputs[j] + 2 * RegulCoeff * Neurons[i].Weights[j];
                }
            }
        }
        private void ImproveGradient(double[] gradient)
        {
            for (int i = 0; i < gradient.Length; i++)
            {
                if (Math.Abs(gradient[i]) < Epsilon)
                {
                    gradient[i] = 0;
                }
            }
        }
        private void SetWeights()
        {
            double[] InitWeights = new double[NumInputs];
            for (int i = 0; i < NumNeurons; i++)
            {
                for (int j = 0; j < NumInputs; j++)
                {
                    InitWeights[j] = Rnd.NextDouble();
                }
                Neurons[i] = new Neuron(NumInputs, InitWeights, Rnd.NextDouble());
            }
        }
        private void SetRegsum()
        {
            foreach (Neuron neuron in Neurons)
            {
                RegSum += neuron.Regsum;
            }
        }
        private void SetNeurInputs()
        {
            foreach (Neuron neuron in Neurons)
            {
                neuron.Inputs = inputs;
            }
        }
        private void SetOutputs()
        {
            for (int i = 0; i < NumNeurons; i++)
            {
                Outputs[i] = Neurons[i].Output;
            }
        }
    }
}