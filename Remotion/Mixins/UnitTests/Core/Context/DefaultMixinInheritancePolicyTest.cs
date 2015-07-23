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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class DefaultMixinInheritancePolicyTest
  {
    private DefaultMixinInheritancePolicy _policy;

    [SetUp]
    public void SetUp ()
    {
      _policy = DefaultMixinInheritancePolicy.Instance;
    }

    [Test]
    public void GetTypesToInheritFrom_None ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (object)).ToArray (), Is.Empty);
    }

    [Test]
    public void GetTypesToInheritFrom_Base ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (string)).ToArray (), Has.Member(typeof (object)));
    }

    [Test]
    public void GetTypesToInheritFrom_Interfaces ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (string)).ToArray (), 
          Has.Member(typeof (IEnumerable<char>)));
    }

    [Test]
    public void GetTypesToInheritFrom_GenericTypeDef ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (List<int>)).ToArray (), Has.Member(typeof (List<>)));
    }

    [Test]
    public void GetTypesToInheritFrom_NoGenericTypeDef_ForOpenGenericType ()
    {
      Assert.That (_policy.GetTypesToInheritFrom (typeof (List<>)).ToArray (), Has.No.Member(typeof (List<>)));
    }

    [Test]
    public void GetClassContextsToInheritFrom ()
    {
      var fakeClassContext = ClassContextObjectMother.Create(typeof (object));
      var result = _policy.GetClassContextsToInheritFrom (typeof (BaseType1), t => fakeClassContext);

      Assert.That (result.ToArray (), Is.EqualTo (new[] { fakeClassContext }));
    }
  }
}
