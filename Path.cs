using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeAgain
{
    public class Path
    {
        public Node Start;
        public int Length => Steps.Count();
        public List<Step> Steps { get; private set; }
        public Path(Node start)
        {
            Steps = new List<Step>();
            Start = start;
        }
        public void AddStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException("Получен пустой шаг при добавлении шага в путь.");
            }
            else
            {
                Steps.Add(step);
            }
        }
        /// <summary>
        /// Проверяет наличие позиции в пути.
        /// </summary>
        /// <param name="position"></param>
        /// <returns><see langword="true"/> - если в пути есть переданная позиция,
        /// <see langword="false"/> - в ином случае</returns>
        public bool Contains(Position position)
        {
            if (Start.Position.Equals(position))
            {
                return true;
            }
            foreach (Step step in Steps)
            {
                if (step.NextNode.Position.Equals(position))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Обновляет W и N для рёбер, по которым прошли.
        /// </summary>
        // Вызывается 128000 раз.
        public void BackPropagate()
        {
            if (Length > 0)
            {
                for (int i = Length - 1; i >=0; --i)
                {
                    Steps[i].Move.Visit++;
                    Steps[i].Move.WinRate += Steps[i].NextNode.Position.Evaluation;
                }
            }
        }
        /// <summary>
        /// Очищает путь. Не удаляет стартовый узел.
        /// </summary>
        public void Clear()
        {
            Steps.Clear();
        }
    }
}
