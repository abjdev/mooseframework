//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System
{
    public struct Char
    {
        public override string ToString()
        {
            string r = " ";
            r._firstChar = this;

            return r;
        }

        public char ToUpper()
        {
            char chr = this;
            if (chr >= 'a' && chr <= 'z')
            {
                chr -= (char)('a' - 'A');
            }

            return chr;
        }

        public static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}