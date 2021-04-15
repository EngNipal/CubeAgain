using System;
using System.Linq;
using System.Threading.Tasks;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class Neuron
    {
        public int NumInputs { get; private set; }
        private double[] weights { get; set; }
        public double[] Weights
        {
            get => weights;
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных weights в Neuron");
                }
                value.CopyTo(weights, 0);
                SetRegsum();
                SetOutput();
            }
        }
        public double Bias { get; private set; }
        public double Regsum { get; private set; }
        private double[] inputs { get; set; }
        public double[] Inputs
        {
            get => inputs;
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных inputs в Neuron");
                }
                value.CopyTo(inputs, 0);
                SetOutput();
            }
        }
        public double Output { get; private set; }
        public double[] GradToInputs { get; private set; }
        public double[] GradToWeights { get; private set; }
        public Neuron(int numInputs, double[] weights, double bias)
        {
            NumInputs = numInputs;
            inputs = new double[numInputs];
            this.weights = new double[numInputs];
            Weights = weights;
            Bias = bias;
        }
        // Для нейрона получаем значение ошибки на выходе и массив входных значений (какие были значения когда-то).
        //public void CorrectWeights(double[] Xinput, double grad)
        //{
        //    GradToInput = new double[InputQuantity];
        //    if (Xinput.Length != weights.Length)
        //    {
        //        throw new Exception("Неверная длина входного градиента в нейроне");
        //    }
        //    for (int i = 0; i < InputQuantity; i++)
        //    {
        //        GradToInput[i] = weights[i] * grad;
        //        weights[i] -= ((Xinput[i] * grad) + (2 * RegulCoeff * weights[i])) * LearningRate;
        //    }
        //    Bias -= (grad + (2 * RegulCoeff * Bias)) * LearningRate;
        //    ImproveGradient();
        //    WeightsChanged = true;
        //}
        public void SetGradients(double gradient)
        {
            SetGradToWeights(gradient);
            SetGradToInputs(gradient);
        }
        private void SetGradToInputs(double grad)
        {
            GradToInputs = new double[NumInputs];
            for (int i = 0; i < NumInputs; i++)
            {
                GradToInputs[i] = weights[i] * grad;
            }
            ImproveGradient(GradToInputs);
        }
        private void SetGradToWeights(double grad)
        {
            GradToWeights = new double[weights.Length + 1];
            for (int i = 0; i < weights.Length; i++)
            {
                GradToWeights[i] = (inputs[i] * grad) + (2 * RegulCoeff * weights[i]);
            }
            GradToWeights[weights.Length] = grad + (2 * RegulCoeff * Bias);
            ImproveGradient(GradToWeights);
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
        private void SetRegsum()
        {
            Regsum = Weights.Sum(elem => elem * elem) + (Bias * Bias);
        }
        private void SetOutput()
        {
            Output = 0.0;
            for (int i = 0; i < NumInputs; i++)
            {
                Output += Weights[i] * inputs[i];
            }
            Output += Bias;
        }
    }
}