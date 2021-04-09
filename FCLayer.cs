using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeAgain
{
    public class FCLayer : ILayer
    {
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
        public int NumInputs { get; private set; }
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
            }
        }
        public double RegSum { get; private set; }
        public int NumNeurons { get; private set; }
        public Neuron[] Neurons { get; private set; }
        public double[] GradToInput { get; private set; }
        private double[] Outputs { get; set; }
        public double[] GetOutputs()                            // TODO: проверить уязвимость поля Outputs при передаче из метода наружу (2021-04-09).
        {
            for(int i = 0; i < NumNeurons; i++)
            {
                Outputs[i] = Neurons[i].GetOutput(Inputs);
            }
            return Outputs;
        }
        // Метод корректировки весов нейронов слоя.
        public void CorrectWeights(double[] Xinputs, double[] gradient)
        {
            GradToInput = new double[NumInputs];
            if (gradient.Length != NumNeurons)
            {
                throw new Exception("Неверная длина градиента для FCLayer.");
            }
            for (int i = 0; i < NumNeurons; i++)
            {
                Neurons[i].CorrectWeights(Xinputs, gradient[i]);
                for (int j = 0; j < NumInputs; j++)
                {
                    GradToInput[j] += Neurons[i].GradToInput[j];        // Это абсолютно правильно! (2021-04-09).
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
        private readonly Random Rnd = new Random();
    }
}