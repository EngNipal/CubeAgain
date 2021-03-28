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
            double[] InitWeights = new double[numInputs];
            for(int i = 0; i < numNeurons; i++)
            {
                for (int j = 0; j < numInputs; j++)
                {
                    InitWeights[j] = Rnd.NextDouble();
                }
                Neurons[i] = new Neuron(numInputs, InitWeights, Rnd.NextDouble());
                RegSum = InitWeights.Sum(elem => elem * elem) + (Neurons[i].Bias * Neurons[i].Bias);
            }
        }
        private readonly Random Rnd = new Random();
        // Количество входов слоя
        public int NumInputs { get; set; }
        // Значения входных параметров.
        private readonly double[] inputs;
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
        // Сумма квадратов Weights и Bias для всех нейронов слоя - сумма регуляризации.
        public double RegSum { get; private set; }
        // Количество нейронов слоя
        public int NumNeurons { get; private set; }
        // Массив нейронов.
        internal Neuron[] Neurons;
        public double[] GetOutput()
        {
            for(int i = 0; i < NumNeurons; i++)
            {
                Outputs[i] = Neurons[i].GetOutput(Inputs);
            }
            return Outputs;
        }
    }
}
