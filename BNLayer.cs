using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class BNLayer
    {
        private int NumInputs { get; set; }                                           // Количество входных данных.                                   
        public double Avg { get; private set; }                                         // Среднее (average)
        public double StandDev { get; private set; }                                    // Стандартное отклонение (сигма).
        private double[] inputs;                                                       // Массив входных данных.
        public double[] Inputs
        {
            get => inputs;
            set
            {
                if (NumInputs != value.Length)
                    throw new Exception("Неверная длина входных параметров в BNLayer.Inputs");
                double[] temp = new double[NumInputs];
                value.CopyTo(temp, 0);
                inputs = temp;
                Avg = inputs.Average();
                StandDev = 0.0;
                foreach (double element in inputs)
                {
                    StandDev += Math.Pow(element - Avg, 2);
                }
                StandDev /= NumInputs - 1;
                StandDev = Math.Sqrt(StandDev);
            }
        }
        public double[] Outputs { get; private set; }                                   // Массив выходных данных.
        public BNLayer(int numInputs)
        {
            NumInputs = numInputs;
            inputs = new double[numInputs];
            Outputs = new double[numInputs];
        }
        // Метод нормализации выходных данных.
        public double[] BatchNorm()
        {
            if (0 != StandDev)
            {
                for (int i = 0; i < NumInputs; i++)
                {
                    Outputs[i] = (Inputs[i] - Avg) / StandDev;
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
        // Градиент по слою.
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
