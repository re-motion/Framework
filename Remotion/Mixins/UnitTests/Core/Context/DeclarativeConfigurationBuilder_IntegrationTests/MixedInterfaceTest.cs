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
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixedInterfaceTest
  {
    [Test]
    public void MixedInterface_GetsClassContext_ViaUses ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (IMixedInterface));
      Assert.That (context.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
    }

    [Test]
    public void MixedInterface_GetsClassContext_ViaExtends ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (IMixedInterface));
      Assert.That (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)), Is.True);
    }

    [Test]
    public void ImplementingClass_InheritsMixins ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassWithMixedInterface));
      Assert.That (context.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)), Is.True);
    }
  }
}
