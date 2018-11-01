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
using System.Runtime.Remoting.Messaging;
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class SafeContextIntegrationTest
  {
    [Test]
    public void SetGetFreeData ()
    {
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.Null);

      SafeContext.Instance.SetData ("Integration", "value");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.EqualTo ("value"));
      Assert.That (CallContext.GetData ("Integration"), Is.EqualTo ("value"));

      SafeContext.Instance.SetData ("Integration", "other value");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.EqualTo ("other value"));

      SafeContext.Instance.FreeData("Integration");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.Null);
      Assert.That (CallContext.GetData ("Integration"), Is.Null);
    }
  }
}
