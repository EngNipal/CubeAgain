using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeAgain
{
    class Neuron
    {
        public Neuron(int numInputs, double[] weights, double bias)
        {
            InputQuantity = numInputs;
            _weights = new double[numInputs];
            Weights = weights;
            Bias = bias;
            Output = 0.0;
        }
        public int InputQuantity { get; private set; }
        private bool WeightsChanged { get; set; }
        private double[] _weights;
        public double[] Weights
        {
            get => _weights;
            set
            {
                if (InputQuantity != value.Length)
                {
                    throw new Exception("Неверная длина входных weights в Neuron");
                }
                value.CopyTo(_weights, 0);                  // TODO: Проверить, так ли нужно копирование значений. Или можно обойтись присвоением (2021-04-03)
                WeightsChanged = true;
            }
        }
        public double Bias { get; set; }
        private double Output { get; set; }
        /// <summary>
        /// Метод получения выходного значения нейрона.
        /// </summary>
        /// <returns></returns>
        public double GetOutput()
        {
            if (WeightsChanged)
            {
                throw new Exception("Веса нейрона изменились. Требуется массив входных значений.");
            }
            return Output;
        }
        public double GetOutput(double[] inputs)
        {
            Output = 0.0;
            if (WeightsChanged)
            {
                for (int i = 0; i < InputQuantity; i++)
                {
                    Output += Weights[i] * inputs[i];
                }
                Output += Bias;
            }
            return Output;
        }
    }
}