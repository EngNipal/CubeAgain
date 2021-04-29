using System;
using System.Linq;
using System.Threading.Tasks;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class Neuron
    {
        public double Bias { get; private set; }
        public double[] GradToInputs { get; private set; }
        public double[] GradToWeights { get; private set; }
        public int NumInputs { get; private set; }
        public double Output { get; private set; }
        public double Regsum { get; private set; }
        public double[] Inputs
        {
            get => _inputs;
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных inputs в Neuron");
                }
                value.CopyTo(_inputs, 0);
                SetOutput();
            }
        }

        public double[] Weights
        {
            get => _weights;
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных weights в Neuron");
                }
                value.CopyTo(_weights, 0);
                SetRegsum();
                SetOutput();
            }
        }

        private double[] _inputs { get; set; }
        private double[] _weights { get; set; }

        public Neuron(int numInputs, double[] weights, double bias)
        {
            NumInputs = numInputs;
            _inputs = new double[numInputs];
            _weights = new double[numInputs];
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
                GradToInputs[i] = _weights[i] * grad;
            }
            ImproveGradient(GradToInputs);
        }
        private void SetGradToWeights(double grad)
        {
            GradToWeights = new double[_weights.Length + 1];
            for (int i = 0; i < _weights.Length; i++)
            {
                GradToWeights[i] = (_inputs[i] * grad) + (2 * RegulCoeff * _weights[i]);
            }
            GradToWeights[_weights.Length] = grad + (2 * RegulCoeff * Bias);
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
                Output += Weights[i] * _inputs[i];
            }
            Output += Bias;
        }
    }
}