using System;
using System.Linq;
using System.Threading.Tasks;

namespace CubeAgain
{
    class Neuron
    {
        public Neuron(int numInputs, double[] weights, double bias)
        {
            //ParentLayer = layer;
            InputQuantity = numInputs;
            _weights = new double[numInputs];
            Weights = weights;
            Bias = bias;
            Output = 0.0;
        }
        // Ссылка на родительский слой.
        //public FCLayer ParentLayer;
        // Количество входных параметров на нейрон. Оно также равно количеству весов нейрона.
        public int InputQuantity { get; private set; }
        // Веса.
        private double[] _weights;
        public double[] Weights
        {
            get => _weights;
            set
            {
                if (InputQuantity != value.Length)
                    throw new Exception("Неверная длина входных weights в Neuron");
                _weights = value;
            }
        }
        // Смещение.
        public double Bias { get; set; }
        // Выходной параметр.
        public double Output { get; private set; }
        internal double GetOutput(double[] inputs)
        {
            for (int i = 0; i < InputQuantity; i++)
            {
                Output += Weights[i] * inputs[i];
            }
            Output += Bias;
            return Output;
        }
    }
}
