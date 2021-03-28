using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class ConvLayer
    {
        public int Padding { get; }                    // Дополнение входных данных. Default = 0.
        public int Stride { get; }                     // Смещение при проходе по входным данным. Default = 1.
        public int[,] Kernel { get; }                  // Размер входного "окна". (3х3, 2х2 и т.п.) - окно всегда квадратное. Default 1x1.
        //public double [,,,] Weights { get; set; }         // TODO: Веса слоя убрать в нейроны.
        public Neuron[,,] Neurons { get; }             // Массив нейронов.
        public int Xsize { get; }                      // Размеры.
        public int Ysize { get; }
        public int Channels { get; }                      // Глубина слоя, или иначе - количество каналов (различаемых признаков).
        public ConvLayer (int InputXsize, int InputYsize, int InputDepth) : this (InputXsize, InputYsize, InputDepth, 1, 1, 0, 1)
        { }
        public ConvLayer (int InputXsize, int InputYsize, int InputDepth, int KernelSize, int NumOfChannels, int padding, int stride)
        {
            Xsize = Math.DivRem(InputXsize + 2 * padding - KernelSize, stride, out int checkX) + 1;
            Ysize = Math.DivRem(InputYsize + 2 * padding - KernelSize, stride, out int checkY) + 1;
            if (checkX != 0 || checkY != 0 )
                throw new Exception("Неверное сочетание входных размеров, kernel, padding и stride");
            Kernel = new int[KernelSize, KernelSize];
            Channels = NumOfChannels;
            Neurons = new Neuron[Xsize, Ysize, NumOfChannels];
            Padding = padding;
            Stride = stride;
            Random Rnd = new Random();
            double[] InitWeights = new double[KernelSize * KernelSize * InputDepth];
            for (int channel = 0; channel < NumOfChannels; channel++)
            {
                for (int z = 0; z < InitWeights.Length; z++)
                {
                    InitWeights[z] = Rnd.NextDouble();
                }
                for (int i = 0; i < Xsize; i++)
                {
                    for (int j = 0; j < Ysize; j++)
                    {
                        Neurons[i, j, channel] = new Neuron(InitWeights.Length, InitWeights, 0);
                    }
                }
            }
        }
    }
}
