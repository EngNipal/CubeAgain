using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class BNLayer
    {
        private int NumInputs { get; set; }
        public double Avg { get; private set; }
        public double StandDeviation { get; private set; }
        private double[] inputs;
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
                StandDeviation = 0.0;
                foreach (double element in inputs)
                {
                    StandDeviation += Math.Pow(element - Avg, 2);
                }
                StandDeviation /= NumInputs - 1;
                StandDeviation = Math.Sqrt(StandDeviation);
            }
        }
        public double[] Outputs { get; private set; }
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
        public double[] BatchNormalization()
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
        /// <param name="StDev"></param>
        /// <returns></returns>
        // Do not delete. The code calling that method is not written yet.
        // Не удалять! Часть кода, вызывающая данный метод ещё не написана.
        public double[] Gradiend(double StDev)
        {
            double[] result = new double[NumInputs];
            if (0 != StDev)
            {
                double FirstConst = (NumInputs - 1) / (NumInputs * StDev);
                double SecondConst = Math.Pow(StDev, 3) * (NumInputs - 1);
                for (int i = 0; i < NumInputs; i++)
                {
                    result[i] = FirstConst;
                    result[i] -= Math.Pow(Inputs[i] - Avg, 2) / SecondConst;
                }
            }
            else
            {
                for (int i = 0; i < NumInputs; i++)
                {
                    result[i] = 1 - (1 / NumInputs);
                }
            }
            return result;
        }
    }
}
