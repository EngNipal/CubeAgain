using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeAgain
{
    internal interface ILayer
    {
        void CorrectWeights(double[] Xinputs, double[] gradient);
    }
}
