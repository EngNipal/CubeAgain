using System;
using static CubeAgain.Environment;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    class MonteCarloTreeSearch
    {
        // Метод, описывающий основную логику.
        /// <summary>
        /// Метод, осуществляющий поиск по дереву
        /// </summary>
        /// <param name="CurrentNode"> - узел, для которого определяется оценка</param>
        /// <param name="graph"> - граф, содержащий посещённые позиции</param>
        /// <returns> Уточнённая оценка всех ходов из переданного узла </returns>
        public static double[] GetProbDistrib(Node CurrentNode, Graph graph)
        {
            Path MainPath = new Path(CurrentNode);                      // Сюда записываем путь, по которому идём.
            for (int i = 0; i < Training.MaxNodes; i++)
            {
                if (CurrentNode.WasVisited)                             // Если текущий узел уже посещался, т.е. это промежуточный Node
                {
                    Turns BestTurn = CurrentNode.GetBestTurn();         // Выбираем лучший ход в узле согласно максимуму (Q + U). 
                    MainPath.AddStep(CurrentNode.Steps[BestTurn]);      // Добавляем ход в путь.
                    CurrentNode = CurrentNode.Steps[BestTurn].Node;     // Делаем новый узел текущим.
                }
                else                                                    // Текущий узел - это лист.
                {
                    CurrentNode.WasVisited = true;                      // Отмечаем посещение узла.
                    ExpandNode(CurrentNode, graph, MainPath);           // Раскрываем его.
                    MainPath.BackPropagate();                           // Обновляем параметры элементов пройденного пути.
                }
            }
            double[] result = new double[HeadPolicy.NumNeurons];         // Записываем и возвращаем уточнённые вероятности для ходов.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = CurrentNode.Steps[(Turns)i].Move.Visit;                     // TODO: !!!!!!!!!! Уточнить как именно выбирается ход !!!!!!!!!!!!!!!
            }
            return result;
        }
        // Метод раскрытия новых узлов. С проверкой на наличие в базе.
        public static void ExpandNode(Node NewNode, Graph graph, Path path)
        {
            for (Turns turn = Turns.R; turn <= Turns.F2; turn++) // Добавляем в текущий узел все девять шагов (поворот и шаг). Раскрываем узел.
            {
                Position childPos = NewNode.Position.PosAfterTurn(turn);                     // Позиция, полученная из конкретного хода.
                Node childNode = graph.NodeFromPosition(childPos, out bool NodeExists);         // Добавляем в граф узел, созданный по новой позиции.
                if (NodeExists)                                         // Если узел с такой позицией уже есть...
                {
                    if (path.Contains(childPos))                        // Если узел был в пройденном пути (сделан возвратный ход или произошло зацикливание).
                    {                                                   // Вводим коррекцию веса при повторении позиции.
                        NewNode.Steps[turn].Move.WinRate += Training.CorrectionIfPositionRepeats;
                    }
                    // Если узел есть, но его не было в текущем пути - сохраняем его оценки (ничего не делаем).
                }
                else // Такого узла ещё не было.
                {
                    double moveWinRate;
                    if (Solved.Equals(childPos))                                // Если это решённая позиция, то записываем в ход большУю оценку на основе длины пути.
                    {
                        moveWinRate = Training.EvaluationOfSolvedPosition;      // TODO: Ещё раз обдумать этот момент (18.01.2021).
                    }
                    else                                                        // Если не терминальный - берём оценку из оценки позиции, умноженную на discounted reward.
                    {
                        moveWinRate = childPos.Evaluation; // * Math.Pow(Training.DiscountCoeff, path.Length);
                    }
                    // Создаём в новом узле новый Step согласно текущему ходу turn.
                    Move currMove = new Move(Policy[(int)turn], 0, moveWinRate);
                    Step currStep = new Step(currMove, childNode);
                    NewNode.AddStep(turn, currStep);
                }
            }
        }
        
    }
}
