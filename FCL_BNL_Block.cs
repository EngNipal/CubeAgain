using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class FCL_BNL_Block
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
        // Функции активации.
        internal void Sigmoid(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                Outputs[i] = 1 / (1 + Math.Exp(0.0 - x[i]));
            }
            //return Outputs;
        }
        internal void RELU(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                Outputs[i] = x[i] > 0 ? x[i] : 0;
            }
            //return Outputs;
        }
    }
}
