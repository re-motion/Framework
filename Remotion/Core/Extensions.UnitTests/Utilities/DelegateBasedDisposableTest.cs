// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Extensions.UnitTests.Utilities
{
  [TestFixture]
  public class DelegateBasedDisposableTest
  {
    [Test]
    public void Dispose_ExecutesTheDelegate ()
    {
      bool delegateExecuted = false;
      var scope = new DelegateBasedDisposable(() => delegateExecuted = true);

      Assert.That (delegateExecuted, Is.False);

      scope.Dispose();

      Assert.That (delegateExecuted, Is.True);
    }

    [Test]
    public void Dispose_Twice_SecondCallIsIgnored ()
    {
      int delegateExecutionCount = 0;

      var scope = new DelegateBasedDisposable (() => ++delegateExecutionCount);

      Assert.That (delegateExecutionCount, Is.EqualTo (0));

      scope.Dispose ();
      scope.Dispose ();

      Assert.That (delegateExecutionCount, Is.EqualTo (1));
    }
  }
}