using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    public abstract class SolutionBase
    {
        private const char CONDITION_PREFIX = '=';  //separates conditioncomment from simplecomment
        public string correctResponse { get; protected set; }
        protected  string _feedback;
        public abstract int outOf { get; }

        public static SolutionBase create(string paramText)
        {
            if (string.IsNullOrEmpty(paramText))
                return new NullSolution();
            //if (paramText.Any() && paramText[0] == CONDITION_PREFIX)
            //    return new EquationSolution(paramText);
            return new SimpleSolution(paramText);
        }


        protected SolutionBase(string paramText)
        {
            correctResponse = paramText;
        }

        protected abstract bool responseIsCorrect(string response, IEnumerable<string> allResponses);
        //public string feedback { get { return _text[1]; } }


        public void checkScore(SubResponse subresponse, IEnumerable<SubResponse> _responses)
        {
            //if markdown is checkbox, then responseiscorrect tests whether true
            //if markdown is input, responseiscorrect tests equality

            if (responseIsCorrect(subresponse.Text, _responses.Select(r => r.Text)))
                subresponse.Score = 1;
            else
                subresponse.Score = 0;
            subresponse.decision = AME_base.SubResponse.Decision.decided; //adjusts decision state
        }
    }
}
