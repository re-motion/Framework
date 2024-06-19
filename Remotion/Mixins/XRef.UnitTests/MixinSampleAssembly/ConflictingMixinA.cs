using System;

namespace Remotion.Mixins.XRef.UnitTests.MixinSampleAssembly
{
  [Extends(typeof(ConflictingClass))]
  public class ConflictingMixinA
  {
    public virtual void Foo ()
    {
    }
  }
}
