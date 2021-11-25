using System;
using System.Threading;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  public static class WxeThreadAbortHelper
  {
    public static void Abort ()
    {
#if NETFRAMEWORK
      Thread.CurrentThread.Abort();
#else
      var exception = (ThreadAbortException) Activator.CreateInstance(typeof (ThreadAbortException), nonPublic: true)!;
      throw exception;
#endif
    }

    public static void ResetAbort ()
    {
#if NETFRAMEWORK
      Thread.ResetAbort();
#endif
    }
  }
}