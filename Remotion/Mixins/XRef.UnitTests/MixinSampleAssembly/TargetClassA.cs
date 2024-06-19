using System;

namespace Remotion.Mixins.XRef.UnitTests.MixinSampleAssembly
{
  [Uses(typeof(UsedMixinB))]
  public class TargetClassA
  {
    public virtual void Foo ()
    {
    }

    [OverrideMixin]
    public virtual void Foo2 ()
    {
    }
  }
}
