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
    [Test]
    public void ReturnsCorrectBaseDirectoryAndRelativeSearchPath ()
    {
      var contextProvider = new AppContextProvider();
      Assert.That(contextProvider.BaseDirectory, Is.EqualTo(AppContext.BaseDirectory));
      Assert.That(contextProvider.RelativeSearchPath, Is.EqualTo(null));
    }
  }
}
