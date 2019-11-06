using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_base
{
    public class NullSolution : SolutionBase
    {
        public NullSolution() : base(string.Empty) { }

        public override int outOf { get { return 0; } }

        protected override bool responseIsCorrect(string response, IEnumerable<string> allResponses)
        {
            return false;
        }

    }
}
