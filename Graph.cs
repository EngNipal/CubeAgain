using System.Collections.Generic;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    public class Graph
    {
        public Node Solved { get; }
        public delegate void MethodContainer(Position position);            // Делегат метода создания тренировочного набора (тупла).
        public static event MethodContainer Analyzed;                       // Событие проведения оценки новой позиции.
        public Graph()
        {
            Solved = NodeFromPosition(Environment.Solved);
            Solved.WasVisited = false;
        }
        // Словарь содержащий ссылки на узлы по их позициям
        private readonly Dictionary<Position, Node> PositionsNodes = new Dictionary<Position, Node>();

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
                Analyze(position.State);
                position.Evaluation = Evaluation;
                result = new Node(position);
                PositionsNodes.Add(position, result);
                Analyzed?.Invoke(position);
                for (Turns turn = Turns.R; turn <= Turns.F2; turn++)
                {
                    result.Steps[turn].Move.Policy = Policy[(int)turn];
                }
            }
            return result;
        }
    }
}
