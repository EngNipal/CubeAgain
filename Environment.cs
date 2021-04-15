using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace CubeAgain
{
     static class Environment
     {   
        public const int Cells = 24;
        public const int MinScrLength = 1;
        public const int MaxScrLength = 16;
        public static Position Solved { get; } = new Position(SetSolved());
        private static readonly Dictionary<Position, Node> PositionsNodes = new Dictionary<Position, Node>();
        ///  Метод получения узла в дереве по позиции
        /// <param name="position">Позиция для узла</param>
        /// <param name="node">Возвращает узел содержащий позицию</param>
        /// <returns><see langword="true"/> - если возвращён новый узел,
        /// <see langword="false"/> - если возвращён уже имеющийся узел</returns>
        public static Node NodeByPosition(Position position) => NodeByPosition(position, out _);
        public static Node NodeByPosition(Position position, out bool exists)
        {
            Node result;
            exists = PositionsNodes.ContainsKey(position);
            if (exists)
            {
                result = PositionsNodes[position];
            }
            else
            {
                NeuralNetwork.Analyze(position);
                result = new Node(position);
                result.SetMovesPolicy();
                PositionsNodes.Add(position, result);
            }
            return result;
        }
        // TODO: Написать метод поворачивающий кубик в пространстве, (2020-12-27)
        // Если его левый задний кубик не на своём месте.

        public static void Rotate(int[] State)
        {
            
        }
        /// <summary>
        /// Выполнение поворота <see langword="Turn"/> в состоянии <see langword="State"/>.
        /// </summary>
        /// <param name="State">состояние позиции</param>
        /// <param name="Turn">выполняемый поворот</param>
        /// <returns></returns>
        public static void MakeTurn(IList<int> State, Turns Turn)
        {
            int Buffer;
            switch (Turn)
            {
                case Turns.R:
                    {
                        Buffer = State[1];
                        State[1] = State[9];
                        State[9] = State[21];
                        State[21] = State[18];
                        State[18] = Buffer;

                        Buffer = State[3];
                        State[3] = State[11];
                        State[11] = State[23];
                        State[23] = State[16];
                        State[16] = Buffer;

                        Buffer = State[12];
                        State[12] = State[14];
                        State[14] = State[15];
                        State[15] = State[13];
                        State[13] = Buffer;
                    }
                    break;
                case Turns.Rp:
                    {
                        Buffer = State[1];
                        State[1] = State[18];
                        State[18] = State[21];
                        State[21] = State[9];
                        State[9] = Buffer;

                        Buffer = State[3];
                        State[3] = State[16];
                        State[16] = State[23];
                        State[23] = State[11];
                        State[11] = Buffer;

                        Buffer = State[12];
                        State[12] = State[13];
                        State[13] = State[15];
                        State[15] = State[14];
                        State[14] = Buffer;
                    }
                    break;
                case Turns.R2:
                    {
                        Buffer = State[1];
                        State[1] = State[21];
                        State[21] = Buffer;
                        Buffer = State[3];
                        State[3] = State[23];
                        State[23] = Buffer;

                        Buffer = State[9];
                        State[9] = State[18];
                        State[18] = Buffer;
                        Buffer = State[11];
                        State[11] = State[16];
                        State[16] = Buffer;

                        Buffer = State[12];
                        State[12] = State[15];
                        State[15] = Buffer;
                        Buffer = State[13];
                        State[13] = State[14];
                        State[14] = Buffer;
                    }
                    break;
                case Turns.U:
                    {
                        Buffer = State[4];
                        State[4] = State[8];
                        State[8] = State[12];
                        State[12] = State[16];
                        State[16] = Buffer;

                        Buffer = State[5];
                        State[5] = State[9];
                        State[9] = State[13];
                        State[13] = State[17];
                        State[17] = Buffer;

                        Buffer = State[0];
                        State[0] = State[2];
                        State[2] = State[3];
                        State[3] = State[1];
                        State[1] = Buffer;
                    }
                    break;
                case Turns.Up:
                    {
                        Buffer = State[4];
                        State[4] = State[16];
                        State[16] = State[12];
                        State[12] = State[8];
                        State[8] = Buffer;

                        Buffer = State[5];
                        State[5] = State[17];
                        State[17] = State[13];
                        State[13] = State[9];
                        State[9] = Buffer;

                        Buffer = State[0];
                        State[0] = State[1];
                        State[1] = State[3];
                        State[3] = State[2];
                        State[2] = Buffer;
                    }
                    break;
                case Turns.U2:
                    {
                        Buffer = State[4];
                        State[4] = State[12];
                        State[12] = Buffer;
                        Buffer = State[8];
                        State[8] = State[16];
                        State[16] = Buffer;

                        Buffer = State[5];
                        State[5] = State[13];
                        State[13] = Buffer;
                        Buffer = State[9];
                        State[9] = State[17];
                        State[17] = Buffer;

                        Buffer = State[0];
                        State[0] = State[3];
                        State[3] = Buffer;
                        Buffer = State[1];
                        State[1] = State[2];
                        State[2] = Buffer;
                    }
                    break;
                case Turns.F:
                    {
                        Buffer = State[2];
                        State[2] = State[7];
                        State[7] = State[21];
                        State[21] = State[12];
                        State[12] = Buffer;

                        Buffer = State[3];
                        State[3] = State[5];
                        State[5] = State[20];
                        State[20] = State[14];
                        State[14] = Buffer;

                        Buffer = State[8];
                        State[8] = State[10];
                        State[10] = State[11];
                        State[11] = State[9];
                        State[9] = Buffer;
                    }
                    break;
                case Turns.Fp:
                    {
                        Buffer = State[2];
                        State[2] = State[12];
                        State[12] = State[21];
                        State[21] = State[7];
                        State[7] = Buffer;

                        Buffer = State[3];
                        State[3] = State[14];
                        State[14] = State[20];
                        State[20] = State[5];
                        State[5] = Buffer;

                        Buffer = State[8];
                        State[8] = State[9];
                        State[9] = State[11];
                        State[11] = State[10];
                        State[10] = Buffer;
                    }
                    break;
                case Turns.F2:
                    {
                        Buffer = State[2];
                        State[2] = State[21];
                        State[21] = Buffer;
                        Buffer = State[3];
                        State[3] = State[20];
                        State[20] = Buffer;

                        Buffer = State[5];
                        State[5] = State[14];
                        State[14] = Buffer;
                        Buffer = State[7];
                        State[7] = State[12];
                        State[12] = Buffer;

                        Buffer = State[8];
                        State[8] = State[11];
                        State[11] = Buffer;
                        Buffer = State[9];
                        State[9] = State[10];
                        State[10] = Buffer;
                    }
                    break;
            }
        }
        public static int[] SetSolved()
        {
            return new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6 };
        }
        /// <summary>
        /// Метод, выводящий состояние заданной позиции в консоль.
        /// </summary>
        /// <param name="State"></param>
        public static void WriteState(IEnumerable<int> State)
        {
            foreach (int element in State)
            {
                Console.Write(element + " ");
            }
            _ = Console.ReadKey();
        }
        /// <summary>
        /// Метод, скрамблящий куб заданным скрамблом.
        /// </summary>
        /// <param name="State"></param>
        /// <param name="scrambleLength"></param>
        public static void SetScramble(IEnumerable<int> State, int scrambleLength)
        {
            SetScramble(State, scrambleLength, out _);
        }
        public static void SetScramble(IEnumerable<int> State, int scrambleLength, out Turns[] scramble)
        {
            scramble = GetScramble(scrambleLength);
            foreach (Turns turn in scramble)
            {
                MakeTurn(State as IList<int>, turn);
            }
        }
        private static Turns[] GetScramble(int scrLength)
        {
            Random Rnd = new Random();
            int[] scArray = new int[scrLength];
            for (byte i = 0; i < scrLength; i++)
            {
                if (i == 0)
                {
                    scArray[i] = Rnd.Next(0, 8);
                }
                else
                {
                    int prev = scArray[i - 1];
                    if (prev == 0 || prev == 1 || prev == 2)
                    {
                        scArray[i] = Rnd.Next((int)Turns.U, (int)Turns.F2);
                    }
                    else if (prev == 3 || prev == 4 || prev == 5)
                    {
                        if (Rnd.NextDouble() < 0.5)
                        {
                            scArray[i] = Rnd.Next((int)Turns.R, (int)Turns.R2);
                        }
                        else
                        {
                            scArray[i] = Rnd.Next((int)Turns.F, (int)Turns.F2);
                        }
                    }
                    else if (prev == 6 || prev == 7 || prev == 8)
                    {
                        scArray[i] = Rnd.Next((int)Turns.R, (int)Turns.U2);
                    }
                    else
                    {
                        throw new Exception("Получен неверный номер хода! Номер должен быть от 0 до 8.");
                    }
                }
            }
            Turns[] res = (Turns[])scArray.Cast<Turns>();
            return res;
        }
        /// <summary>
        /// Метод вывода скрамбла в консоль.
        /// </summary>
        /// <param name="scramble"></param>
        public static void WriteScramble(Turns[] scramble)
        {
            Console.WriteLine("Ваш скрамбл:");
            foreach (Turns element in scramble)
            {
                Console.Write($"{element} ");
            }
            Console.WriteLine();
        }
    }
}
