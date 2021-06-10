using System;
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AppContextProviderTest
  {
#if NETFRAMEWORK
    [Test]
    public void CodeRunInSeparateAppDomain_ReturnsCorrectBaseDirectoryAndRelativeSearchPath ()
    {
      var setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"C:\Base\";
      setup.DynamicBase = Path.GetTempPath();
      setup.PrivateBinPath = @"Custom\Relative;Search\Path";

      new AppDomainRunner (
          setup,
          delegate
          {
            var contextProvider = new AppContextProvider();
            Assert.That (contextProvider.BaseDirectory, Is.EqualTo (@"C:\Base\"));
            Assert.That (contextProvider.RelativeSearchPath, Is.EqualTo (@"Custom\Relative;Search\Path"));
          }).Run();
    }
#endif

#if !NETFRAMEWORK
    [Test]
    public void ReturnsCorrectBaseDirectoryAndRelativeSearchPath ()
    {
      var contextProvider = new AppContextProvider();
      Assert.That (contextProvider.BaseDirectory, Is.EqualTo (AppContext.BaseDirectory));
      Assert.That (contextProvider.RelativeSearchPath, Is.EqualTo (null));
    }
#endif
  }
}
