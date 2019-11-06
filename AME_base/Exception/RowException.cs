using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AME_base
{
    public class RowException : Exception
    {
        //for instances when question generation / preview needs to be halted
        //syntax errors

        public enum ErrorCode
        {
            INFINITE_LOOP_ERROR = 0, //from generator
            BAD_RANGE_EXPRESSION = 1, //from generator
            DUPLICATED_LETTER = 3,
            VARIABLE_DOES_NOT_EXIST = 4
        }
        private static readonly string[] messages = 
        { 
            "there is an infinite loop within one or more of your variables e.g. <solution for a>: b + c, but <solution for b>: a + 1",
            "ranges must be whole numbers in the format '1 to 5' or '12 to 24' etc.",
            "one of the choices (A B C D) has been used more than once (e.g. A B B C)",
            "the named variable e.g. a, b etc. does not exist"
        };
        
        
        public RowException(ErrorCode errorCode)
            : base(messages[(int)errorCode])
        {
        }

    }
}
