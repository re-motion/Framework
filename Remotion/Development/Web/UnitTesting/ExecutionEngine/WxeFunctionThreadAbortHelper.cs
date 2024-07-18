using System;
using System.Threading;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  public static class WxeThreadAbortHelper
  {
    public static void Abort ()
    {
      var exception = (ThreadAbortException)Activator.CreateInstance(typeof(ThreadAbortException), nonPublic: true)!;
      throw exception;
    }

    public static void ResetAbort ()
    {
    }
  }
}
