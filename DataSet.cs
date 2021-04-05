using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    class Dataset : ICloneable
    {
        public double[] NetInput { get; set; }
        // 1st dimension - block number, 2nd - block output values.
        // 1-я координата - номер блока; 2-я - массив выходных значений блока.
        public double[][] InternalNetOutputs { get; set; }
        public double[] StandDev { get; set; }
        public double[] SourcePolicy { get; set; }
        public double[] ImprovedPolicy { get; set; }
        public double NetScore { get; set; }
        public double Reward { get; set; }
        public int PathLength { get; set; }
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
            SourcePolicy = new double[Policy.Length];
            ImprovedPolicy = new double[Policy.Length];
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
            SourcePolicy.CopyTo(other.SourcePolicy, 0);
            ImprovedPolicy.CopyTo(other.ImprovedPolicy, 0);
            other.NetScore = NetScore;
            other.Reward = Reward;
            other.PathLength = PathLength;
            return other;
        }
        public double GetLoss()
        {
            // z - Результат игры. Он зависит от длины пути, чем длиннее путь, тем хуже результат.
            double z = Training.SolvedEvaluation / PathLength;
            double VLoss = z - NetScore;
            VLoss *= VLoss;
            // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
            double RLoss = Zero;
            RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
            RLoss *= Training.RegulCoeff;
            // PLoss - cross-entropy loss.
            double PLoss = Zero;
            for (int i = 0; i < ImprovedPolicy.Length; i++)
            {
                PLoss += ImprovedPolicy[i] * (Zero - Math.Log(SourcePolicy[i]));
            }
            return VLoss + PLoss + RLoss;
        }
    }
}
