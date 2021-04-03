using System;
using System.Collections.Generic;
using System.Linq;
using static CubeAgain.Environment;
using static CubeAgain.NeuralNetwork;

namespace CubeAgain
{
    class Program
    {
        private const double Zero = 0.0;

        static void Main(string[] args)
        {
            // Общая логика:
            // 0. Задали позицию запутыванием.
            // 1. Получили позицию.
            // 2. Оценили сетью.
            // 3. Записали оценку сети в обучающий сет.
            // 4. Походили по дереву - получили уточнённые оценки.
            // 5. Записали уточнённые оценки в обучающий сет.
            // 6. Сделали ход - получили новую позицию.
            // 7. Повторили шаги 2-6 пока не придём к выигрышу.
            // 8. На собранном сете провели корректировку весов нейросети.
            // 9. Повторили шаги 0-8 пока не научимся.

            IEnumerable<int> CurrState = SetSolved();                        // Устанавливаем решённую позицию в CurrState.
            SetNetworkStructure();                              // Задаём структуру нейронной сети.
            Training.SaveNetWeights();                          // Запись текущих весов NN в тренировочную базу.
            Analyzed += Training.AddTuple;                      // Подписка тренировочной базы на анализ позиций.
            Random Rnd = new Random();
            SetScramble(CurrState, Rnd.Next(1, 16), out Turns[] NewScramble);             // Задаём скрамбл длиной до 15 ходов и запутываем куб.
            WriteScramble(NewScramble);                                                 // Выводим скрамбл в консоль
            Position CurrentPos = new Position(CurrState);                // Текущая позиция - это запутанная CurrState.
            Node CurrentNode = new Node(CurrentPos);                    // Текущий узел.
            Path GamePath = new Path(CurrentNode);                      // Список ходов, по которым идёт игра.
            Graph MainGraph = new Graph();                           // Начинаем граф.
            const int TrainDataSetVolume = 128;                              // Объём тренировочной базы данных!!!!!!!!!!!!!!!
            TrainTuple[] MiniBatch = new TrainTuple[TrainDataSetVolume];     // Тренировочная база данных.
            for (int Tau = 0; Tau < TrainDataSetVolume; Tau++)               // Набираем минибатч...
            {
                double[] ImprovedPolicy = MonteCarloTreeSearch.GetProbDistrib(CurrentNode, MainGraph);    // Улучшенная оценка ходов, на основе MCTS.
                TrainTuple CurrentTuple = Training.DataBase[CurrentPos];                        // В словаре находим тупл, соответствующий CurrentPos.
                ImprovedPolicy.CopyTo(CurrentTuple.ImprovedPolicy, 0);                          // Дописываем туда улучшенную оценку. Другие параметры были записаны ранее,
                                                                                                // при создании нового узла в графе. Смотри класс Graph.
                CurrentTuple.PathLength = GamePath.Length;                                      // Дописываем в тупл длину пути.
                MiniBatch[Tau] = CurrentTuple;                                                  // Сохраняем теперь уже полноценный тупл в базу.
                Turns BestTurn = Training.Argmax(ImprovedPolicy);                               // Нашли наилучший ход.
                GamePath.AddStep(CurrentNode.Steps[BestTurn]);                                  // Добавили шаг в путь игры.
                                                                                                // Сделали ход, согласно максимуму из ImprovedPolicy.
                CurrentPos = CurrentPos.PosAfterTurn(BestTurn);                                 // Назначили новую позицию текущей.
                CurrentNode = MainGraph.NodeFromPosition(CurrentPos);
                if (Solved.Equals(CurrentPos))                                          // Если текущая позиция - решённая...
                {
                    CurrentTuple.PathLength++;                                          // Увеличиваем длину пути в тупле.
                    CurrentTuple.Reward = 1;                                            // Назначаем Reward.
                                                                                        // TODO: определиться с Reward-ом.
                    CurrState = SetSolved();
                    SetScramble(CurrState, Rnd.Next(1, 16));                              // Задаём новый скрамбл, запутываем им текущую позицию.
                    CurrentPos = new Position(CurrState);                                 // Текущая позиция - это запутанная CurrState.
                    Analyze(CurrentPos);
                    CurrentNode = MainGraph.NodeFromPosition(CurrentPos);            // Получаем узел по позиции.
                    GamePath.Clear();                                                   // Очищаем путь игры.
                    GamePath.Begin = CurrentNode;                                       // Начинаем новый путь с текущего узла.
                }
            }
            // TODO: Дописать этот блок!!!!!
            // *** Корректировка весов ***
            const double RegulCoeff = 0.001;                                            // Гиперпараметр регуляризации.
            // Подсчитываем Лосс-функцию.
            foreach (TrainTuple tuple in MiniBatch)
            {
                double z = GamePath.Length - tuple.PathLength;                    // z - Количество ходов, которое реально прошло до решенной позиции
                double v = tuple.Score;                                               // v - Оценка нейросети сколько ходов ещё до конца из этой позиции.
                double VLoss = (z - v) * (z - v);                                       // Квадрат разности между этими величинами - есть VLoss.
                double RLoss = Zero;                                                     // RLoss - L2 регуляризация, умноженная на коэффициент регуляризации.
                RLoss += (from block in Blocks select block.FCL.RegSum).Sum();
                RLoss *= RegulCoeff;
                double PLoss = Zero;                                                     // PLoss - cross-entropy loss.
                for (int i = 0; i < tuple.ImprovedPolicy.Length; i++)
                {
                    PLoss += tuple.ImprovedPolicy[i] * (Zero - Math.Log(tuple.SourcePolicy[i]));
                }
                double FullLoss = VLoss + PLoss + RLoss;

                for (int i = NumBlocks - 1; i >= 0; i--)
                {
                    //Blocks[i]
                }
            }

            WriteState(CurrState);
        }
    }
}
