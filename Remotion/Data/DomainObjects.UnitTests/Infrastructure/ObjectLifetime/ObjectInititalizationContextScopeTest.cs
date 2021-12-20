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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectInititalizationContextScopeTest
  {
    private Mock<IObjectInitializationContext> _initializationContext;

    [SetUp]
    public void SetUp ()
    {
      _initializationContext = new Mock<IObjectInitializationContext>();
    }

    [Test]
    public void CurrentObjectInitializationContext_NullByDefault ()
    {
      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }

    [Test]
    public void CurrentObjectInitializationContext_NonNullWhileInScope ()
    {
      using (new ObjectInititalizationContextScope(_initializationContext.Object))
      {
        Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Not.Null.And.SameAs(_initializationContext.Object));
      }
    }

    [Test]
    public void CurrentObjectInitializationContext_NullAfterScope ()
    {
      using (new ObjectInititalizationContextScope(_initializationContext.Object))
      {
      }
      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }

    [Test]
    public void CurrentObjectInitializationContext_NestedScopes ()
    {
      var initializationContext2 = new Mock<IObjectInitializationContext>();

      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
      using (new ObjectInititalizationContextScope(_initializationContext.Object))
      {
        Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Not.Null.And.SameAs(_initializationContext.Object));
        using (new ObjectInititalizationContextScope(initializationContext2.Object))
        {
          Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Not.Null.And.SameAs(initializationContext2.Object));
        }
        Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Not.Null.And.SameAs(_initializationContext.Object));
      }
      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }

    [Test]
    public void CurrentObjectInitializationContext_ThreadStatic ()
    {
      using (new ObjectInititalizationContextScope(_initializationContext.Object))
      {
        ThreadRunner.Run(() => Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null));
      }
      Assert.That(ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }
  }
}
