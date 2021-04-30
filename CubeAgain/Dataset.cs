using System;
using System.Linq;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    public class Dataset : ICloneable
    {
        public double[] NetInput { get; set; }
        // 1st dimension - block number, 2nd - block output values.
        // 1-я координата - номер блока; 2-я - массив выходных значений блока.
        //public double[][] BlockOutputs { get; set; }
        //public double[][] FCLOutputs { get; set; }
        //public double[][] BNLOutputs { get; set; }
        //public double[] StandDev { get; set; }
        public double[] NetPolicy { get; set; }
        public double[] MctsPolicy { get; set; }
        public double NetEval { get; set; }
        // Game result.
        public double Reward { get; set; }
        public int PathLength { get; set; }
        // 1-я координата - номер блока; 2-я - вектор градиента FC-слоя.
        public double[][] GradientsForFC { get; private set; }
        public double[] GradientForPolicy { get; private set; }
        // VLoss - квадрат разности между реальным результатом игры и предсказанным сетью.
        public double VLoss { get; private set; }
        // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
        public double RLoss { get; private set; }
        // PLoss - cross-entropy loss.
        public double PLoss { get; private set; }
        // Должен пойти на HeadEval.
        public double GradV => 2 * (NetEval - Reward);
        public double Loss => VLoss + RLoss + PLoss;
        private const double Zero = 0.0;
        public Dataset()
        {
            Reward = Zero;
            NetInput = new double[NumInputs];
            NetPolicy = new double[Policy.Length];
            MctsPolicy = new double[Policy.Length];
            GradientsForFC = new double[NumBlocks][];
            for (int i = 0; i < NumBlocks; i++)
            {
                GradientsForFC[i] = new double[Blocks[i].FCL.NumNeurons];
            }
            GradientForPolicy = new double[HeadPolicy.NumNeurons];
        }
        public void CompleteUnsolved(double[] improvedPolicy, int pathLength)
        {
            improvedPolicy.CopyTo(MctsPolicy, 0);
            PathLength = pathLength;
            Reward = Math.Pow(Training.DiscountCoeff, PathLength) * Training.UnsolvedEvaluation;
            SetVLoss();
            SetRLoss();
            SetPLoss();
            ExtractGradients();
        }
        public void CompleteSolved()
        {
            PathLength++;
            Reward = Math.Pow(Training.DiscountCoeff, PathLength) * Training.SolvedEvaluation;
            SetVLoss();
            ExtractGradients();
        }
        private void SetVLoss()
        {
            VLoss = Reward - NetEval;
            VLoss *= VLoss;
        }
        private void SetRLoss()
        {
            RLoss = Zero;
            RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
            RLoss += HeadPolicy.RegSum + HeadEval.Regsum;
            RLoss *= Training.RegulCoeff;
        }
        private void SetPLoss()
        {
            PLoss = Zero;
            for (int i = 0; i < MctsPolicy.Length; i++)
            {
                PLoss += MctsPolicy[i] * (Zero - Math.Log(NetPolicy[i]));
            }
        }
        private void ExtractGradients()
        {
            CalculateGradients(Reward, MctsPolicy);
            HeadPolicy.GradToWeights.CopyTo(GradientForPolicy, 0);
            for (int i = 0; i < NumBlocks; i++)
            {
                Blocks[i].FCL.GradToWeights.CopyTo(GradientsForFC[i], 0);
            }
        }
        public object Clone()
        {
            Dataset other = new Dataset();
            NetInput.CopyTo(other.NetInput, 0);
            NetPolicy.CopyTo(other.NetPolicy, 0);
            MctsPolicy.CopyTo(other.MctsPolicy, 0);
            other.NetEval = NetEval;
            other.Reward = Reward;
            other.PathLength = PathLength;
            other.RLoss = RLoss;
            other.SetVLoss();
            other.SetPLoss();
            return other;
        }
    }
}
