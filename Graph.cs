using System.Collections.Generic;

namespace CubeAgain
{
    public class Graph
    {
        public Node Solved { get; } // TODO: Consider to delete (2021-04-04).
        public Graph()
        {
            Solved = NodeFromPosition(Environment.Solved);
            Solved.WasVisited = false;
        }
        private readonly Dictionary<Position, Node> PositionsNodes = new Dictionary<Position, Node>();

        ///  Метод получения узла в дереве по позиции
        /// <param name="position">Позиция для узла</param>
        /// <param name="node">Возвращает узел содержащий позицию</param>
        /// <returns><see langword="true"/> - если возвращён новый узел,
        /// <see langword="false"/> - если возвращён уже имеющийся узел</returns>
        public Node NodeFromPosition(Position position) => NodeFromPosition(position, out _);
        public Node NodeFromPosition(Position position, out bool exists)
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
    }
}
