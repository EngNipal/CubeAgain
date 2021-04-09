using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CubeAgain.NeuralNetwork;
using static CubeAgain.Training;

namespace CubeAgain
{
    public class Dataset : ICloneable
    {
        public double[] NetInput { get; set; }
        // 1st dimension - block number, 2nd - block output values.
        // 1-я координата - номер блока; 2-я - массив выходных значений блока.
        public double[][] InternalNetOutputs { get; set; }
        public double[] StandDev { get; set; }
        public double[] NetPolicy { get; set; }
        public double[] SearchPolicy { get; set; }
        public double NetScore { get; set; }
        public double Reward { get; set; }
        public int PathLength { get; set; }
        // Game result.
        public double Z { get; private set; }
        // VLoss - квадрат разности между реальным результатом игры и предсказанным сетью.
        public double VLoss { get; private set; }
        // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
        public double RLoss { get; private set; }
        // PLoss - cross-entropy loss.
        public double PLoss { get; private set; }
        // Должен пойти на HeadEval.
        public double GradV => 2 * (NetScore - Z);
        public double[] EvalGrad { get; private set; }
        public double Loss => VLoss + RLoss + PLoss;
        private const double Zero = 0.0;
        public Dataset()
        {
            NetInput = new double[NumInputs];
            InternalNetOutputs = new double[NumBlocks][];
            for (int i = 0; i < NumBlocks; i++)
            {
                InternalNetOutputs[i] = new double[Blocks[i].Outputs.Length];
            }
            StandDev = new double[NumBlocks];
            NetPolicy = new double[Policy.Length];
            SearchPolicy = new double[Policy.Length];
        }
        public void CompleteUnsolved(double[] improvedPolicy, int pathLength)
        {
            improvedPolicy.CopyTo(SearchPolicy, 0);
            PathLength = pathLength;
            Reward = Math.Pow(DiscountCoeff, PathLength) * UnsolvedEvaluation;
            SetZ();
            SetVLoss();
            SetRLoss();
            SetPLoss();
        }
        public void CompleteSolved()
        {
            PathLength++;
            Reward = Math.Pow(DiscountCoeff, PathLength) * SolvedEvaluation;
            SetZ();
            SetVLoss();
        }
        private void SetZ()
        {
            Z = SolvedEvaluation / PathLength;
        }
        private void SetVLoss()
        {
            VLoss = Z - NetScore;
            VLoss *= VLoss;
        }
        private void SetRLoss()
        {
            RLoss = Zero;
            RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
            RLoss += HeadPolicy.RegSum + HeadEval.Regsum;
            RLoss *= RegulCoeff;
        }
        private void SetPLoss()
        {
            PLoss = Zero;
            for (int i = 0; i < SearchPolicy.Length; i++)
            {
                PLoss += SearchPolicy[i] * (Zero - Math.Log(NetPolicy[i]));
            }
        }
        public object Clone()
        {
            Dataset other = new Dataset();
            NetInput.CopyTo(other.NetInput, 0);
            for (int i = 0; i < InternalNetOutputs.Length; i++)
            {
                InternalNetOutputs[i].CopyTo(other.InternalNetOutputs[i], 0);
            }
            StandDev.CopyTo(other.StandDev, 0);
            NetPolicy.CopyTo(other.NetPolicy, 0);
            SearchPolicy.CopyTo(other.SearchPolicy, 0);
            other.NetScore = NetScore;
            other.Reward = Reward;
            other.PathLength = PathLength;
            other.RLoss = RLoss;
            other.SetZ();
            other.SetVLoss();
            other.SetPLoss();
            return other;
        }
    }
}
