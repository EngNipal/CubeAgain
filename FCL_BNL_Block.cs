using System;

namespace CubeAgain
{
    public class FCL_BNL_Block
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
        // TODO: Дописать метод (2021-04-10).
        //public void CorrectWeightsIfRelu(double[] Xinputs, double[] fclOut, double[] gradient, double stdev, double avg)
        //{
        //    if (gradient.Length != Outputs.Length)
        //    {
        //        throw new Exception("Неверная длина градиента для блока.");
        //    }
        //    BNL.SetGradToInput(fclOut, gradient, stdev, avg);
        //    FCL.CorrectWeights(Xinputs, BNL.GradToInput);
        //}
        //public void CorrectWeightsIfSigmoid(double[] Xinputs, double[] fclOut, double[] bnlOut, double[] gradient, double stdev, double avg)
        //{
        //    if (gradient.Length != Outputs.Length)
        //    {
        //        throw new Exception("Неверная длина градиента для блока.");
        //    }
        //    BNL.SetGradToInput(fclOut, SigmoidGradient(bnlOut, gradient), stdev, avg);
        //    FCL.CorrectWeights(Xinputs, BNL.GradToInput);
        //}
        //// TODO: Доработать метод.
        //private double[] SigmoidGradient(double[] bnlOut, double[] outergradient)
        //{
        //    double[] result = new double[Outputs.Length];

        //}
    }
}
