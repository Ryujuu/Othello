using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    public class PositionEvaluationResult
    {
        public Position? coordinates;
        public double score;
        public int depth;
    }
}
