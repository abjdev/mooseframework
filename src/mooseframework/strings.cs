//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose
{
    internal unsafe class strings
    {
        public static int strlen(byte* c)
        {
            int i = 0;
            while (c[i] != 0)
            {
                i++;
            }

            return i;
        }
    }
}
