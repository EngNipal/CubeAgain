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
        public double[] NetPolicy { get; set; }
        public double[] SearchPolicy { get; set; }
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
            NetPolicy = new double[Policy.Length];
            SearchPolicy = new double[Policy.Length];
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
            return other;
        }
        // VLoss - квадрат разности между реальным результатом игры и предсказанным сетью.
        public double GetVLoss()
        {
            // z - Результат игры. Он зависит от длины пути, чем длиннее путь, тем хуже результат.
            double z = Training.SolvedEvaluation / PathLength;
            double VLoss = z - NetScore;
            VLoss *= VLoss;
            return VLoss;
        }
        // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
        public double GetRLoss()
        {
            double RLoss = Zero;
            RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
            RLoss *= Training.RegulCoeff;
            return RLoss;
        }
        // PLoss - cross-entropy loss.
        public double GetPLoss()
        {
            double PLoss = Zero;
            for (int i = 0; i < SearchPolicy.Length; i++)
            {
                PLoss += SearchPolicy[i] * (Zero - Math.Log(NetPolicy[i]));
            }
            return PLoss;
        }
    }
}
