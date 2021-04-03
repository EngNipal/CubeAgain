using System.Collections.Generic;

namespace CubeAgain
{
    public class Graph
    {
        public Node Solved { get; } // TODO: Перенести в класс узла.
        public Graph()
        {
            Solved = NodeFromPosition(Environment.Solved);
            Solved.WasVisited = false;
        }
        // Словарь содержащий ссылки на узлы по их позициям
        internal static readonly Dictionary<Position, Node> PositionsNodes = new Dictionary<Position, Node>();

        ///  Метод получения узла в дереве по позиции
        /// <param name="position">Позиция для узла</param>
        /// <param name="node">Возвращает узел содержащий позицию</param>
        /// <returns><see langword="true"/> - если возвращён новый узел,
        /// <see langword="false"/> - если возвращён уже имеющийся узел</returns>
        public Node NodeFromPosition(Position position) => NodeFromPosition(position, out _);
        public Node NodeFromPosition(Position position, out bool exists)
        {
            // Проверяется наличие позиции в уже созданных
            // если нет, то создаётся новый узел с этой позицией
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
                PositionsNodes.Add(position, result);
                for (Turns turn = Turns.R; turn <= Turns.F2; turn++)
                {
                    result.Steps[turn].Move.Policy = NeuralNetwork.Policy[(int)turn];
                }
            }
            return result;
        }
    }
}
