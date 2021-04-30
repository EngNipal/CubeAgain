using System;
using System.Linq;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class Neuron
    {
        public double[] GradInputs { get; private set; }
        public double[] GradWeights { get; private set; }
        public int NumInputs { get; private set; }
        public double Output { get; private set; }
        public double Regsum { get; private set; }
        public double[] Inputs
        {
            get
            {
                double[] result = new double[NumInputs];
                _inputs.CopyTo(result, 0);
                return result;
            }
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных inputs в Neuron");
                }
                value.CopyTo(_inputs, 0);
                Activate();
            }
        }

        public double[] Weights
        {
            get
            {
                double[] result = new double[NumInputs];
                _weights.CopyTo(result, 0);
                return result;
            }
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных weights в Neuron");
                }
                value.CopyTo(_weights, 0);
                SetRegsum();
                _activated = false;
            }
        }

        private bool _activated { get; set; }
        private double _bias { get; set; }
        private double[] _inputs { get; set; }
        private double[] _weights { get; set; }

        public Neuron(int numInputs)
        {
            Random rnd = new Random();
            NumInputs = numInputs;
            _inputs = new double[numInputs];
            _weights = new double[numInputs];
            for (int i = 0; i < numInputs; i++)
            {
                _weights[i] = rnd.NextDouble();
            }
            _bias = rnd.NextDouble();
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

        public void Activate()
        {
            if (_activated)
            {
                return;
            }
            Output = 0.0;
            for (int i = 0; i < NumInputs; i++)
            {
                Output += Weights[i] * _inputs[i];
            }
            Output += _bias;
            _activated = true;
        }

        public void SetGradients(double gradient)
        {
            SetGradWeights(gradient);
            SetGradInputs(gradient);
        }

        private void SetGradInputs(double gradient)
        {
            GradInputs = new double[NumInputs];
            for (int i = 0; i < NumInputs; i++)
            {
                GradInputs[i] = _weights[i] * gradient;
            }
            ImproveGradient(GradInputs);
        }

        private void SetGradWeights(double grad)
        {
            GradWeights = new double[_weights.Length + 1];
            for (int i = 0; i < _weights.Length; i++)
            {
                GradWeights[i] = (_inputs[i] * grad) + (2 * RegulCoeff * _weights[i]);
            }
            GradWeights[_weights.Length] = grad + (2 * RegulCoeff * _bias);
            ImproveGradient(GradWeights);
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
            Regsum = Weights.Sum(elem => elem * elem) + (_bias * _bias);
        }
    }
}