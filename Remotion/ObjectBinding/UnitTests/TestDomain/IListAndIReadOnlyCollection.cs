using System;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.ObjectBinding.UnitTests.TestDomain
{
  public interface IListAndIReadOnlyCollection<T> : IReadOnlyCollection<T>, IList
  {
  }
}