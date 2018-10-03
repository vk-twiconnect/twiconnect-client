using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TWIConnect.Client.Utilities
{
  class Threading
  {
    internal static void AsyncCallWithTimeout(Action action, int timeoutMilliseconds)
    {
      Thread threadToKill = null;
      Action wrappedAction = () =>
      {
        threadToKill = Thread.CurrentThread;
        action();
      };

      IAsyncResult result = wrappedAction.BeginInvoke(null, null);
#if DEBUG
      if (result.AsyncWaitHandle.WaitOne(int.MaxValue))
#else
      if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
#endif
      {
        wrappedAction.EndInvoke(result);
      }
      else
      {
        threadToKill.Abort();
        throw new TimeoutException();
      }
    }

    internal static T AsyncCallWithTimeout<T>(Func<T> operation, int timeoutMilliseconds)
    {
      Task<T> task = Task.Factory.StartNew<T>(operation);
      task.Wait(timeoutMilliseconds);

      if (!task.IsCompleted)
      {
        throw new TimeoutException();
      }
      else
      {
        return task.Result;
      }
    }
  }
}
