using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_base
{
    public class SimpleSolution : SolutionBase //belongs to subquestion along with markdownbase
    {
        public SimpleSolution(string paramText) : base(paramText) { }

        public override int outOf { get { return 1; } }

        protected override bool responseIsCorrect(string response, IEnumerable<string> allResponses)
        {
            if (correctResponse == "*")
                return !string.IsNullOrEmpty(response); //wildcard * takes any response as correct answer
            return SimpleSolution.compare(correctResponse, response);
        }

        private static bool compare(string a, string b) 
        {
            bool retVal = false;
            decimal doubleCorrect, doubleResponse;
            if (decimal.TryParse(a, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out doubleCorrect) &
                    decimal.TryParse(b, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out doubleResponse))
                retVal = Math.Abs(doubleCorrect - doubleResponse) <= Math.Abs(doubleCorrect * QuestionAssignmentHTML.ALLOWABLE_ERROR_FOR_CORRECT_ANSWER); //within 5%
            else
                retVal = string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            return retVal;
        }
    }
}
