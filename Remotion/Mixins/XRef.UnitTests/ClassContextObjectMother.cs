using System;
using System.Linq;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.XRef.UnitTests;

public static class ClassContextObjectMother
{
  public static ClassContext Create (Type type = null)
  {
    type = type ?? typeof(object);
    return new ClassContext(type, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>());
  }
}
