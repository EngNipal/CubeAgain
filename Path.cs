using System;
using System.Collections.Generic;

namespace CubeAgain
{
    class Path
    {
        public Node Begin { get; set; }
        public int Length { get; private set; }
        public List<Step> Steps { get; private set; }
        public Path(Node node)
        {
            Begin = node;
            Length = 0;
            Steps = new List<Step>();
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
                Length++;
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
            foreach (Step step in Steps)
            {
                if (step.Node.Position.Equals(position))
                {
                    return  true;
                }
            }
            return false;
        }
        /// <summary>
        /// Обновляет W и N для рёбер, по которым прошли.
        /// </summary>
        public void BackPropagate() // TODO: См тудушку в классе Node.
        {
            if (Length > 0)
            {
                for (int i = Length - 2; i >= 0; i--)
                {
                    Steps[i].Move.Visit++;
                    Steps[i].Move.WinRate += Steps[i].Node.Position.Evaluation;
                }
            }
        }
        /// <summary>
        /// Очищает путь и обнуляет начальный узел.
        /// </summary>
        public void Clear()
        {
            Steps.Clear();
            Begin = null;
        }
    }
}
