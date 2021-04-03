using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeAgain
{
    public class Position : IEquatable<Position>
    {
        public int[] State { get; private set; }
        public double Evaluation { get; set; }
        public int HashCode { get; }
        public Position(IEnumerable<int> state)
        {
            State = state?.ToArray() ?? Array.Empty<int>();
            HashCode = -1073676287;                             // Десятизначное простое число. Здесь - простое число Кэрола.
            foreach (int item in State)
            {
                HashCode = ((HashCode << 13) | (int)((uint)HashCode >> 17)) ^ item;
            }
        }
        // Следующие конструкторы созданы для возможности реализации таких конструкций:
        // Position pos1 = new Position(0, 0);
        // Position pos2 = new Position(0.1, 0, 0);
        //
        // List<int> list = new List<int>() { 1, 2 };
        //
        // Position pos3 = new Position(list);
        // Position pos4 = new Position(1.0, list);
        public Position(params int[] state) : this((IEnumerable<int>)state)
        { }
        public Position(double value, IEnumerable<int> state) : this(state) => Evaluation = value;
        public Position(double value, params int[] state) : this(value, (IEnumerable<int>)state)
        { }
        public override int GetHashCode() => HashCode;
        public override bool Equals(object obj) => Equals(obj as Position);
        public bool Equals(Position other) => other != null && HashCode == other.HashCode && State.SequenceEqual(other.State);
        /// <summary>
        /// Получение новой позиции
        /// </summary>
        /// <param name="turn"></param>
        /// <returns>Новая позиция, полученная из данной, ходом <see paramname="turn"/></returns>
        public Position PosAfterTurn(Turns turn)
        {
            int[] tempState = new int[State.Length];
            State.CopyTo(tempState, 0);
            Environment.MakeTurn(tempState, turn);
            return new Position(Evaluation, tempState);
        }
    }
}
