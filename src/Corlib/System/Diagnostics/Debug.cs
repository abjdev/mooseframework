//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Diagnostics
{
    public static class Debug
    {
        public static void WriteLine(string s)
        {
            Serial.WriteLine(s);
            s.Dispose();
        }

        public static void WriteLine()
        {
            Serial.WriteLine();
        }

        public static void Write(char c)
        {
            Serial.Write(c);
        }

        public static void Write(string s)
        {
            Serial.Write(s);
            s.Dispose();
        }
    }
}