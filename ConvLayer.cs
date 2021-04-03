using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    class ConvLayer
    {
        public int Padding { get; }
        public int Stride { get; }
        public int[,] Kernel { get; }
        public Neuron[,,] Neurons { get; }
        public int Xsize { get; }
        public int Ysize { get; }
        public int Channels { get; }                      // Глубина слоя, или иначе - количество каналов (различаемых признаков).
        // TODO: Рассмотреть возможность задавать параметры конструктора массивом значений (2021-04-03).
        public ConvLayer (int InputXsize, int InputYsize, int InputDepth) : this (InputXsize, InputYsize, InputDepth, 1, 1, 0, 1)
        { }
        public ConvLayer (int InputXsize, int InputYsize, int InputDepth, int KernelSize, int NumOfChannels, int padding, int stride)
        {
            Xsize = Math.DivRem(InputXsize + 2 * padding - KernelSize, stride, out int checkX) + 1;
            Ysize = Math.DivRem(InputYsize + 2 * padding - KernelSize, stride, out int checkY) + 1;
            if (checkX != 0 || checkY != 0 )
            {
                throw new Exception("Неверное сочетание входных размеров, kernel, padding и stride");
            }
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
