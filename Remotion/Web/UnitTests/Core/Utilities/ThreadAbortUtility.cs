using System.Threading;
using NUnit.Framework;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  public class ThreadAbortUtility
  {
    public static void Abort ()
    {
#if NETFRAMEWORK
      Thread.CurrentThread.Abort();
#else
      Assert.Ignore("This test uses Thread.Abort, which is not supported under .NET Core."); 
#endif
    }

    public static void ResetAbort ()
    {
#if NETFRAMEWORK
      Thread.ResetAbort();
#else
      Assert.Ignore("This test uses Thread.Abort, which is not supported under .NET Core."); 
#endif
    }
  }
}