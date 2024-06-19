using System;

namespace Remotion.Mixins.XRef.UnitTests.MixinSampleAssembly
{
  [Extends(typeof(ConflictingClass))]
  public class ConflictingMixinB
  {
    public virtual void Foo ()
    {
    }
  }
}
