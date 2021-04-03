﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    internal class TrainTuple
    {
        public double[] NetInput { get; set; }                                      // Вход сети (нормализованные данные).
        public double[][] InternalNetOutputs { get; set; }                          // Выходы слоёв внутри сети. (1-я координата - номер блока; 2-я - массив выходных значений блока.
        public double[] StDev { get; set; }                                         // Стандартное отклонение в BN-слоях. (координата - номер блока)
        public double[] SourcePolicy { get; set; }                                  // Оценка позиции до MCTS.
        public double[] ImprovedPolicy { get; set; }                                // Оценка позиции после MCTS.
        public double Score { get; set; }                                           // Оценка позиции сетью.
        public double Reward { get; set; }                                          // Главная плюшка, получаемая от среды.
        public int PathLength { get; set; }                                         // Длина пути в конкретный момент.
        public TrainTuple()
        {
            NetInput = new double[NumInputs];
            InternalNetOutputs = new double[NumBlocks][];
            for (int i = 0; i < NumBlocks; i++)
            {
                InternalNetOutputs[i] = new double[Blocks[i].Outputs.Length];
            }
            StDev = new double[NumBlocks];
            SourcePolicy = new double[Policy.Length];
            ImprovedPolicy = new double[Policy.Length];
        }
    }
}
