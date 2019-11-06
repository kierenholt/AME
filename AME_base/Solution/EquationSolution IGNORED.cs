using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AME_base
{
    public class EquationSolution : SolutionBase
    {
        public EquationSolution(string paramText) : base(paramText) { }

        private bool haltOnEmptyField = false; //avoids null errors if a variable refers to a blank field
        private string[] _allSubResponses;

        //equationsolution could point to other solutions e.g. =if (a, b==c, etc.) 
        protected  override bool responseIsCorrect(string response, IEnumerable<string> paramAllSubResponses)
        {
            _allSubResponses = paramAllSubResponses.ToArray();
            return SimpleSolution.compare(evaluateBracketed(correctResponse.Substring(1), 0).s, response);
        }


        public override int outOf { get { return 1; } }

        private struct StringInt
        {
            public string s;
            public int i;
            public StringInt(string paramS, int paramI) { s = paramS.ToLower(); i = paramI; }
        }


        private StringInt evaluateEquality(string firstBuffer, string s, int i)
        {
            //http://stackoverflow.com/questions/14952113/how-can-i-match-nested-brackets-using-regex
            //ranges can contain variables and brackets
            string[] buffers = new string[2];
            buffers[0] = firstBuffer;

            while (i < s.Length && s[i] != ')')
            {
                //if { concat next expression to current buffer
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateBracketed(s, i + 1);
                    buffers[1] += evaluated.s;
                    i = evaluated.i;
                }
                else
                {
                    if (s[i].isAlpha())
                    {
                        StringInt evaluated = evaluateLetters(s, i);
                        buffers[1] += evaluated.s;
                        i = evaluated.i;
                    }
                    else
                    {
                        buffers[1] += s[i];
                    }
                }
                i++;
            }

            return new StringInt(compareWithinError(buffers[0], buffers[1]).ToString(), i);
        }

        private bool compareWithinError(dynamic a, dynamic b)
        {
            bool retVal = false;
            string evaluatedA = evaluateBracketed(a.ToString(), 0).s;
            string evaluatedB = evaluateBracketed(b.ToString(), 0).s;

            //try decimal
            decimal leftD = 0M;
            decimal rightD = 0M;
            bool leftSuccessD = !haltOnEmptyField && decimal.TryParse(evaluatedA,
                    System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out leftD);
            bool rightSuccessD = !haltOnEmptyField && decimal.TryParse(evaluatedB,
                    System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out rightD);
            if (leftSuccessD && rightSuccessD)
                retVal = Math.Abs(leftD - rightD) <= 0.05M * Math.Abs(leftD);

            //try bools
            bool leftB = false;
            bool rightB = false;
            bool leftSuccessB = !haltOnEmptyField && bool.TryParse(evaluatedA, out leftB);
            bool rightSuccessB = !haltOnEmptyField && bool.TryParse(evaluatedB, out rightB);
            if (leftSuccessB && rightSuccessB)
                retVal = leftB == rightB;

            return retVal;
        }

        private StringInt evaluateBracketed(string s, int i)
        {
            //anything in brackets
            //http://stackoverflow.com/questions/14952113/how-can-i-match-nested-brackets-using-regex
            string buffer = string.Empty;
            while (i < s.Length && s[i] != ')')
            {
                //if ( concat next expression to current buffer
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateBracketed(s, i + 1);
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
                        if (s[i] == '=')
                        {
                            return evaluateEquality(buffer, s, i + 1);
                        }
                        else
                        {
                            if (s[i] == '$')
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
                i++;
            }

            //end of string or }
            if (haltOnEmptyField)
                return new StringInt("", i);
            Expression e = new Expression(buffer, EvaluateOptions.IgnoreCase);
            e.Options |= EvaluateOptions.RoundAwayFromZero;

            string retVal = string.Empty;
            try
            {
                object evaluated = e.Evaluate();
                if (evaluated != null)
                    retVal = evaluated.ToString();
            }
            catch (EvaluationException err)
            {
                retVal = buffer;
            }
            return new StringInt(retVal, i);
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
            if (Int32.TryParse(buffer, out index) && index >= 1 && index <= _allSubResponses.Count())
                return new StringInt(_allSubResponses[index - 1], i);
            throw new RowException(RowException.ErrorCode.VARIABLE_DOES_NOT_EXIST);
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
                (buffer.ToLower() != "e" || (i < 2 || !s[i - 2].isNumeric()))) //allow abcdfgh OR e if previous char was NOT numeric i.e. not 5e
                return evaluateVariable(s, i - 1, true);
            if (i < s.Length && s[i] == '(')
                return evaluateFunction(buffer, s, i + 1); //skip first bracket
            return new StringInt(buffer, i - 1); //pass through anything else e.g. "and"
        }

        private StringInt evaluateVariable(string s, int i, bool triggerHaltOnEmptyField)
        {
            int index = s[i].alphaIndex();
            string found = null;
            if (index < _allSubResponses.Count())
                found = _allSubResponses[s[i].alphaIndex()];

            string retVal;
            if (found == null)
            {
                retVal = s;
            }
            else
            {
                retVal = found;
                haltOnEmptyField = haltOnEmptyField || (triggerHaltOnEmptyField && string.IsNullOrEmpty(retVal));
            }
            return new StringInt(retVal, i);
        }


        private StringInt evaluateFunction(string functionName, string s, int i)
        {
            string functionNameLower = functionName.ToLower();

            //one parameter one char can be done now - also remove the haltonblankfield check
            if (functionNameLower == "isfull" || functionNameLower == "full" ||
                functionNameLower == "isfilled" || functionNameLower == "filled" ||
                functionNameLower == "isempty" || functionNameLower == "empty")
            {
                StringInt evaluated = evaluateVariable(s, i, false); //skip the first bracket
                bool result = (functionNameLower == "isfull" || functionNameLower == "full" ||
                    functionNameLower == "isfilled" || functionNameLower == "filled") == !string.IsNullOrEmpty(evaluated.s);
                return new StringInt(result.ToString(), evaluated.i + 1); //skip the end bracket
            }
            if (functionNameLower == "countfull" || functionNameLower == "countempty")
            {
                //starts after first bracket
                List<string> parameters2 = new List<string>() { string.Empty };
                while (i < s.Length && s[i] != ')')
                {
                    if (s[i] == ',')
                    {
                        parameters2.Add(string.Empty);
                    }
                    else
                    {
                        parameters2[parameters2.Count - 1] += s[i];
                    }
                    i++;
                }
                int result = parameters2.Count(p =>
                    (functionNameLower == "countfull") == !string.IsNullOrEmpty(evaluateVariable(p, 0, false).s));
                return new StringInt(result.ToString(), i); //skip the end bracket
            }

            //starts after first bracket
            List<string> parameters = new List<string>() { string.Empty };
            while (i < s.Length && s[i] != ')')
            {
                //if { concat next expression to current buffer
                if (s[i] == '(')
                {
                    StringInt evaluated = evaluateBracketed(s, i + 1);
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
                        if (s[i].isAlpha())
                        {
                            StringInt evaluated = evaluateLetters(s, i);
                            parameters[parameters.Count - 1] += evaluated.s;
                            i = evaluated.i;
                        }
                        else
                        {
                            parameters[parameters.Count - 1] += s[i];
                        }
                    }
                }
                i++;
            }

            //evaluate functions
            if (functionNameLower == "mean")
                return new StringInt(parameters.Select(p => decimal.Parse(evaluateBracketed(p, 0).s)).Average().ToString(), i);
            if (functionNameLower == "median")
                return new StringInt(parameters.Select(p => decimal.Parse(evaluateBracketed(p, 0).s)).Median().ToString(), i);
            if (functionNameLower == "mode")
                return new StringInt(parameters.Select(p => decimal.Parse(evaluateBracketed(p, 0).s)).Mode().ToString(), i);
            if (functionNameLower == "max")
                return new StringInt(parameters.Select(p => decimal.Parse(evaluateBracketed(p, 0).s)).Max().ToString(), i);
            if (functionNameLower == "min")
                return new StringInt(parameters.Select(p => decimal.Parse(evaluateBracketed(p, 0).s)).Min().ToString(), i);
            if (functionNameLower == "in")
            {
                string[] evaluatedParameters = parameters.Select(p => evaluateBracketed(p, 0).s).ToArray();
                return new StringInt(evaluatedParameters.Skip(1).Any(p => compareWithinError(p, evaluatedParameters[0])).ToString(), i);
            }
            if (functionNameLower == "python")
            {
                return new StringInt(doPython(
                    parameters[0],
                    parameters.Skip(1).Select(p => evaluateBracketed(p, 0).s).ToArray()), i);
            }

            return new StringInt(new Expression(
                    functionName + '(' + string.Join(",", parameters) + ')', EvaluateOptions.IgnoreCase
                ).Evaluate().ToString(), i); //leave anything else for ncalc
        }

        private string doPython(string codeString, string[] parameters)
        {
            if (!String.IsNullOrEmpty(codeString))
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                ScriptSource source = engine.CreateScriptSourceFromString(codeString);
                try
                {
                    source.Execute(scope);
                }
                catch
                {
                    return "!compile error!";
                }
                PythonFunction funcPython; //used for info about the function
                //try to get function 
                var functionKVs = scope.GetItems().Where(kv => kv.Value is PythonFunction);
                if (functionKVs.Any())
                {
                    funcPython = functionKVs.First().Value;
                }
                else
                {
                    return String.Format("def statement not found");
                }
                dynamic funcC = scope.GetVariable<dynamic>(funcPython.__name__);
                dynamic[] funcParameters = parameters.Select(p => allParse(p)).ToArray();
                dynamic funcReturn = null;
                if (funcPython.__code__.co_argcount != parameters.Count())
                {
                    return "!wrong number of parameters!";
                }
                try
                {
                    if (funcPython.__code__.co_argcount == 0)
                        funcReturn = funcC();
                    if (funcPython.__code__.co_argcount == 1)
                        funcReturn = funcC(funcParameters[0]);
                    if (funcPython.__code__.co_argcount == 2)
                        funcReturn = funcC(funcParameters[0], funcParameters[1]);
                    if (funcPython.__code__.co_argcount == 3)
                        funcReturn = funcC(funcParameters[0], funcParameters[1], funcParameters[2]);
                    if (funcPython.__code__.co_argcount == 4)
                        funcReturn = funcC(funcParameters[0], funcParameters[1], funcParameters[2], funcParameters[3]);
                    if (funcPython.__code__.co_argcount == 5)
                        funcReturn = funcC(funcParameters[0], funcParameters[1], funcParameters[2], funcParameters[3], funcParameters[4]);
                    if (funcReturn != null)
                    {
                        return Convert.ToString(funcReturn);
                    }
                }
                catch
                {
                    return "compile error";
                }
            }
            return string.Empty;
        }

        private dynamic allParse(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (str == "True") return true;
            if (str == "False") return false;

            int intVal;
            if (int.TryParse(str, out intVal))
                return intVal;
            decimal decVal;
            if (decimal.TryParse(str,
                    System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo, out decVal))
                return decVal;
            return str;
        }
        
    }
}
