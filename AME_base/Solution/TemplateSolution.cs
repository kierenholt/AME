using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_base
{
    public class TemplateSolution //NOT DERIVED FROM commentbase
    {

        //returns randomly generated values for markdown to use
        //returns simplecomment if markdown is input etc.

        //delimiters
        private const char LIST_DELIMITER = ',';
        private const string RANGE_DELIMITER = "..";
        private const char QUOTE = '"';
        private const char DOLLAR = '$';

        //BEHAVIOUR SETTINGS
        private const bool indicesAreVariables = true;
        private const bool precisionEnabled = false; //copies precision from left and right content into generated variables
        private const int OVERFLOW_LIMIT = 1000;

        private List<TemplateSolution> _allTemplateComments;
        private string _text;
        private int _indexForListEvaluation;
        private Random _random; 
        private int overflowCounter = 0;
        

        public TemplateSolution(string paramText, List<TemplateSolution> paramAllTemplateComments, Random paramRandom,
            int paramIndexForRangeEvaluation)
        {
            _allTemplateComments = paramAllTemplateComments;
            _random = paramRandom;
            _text = paramText;
            _indexForListEvaluation = paramIndexForRangeEvaluation; //ensures that lists are synchronised
        }
        
        //public void reset(int seed)
        //{
        //    _random = new Random(seed);
        //    _calculatedValue = String.Empty;
        //}


        [NonSerialized]
        private string _calculatedValue = null;
        private string calculatedValue
        {
            get
            {
                if (string.IsNullOrEmpty(_calculatedValue))
                    _calculatedValue = evaluateSingle(_text, 0).s.Trim(new[] { '"' });
                return _calculatedValue;
            }
        }
        //private void forceCalculate()
        //{
        //    if (string.IsNullOrEmpty(_calculatedValue))
        //        _calculatedValue = evaluateSingle(_text, 0).s.Trim(new[] { '"' });
        //}
        //private void setCalculatedValue(double value) { _calculatedValue = value.ToString(); } //used by fractions

        public string calculatedValueString
        {
            get
            {
                //SIGN EMULATION
                string sign = String.Empty;
                decimal number = 0m;
                //if (decimal.TryParse(calculatedValue,
                //    System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out number))

                return calculatedValue;
            }
        }

        private struct StringInt
        {
            public string s;
            public int i;
            public StringInt(string paramS, int paramI) { s = paramS; i = paramI; }
        }

        //http://stackoverflow.com/questions/333737/evaluating-string-342-yield-int-18
        private StringInt evaluateSingle(string s, int i)
        {
            //anything in brackets
            //http://stackoverflow.com/questions/14952113/how-can-i-match-nested-brackets-using-regex
            string buffer = string.Empty;
            while (i < s.Length && s[i] != ')')
            {
                if (overflowCounter++ > OVERFLOW_LIMIT)
                {
                    throw new RowException(RowException.ErrorCode.INFINITE_LOOP_ERROR);
                }
                //if ( concat next expression to current buffer
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateSingle(s, i + 1);
                    buffer += evaluated.s;
                    i = evaluated.i;
                }
                else
                {
                    if (s[i].isAlpha())
                    {
                        StringInt evaluated = evaluateLetters(s, i);
                        buffer += evaluated.s;
                        i = evaluated.i;
                    }
                    else
                    {
                        if (s[i] == 'π')
                        {
                            buffer += "3.14159265359";
                        }
                        else
                        {
                            if (i + 1 < s.Length && s.Substring(i, 2) == RANGE_DELIMITER)
                            {
                                return evaluateRange(buffer, s, i + 2);
                            }
                            else
                            {
                                if (s[i] == LIST_DELIMITER)
                                {
                                    return evaluateList(buffer, s, i + 1);
                                }
                                else
                                {
                                    if (s[i] == QUOTE)
                                    {
                                        return evaluateQuote(s, i);
                                    }
                                    else
                                    {
                                        if (s[i] == DOLLAR)
                                        {
                                            StringInt evaluated = evaluateDollar(s, i + 1);
                                            buffer += evaluated.s;
                                            i = evaluated.i;
                                        }
                                        else
                                        {
                                            buffer += s[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                i++;
            }

            if (string.IsNullOrWhiteSpace(buffer))
                return new StringInt(string.Empty, i);
            if (buffer[0] == '"')
                return new StringInt(buffer.Trim('"'), i); //allow quotes to  pass through
            Expression e = new Expression(buffer, EvaluateOptions.IgnoreCase );
            e.Options |= EvaluateOptions.RoundAwayFromZero;
            return new StringInt(e.Evaluate().ToString(), i);
        }

        private StringInt evaluateQuote(string s, int i)
        {
            i++; //skip first quote
            string buffer = string.Empty;
            while (i < s.Length && s[i] != QUOTE)
            {
                buffer += s[i];
                i++;
            }
            return new StringInt('"' + buffer + '"', i); //KEEP the quotation marks so that it can pass
        }

        private StringInt evaluateRange(string firstBuffer, string s, int i)
        {
            //ranges can contain variables and brackets
            string[] buffers = new string[2];
            buffers[0] = evaluateSingle(firstBuffer, 0).s;

            while (i < s.Length && s[i] != ')')
            {
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateSingle(s, i + 1);
                    buffers[1] += evaluated.s;
                    i = evaluated.i;
                }
                else
                {
                    buffers[1] += s[i];
                }
                i++;
            }

            String minString = evaluateSingle(buffers[0], 0).s;
            String maxString = evaluateSingle(buffers[1], 0).s;
            Decimal decimalmin = 0;
            Decimal decimalmax = 0;
            if (!Decimal.TryParse(minString, out decimalmin) || !Decimal.TryParse(maxString, out decimalmax))
                throw new RowException(RowException.ErrorCode.BAD_RANGE_EXPRESSION);
            int min = (int)Math.Ceiling(decimalmin);
            int max = (int)Math.Floor(decimalmax);
            if (min > max)
            {
                int tempSwapMinandMax = min;
                min = max;
                max = tempSwapMinandMax;
            }

            int temp = min == max ? min : min + _random.Next(max - min + 1);
            return new StringInt(temp.ToString(), i);
        }

        private StringInt evaluateList(string firstBuffer, string s, int i)
        {
            //lists can contain variables
            List<string> buffers = new List<string>() { firstBuffer, string.Empty };
            while (i < s.Length && s[i] != ')')
            {
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateSingle(s, i + 1);
                    buffers[buffers.Count - 1] += evaluated.s;
                    i = evaluated.i;
                }
                else
                {
                    if (s[i] == LIST_DELIMITER)
                    {
                        buffers.Add(string.Empty);
                    }
                    else
                    {
                        buffers[buffers.Count - 1] += s[i];
                    }
                }
                i++;
            }

            int randomIndex = _indexForListEvaluation % buffers.Count;
            return new StringInt(evaluateSingle(buffers[randomIndex], 0).s, i);
        }


        private StringInt evaluateLetters(string s, int i)
        {
            string buffer = string.Empty;
            //if bracket, evaluate function
            //if not a letter, ignore
            while (i < s.Length && s[i].isAlpha())
            {
                buffer += s[i];
                i++;
            }
            if (buffer.Length == 1 &&
                (buffer.ToLower() != "e" || (i < 2 || !s[i - 2].isNumeric()))) //allow abcdfgh OR e if previous char was NOT numeric i.e. not 5e)
                return evaluateVariable(s, i - 1);
            if (i < s.Length && s[i] == '(')
                return evaluateFunction(buffer, s, i + 1); //skip first bracket
            return new StringInt(buffer, i - 1); //pass through anything else e.g. "and"
        }


        private StringInt evaluateDollar(string s, int i)
        {
            //starts at number index
            string buffer = string.Empty;
            while (i < s.Length && s[i].isNumeric())
            {
                buffer += s[i];
                i++;
            }
            int index;
            if (Int32.TryParse(buffer, out index) && index >= 1 && index <= _allTemplateComments.Count)
                return new StringInt(_allTemplateComments[index - 1].calculatedValue, i);
            throw new RowException(RowException.ErrorCode.VARIABLE_DOES_NOT_EXIST);
        }

        private StringInt evaluateVariable(string s, int i)
        {
            int index = s[i].alphaIndex();
            if (index < _allTemplateComments.Count)
                return new StringInt(_allTemplateComments[index].calculatedValue, i);
            throw new RowException(RowException.ErrorCode.VARIABLE_DOES_NOT_EXIST);
        }

        public static string[] functionNames = new string[] { 
            "maxlength",
            "padleftzeroes",
            "padrightzeroes",
            "abs",
            "mean",
            "median",
            "mode",
            "max",
            "min",
            "if",
            "exponent",
            "mantissa",
            "HCF",
            "coprime",
            "includeSign",
            "includeOppSign"
        };

        private StringInt evaluateFunction(string functionName, string s, int i)
        {
            string functionNameLower = functionName.ToLower();

            //starts after first bracket
            List<string> parameters = new List<string>() { string.Empty };
            while (i < s.Length && s[i] != ')')
            {
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateSingle(s, i + 1);
                    parameters[parameters.Count - 1] += evaluated.s;
                    i = evaluated.i;
                }
                else
                {
                    if (s[i] == ',')
                    {
                        parameters.Add(string.Empty);
                    }
                    else
                    {
                        parameters[parameters.Count - 1] += s[i];
                    }
                }
                i++;
            }


            if (functionNameLower == "if")
                return new StringInt(
                        bool.Parse(evaluateSingle(parameters[0], 0).s) ?
                        evaluateSingle(parameters[1], 0).s :
                        evaluateSingle(parameters[2], 0).s,
                    i);

            //evaluate functions
            List<decimal> evaluatedParameters = parameters.Select(p => decimal.Parse(evaluateSingle(p, 0).s, System.Globalization.NumberStyles.Float)).ToList();


            if (functionNameLower == "exponent")
            {
                string asExponent = evaluatedParameters[0].ToString("E");
                int Eindex = asExponent.IndexOf('E');
                return new StringInt("0" + asExponent.Substring(Eindex + 1), i);
            }

            if (functionNameLower == "mantissa")
            {
                string asExponent = evaluatedParameters[0].ToString("E");
                int Eindex = asExponent.IndexOf('E');
                return new StringInt(asExponent.Substring(0,Eindex),i);
            }

            if (functionNameLower == "maxlength")
            {
                int max = Math.Min((int)evaluatedParameters[1], evaluatedParameters[0].ToString().Length); 
                return new StringInt(evaluatedParameters[0].ToString().Substring(0, max), i);
            }
            if (functionNameLower == "padleftzeroes")
                return new StringInt('"' + evaluatedParameters[0].ToString().PadLeft((int)evaluatedParameters[1], '0'), i);
            if (functionNameLower == "padrightzeroes")
            {
                string str = evaluatedParameters[0].ToString();
                if (!str.Contains('.'))
                    str += '.';
                return new StringInt('"' + str.PadRight((int)evaluatedParameters[1], '0'), i);
            }

            if (functionNameLower == "getdigit")
            {
                int n = (int)evaluatedParameters[0];
                List<int> digits = n.digits().ToList();
                int index = digits.Count - 1 - (int)Math.Log10((double)evaluatedParameters[1]);
                return new StringInt(digits[index].ToString(), i);
            }
            if (functionNameLower == "abs")
                return new StringInt(Math.Abs(evaluatedParameters[0]).ToString(), i);
            if (functionNameLower == "mean")
                return new StringInt(evaluatedParameters.Average().ToString(), i);
            if (functionNameLower == "median")
                return new StringInt(evaluatedParameters.Median().ToString(), i);
            if (functionNameLower == "mode")
                return new StringInt(evaluatedParameters.Mode().ToString(), i);
            if (functionNameLower == "max")
                return new StringInt(evaluatedParameters.Max().ToString(), i);
            if (functionNameLower == "min")
                return new StringInt(evaluatedParameters.Min().ToString(), i);
            if (functionNameLower == "hcf")
                return new StringInt(Helpers.HCF((int)evaluatedParameters[0], (int)evaluatedParameters[1]).ToString(), i);
            if (functionNameLower == "coprime")
            {
                int denom = (int)evaluatedParameters[0];
                if (denom < 2) return new StringInt("0", i);
                int guess = _random.Next(denom);
                while (Helpers.HCF(denom, guess) > 1)
                    guess = _random.Next(denom);
                return new StringInt(guess.ToString(), i);
            }
            if (functionNameLower == "includesign")
            {
                if (evaluatedParameters[0] > 0) return new StringInt(String.Format(@"""+ {0}""", evaluatedParameters[0].ToString()), i);
                return new StringInt(String.Format(@"""- {0}""", (-1*evaluatedParameters[0]).ToString()), i);
            }
            if (functionNameLower == "includeoppsign")
            {
                if (evaluatedParameters[0] > 0) return new StringInt(String.Format(@"""- {0}""", evaluatedParameters[0].ToString()), i);
                return new StringInt(String.Format(@"""+ {0}""",(-1*evaluatedParameters[0]).ToString()), i);
            }

            return new StringInt(new Expression(
                    functionName + '(' + string.Join(",", parameters.Select(p => evaluateSingle(p, 0).s)) + ')', EvaluateOptions.IgnoreCase
                ).Evaluate().ToString(), i); //leave anything else for ncalc

        }


        //private class Fraction
        //{
        //    public Variable whole;
        //    public Variable numerator;
        //    public Variable denominator;
        //    public bool onLeftSide;

        //    //simplify fractions 
        //    public void simplify(ref string[] _leftRight)
        //    {
        //        //only simplify if the original markup is simplified or if *both* are underscore

        //        int originalDenominatorInt = denominator.isUnderscore ? 0 : Convert.ToInt32(denominator.originalMarkDown.Replace(" ", ""));
        //        int originalNumeratorInt = numerator.isUnderscore ? 0 : Convert.ToInt32(numerator.originalMarkDown.Replace(" ",""));
        //        int originalHCF = Helpers.HCF(originalDenominatorInt, originalNumeratorInt);

        //        if ((denominator.isUnderscore && numerator.isUnderscore) || originalHCF == 1)
        //        {
        //            int denominatorInt = Convert.ToInt32(denominator.calculatedValue);
        //            int numeratorInt = Convert.ToInt32(numerator.calculatedValue);

        //            int HCF = Helpers.HCF(denominatorInt, numeratorInt);
        //            denominatorInt /= HCF;
        //            numeratorInt /= HCF;

        //            if (whole != null) //convert from top heavy
        //            {
        //                int wholeInt = Convert.ToInt32(whole.calculatedValue);
        //                wholeInt += numeratorInt / denominatorInt;
        //                whole.setCalculatedValue(wholeInt);
        //                whole.removedDueToZeroFraction = wholeInt == 0;
        //                numeratorInt = numeratorInt % denominatorInt;
        //            }

        //            //update variables
        //            denominator.setCalculatedValue(denominatorInt);
        //            numerator.setCalculatedValue(numeratorInt);

        //            //if numerator is zero then remove the slash
        //            if (numeratorInt == 0)
        //            {
        //                numerator.removedDueToZeroFraction = true;
        //                denominator.removedDueToZeroFraction = true;
        //                removeSlash(ref _leftRight);

        //                //if whole and numerator are both zero then show the whole as zero
        //                if (whole != null && Convert.ToInt32(whole.calculatedValue) == 0)
        //                    whole.removedDueToZeroFraction = false;
        //            }
        //        }
        //    }

        //    private void removeSlash(ref string[] _leftRight)
        //    {
        //        int index = onLeftSide ? 0 : 1;
        //        _leftRight[index] = _leftRight[index].Remove(numerator.lastIndex, 1).Insert(numerator.lastIndex, " ");
        //    }
        //} //end of fraction class


    }
}
