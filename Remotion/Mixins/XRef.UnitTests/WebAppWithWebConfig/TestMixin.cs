using System;

namespace Remotion.Mixins.XRef.UnitTests.WebAppWithWebConfig
{
  public class TestMixin : Mixin<IDoSomething>
  {
    [OverrideTarget]
    public void DoSomething ()
    {
      throw new NotImplementedException();
    }
  }
}
