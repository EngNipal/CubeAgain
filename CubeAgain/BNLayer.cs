using System;
using System.Linq;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class BNLayer
    {
        private double[] inputs { get; set; }
        public double[] Inputs
        {
            get => inputs;
            set
            {
                if (NumInputs != value.Length)
                {
                    throw new Exception("Неверная длина входных параметров в BNLayer.Inputs");
                }
                double[] temp = new double[NumInputs];
                value.CopyTo(temp, 0);
                inputs = temp;
                Avg = inputs.Average();
                SetDeviation();
            }
        }
        public double[] GradToInput { get; private set; }
        private int NumInputs { get; set; }
        private double Avg { get; set; }
        private double StandDeviation { get; set; }
        private double[] Outputs { get; set; }
        public BNLayer(int numInputs)
        {
            NumInputs = numInputs;
            inputs = new double[numInputs];
            Outputs = new double[numInputs];
        }
        /// <summary>
        /// Метод нормализации выходных данных.
        /// </summary>
        /// <returns></returns>
        public double[] Normalization()
        {
            if (0 != StandDeviation)
            {
                for (int i = 0; i < NumInputs; i++)
                {
                    Outputs[i] = (Inputs[i] - Avg) / StandDeviation;
                }
            }
            else
            {
                for (int i = 0; i < NumInputs; i++)
                {
                    Outputs[i] = Inputs[i] - Avg;
                }
            }
            return Outputs;
        }
        /// <summary>
        /// Метод вычисления градиента по слою.
        /// </summary>
        /// <param name="stDev"></param>
        /// <returns></returns>
        // Do not delete. The code calling that method is not written yet.
        // Не удалять! Часть кода, вызывающая данный метод ещё не написана.
        public void SetGradToInput(double[] Xinputs, double[] gradient, double stDev, double avg)
        {
            GradToInput = new double[NumInputs];
            if (0 != stDev)
            {
                double FirstConst = (NumInputs - 1) / (NumInputs * stDev);
                double SecondConst = Math.Pow(stDev, 3) * (NumInputs - 1);
                for (int i = 0; i < NumInputs; i++)
                {
                    GradToInput[i] = FirstConst;
                    GradToInput[i] -= Math.Pow(Inputs[i] - avg, 2) / SecondConst;
                    GradToInput[i] *= gradient[i] * Xinputs[i];
                }
            }
            else
            {
                for (int i = 0; i < NumInputs; i++)
                {
                    GradToInput[i] = 1 - (1 / NumInputs);
                    GradToInput[i] *= gradient[i] * Xinputs[i];
                }
            }
            ImproveGradient();
        }
        private void ImproveGradient()
        {
            for (int i = 0; i < GradToInput.Length; i++)
            {
                if (Math.Abs(GradToInput[i]) < Epsilon)
                {
                    GradToInput[i] = 0;
                }
            }
        }
        private void SetDeviation()
        {
            StandDeviation = 0.0;
            foreach (double inputvalue in inputs)
            {
                StandDeviation += (inputvalue - Avg) * (inputvalue - Avg);
            }
            StandDeviation /= NumInputs - 1;
            StandDeviation = Math.Sqrt(StandDeviation);
        }
    }
}
