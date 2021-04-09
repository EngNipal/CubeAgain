using System;
using System.Linq;
using System.Threading.Tasks;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class Neuron
    {
        public Neuron(int numInputs, double[] weights, double bias)
        {
            InputQuantity = numInputs;
            this.weights = new double[numInputs];
            Weights = weights;
            Bias = bias;
            Output = 0.0;
        }
        public int InputQuantity { get; private set; }
        private bool WeightsChanged { get; set; }
        private double[] weights { get; set; }
        public double[] Weights
        {
            get => weights;
            set
            {
                if (InputQuantity != value.Length)
                {
                    throw new Exception("Неверная длина входных weights в Neuron");
                }
                value.CopyTo(weights, 0);                  // TODO: Проверить, так ли нужно копирование значений. Или можно обойтись присвоением (2021-04-03)
                SetRegsum();
                WeightsChanged = true;
            }
        }
        public double Bias { get; set; }
        public double Regsum { get; private set; }
        private double Output { get; set; }
        public double[] GradToInput { get; private set; }
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
            WeightsChanged = false;
            return Output;
        }
        // Для нейрона получаем значение ошибки на выходе и массив входных значений (какие были значения когда-то).
        public void CorrectWeights(double[] Xinput, double grad)
        {
            GradToInput = new double[InputQuantity];
            if (Xinput.Length != weights.Length)
            {
                throw new Exception("Неверная длина входного градиента в нейроне");
            }
            for (int i = 0; i < InputQuantity; i++)
            {
                GradToInput[i] = weights[i] * grad;
                weights[i] -= ((Xinput[i] * grad) + (2 * RegulCoeff * weights[i])) * LearningRate;
            }
            Bias -= (grad + (2 * RegulCoeff * Bias)) * LearningRate;
            WeightsChanged = true;
        }
        private void SetRegsum()
        {
            Regsum = Weights.Sum(elem => elem * elem) + (Bias * Bias);
        }
    }
}