using System;

namespace Remotion.Mixins.XRef.UnitTests.MixinSampleAssembly
{
  [Extends(typeof(TargetClassA))]
  public class MixinClassA
  {
    [OverrideTarget]
    public virtual void Foo ()
    {
    }
  }
}
