using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class RequiresTestDatabaseAttribute : Attribute, ITestAction
  {
    public void BeforeTest (ITest test)
    {
      SetUpFixture.Setup();
    }

    public void AfterTest (ITest test)
    {
    }

    public ActionTargets Targets => ActionTargets.Suite;
  }
}
