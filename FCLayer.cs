using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeAgain
{
    public class FCLayer
    {
        public FCLayer(int numInputs, int numNeurons)
        {
            NumInputs = numInputs;
            NumNeurons = numNeurons;
            inputs = new double[numInputs];
            Outputs = new double[numNeurons];
            Neurons = new Neuron[numNeurons];
            SetWeights(numInputs);
            SetRegsum();
        }
        public int NumInputs { get; set; }
        private double[] inputs;
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
            }
        }
        public double[] Outputs { get; private set; }
        public double RegSum { get; private set; }
        public int NumNeurons { get; private set; }
        public Neuron[] Neurons;
        public double[] GetOutputs()
        {
            for(int i = 0; i < NumNeurons; i++)
            {
                Outputs[i] = Neurons[i].GetOutput(Inputs);
            }
            return Outputs;
        }
        // Метод корректировки весов нейронов слоя.
        // !!!!! Нужно очень грамотно собрать вектор градиента !!!!!
        // TODO: Определиться с корректировкой весов. Доработать метод.
        internal void CorrectWeights(double[] gradfrom)
        {
            int i = 0;
            foreach (Neuron neuron in Neurons)
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
                    if (neuron.GetOutput() >= 0)
                    {
                        neuron.Weights[j] -= Inputs[j] * neuron.GetOutput() * gradfrom[i] * Training.LearningRate;
                    }
                }
                i++;
            }
        }
        private void SetWeights(int numInputs)
        {
            double[] InitWeights = new double[numInputs];
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
            for (int i = 0; i < NumNeurons; i++)
            {
                RegSum = Neurons[i].Weights.Sum(elem => elem * elem) + (Neurons[i].Bias * Neurons[i].Bias);
            }
        }
        private readonly Random Rnd = new Random();
    }
}