using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class FCL_BNL_Block : ILayer
    {
        public FCLayer FCL { get; private set; }
        public BNLayer BNL { get; private set; }
        public double[] Outputs { get; private set; }
        public FCL_BNL_Block(FCLayer fcLayer)
        {
            FCL = fcLayer;
            BNL = new BNLayer(fcLayer.NumNeurons);
            Outputs = new double[fcLayer.NumNeurons];
        }
        /// <summary>
        /// Активационная функция "Сигмоид".
        /// </summary>
        /// <param name="x"></param>
        public void Sigmoid(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                Outputs[i] = 1 / (1 + Math.Exp(0.0 - x[i]));
            }
        }
        /// <summary>
        /// Активационная функция RELU.
        /// </summary>
        /// <param name="x"></param>
        public void RELU(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                Outputs[i] = x[i] > 0 ? x[i] : 0;
            }
        }
        // TODO: Дописать метод.
        public void CorrectWeights(double[] Xinputs, double[] gradient)
        {

        }
    }
}
