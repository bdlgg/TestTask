using System.Threading;

namespace TestTask;

public static class Server
{
    private static int count = 0;
    private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

    public static int GetCount()
    {
     locker.EnterReadLock();
     try
     {
         return count;
     }
     finally{
         locker.ExitReadLock();
     }
    }

    public static void AddToCount(int value)
    {
        locker.EnterWriteLock();
        try
        {
            count += value;
        }
        finally{
            locker.ExitWriteLock();
        }
    }
    
}