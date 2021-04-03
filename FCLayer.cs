using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeAgain
{
    class FCLayer
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
        private readonly Random Rnd = new Random();
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
    }
}