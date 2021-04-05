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
    }
}
