namespace CubeAgain
{
    internal interface ILayer
    {
        void CorrectWeights(double[] Xinputs, double[] gradient);
    }
}
