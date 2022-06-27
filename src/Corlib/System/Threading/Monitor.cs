//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Threading
{
    public static unsafe class Monitor
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Enter(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (ThreadPool.CanLock)
            {
                ThreadPool.Lock();
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static void Exit(object obj)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (ThreadPool.CanLock)
            {
                ThreadPool.UnLock();
            }
        }
    }
}