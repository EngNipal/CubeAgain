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
            Length = 1;
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
        // Метод, обновляющий W и N для рёбер, по которым прошли. (Back Propagation)
        public void BackPropagate()
        {
            for (int i = Length - 1; i >= 0; i--)
            {
                Steps[i].Move.Visit++;
                Steps[i].Move.WinRate += Steps[i].Node.Position.Evaluation;
            }
        }
        public void Clear()
        {
            Steps.Clear();
            Begin = null;
        }
    }
}
