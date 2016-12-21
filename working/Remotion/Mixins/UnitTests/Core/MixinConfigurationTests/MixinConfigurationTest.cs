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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationTest
  {
    private MixinConfiguration _oldMasterConfiguration;
    private MixinConfiguration _oldActiveConfiguration;

    [SetUp]
    public void SetUp ()
    {
      _oldMasterConfiguration = MixinConfiguration.GetMasterConfiguration ();
      _oldActiveConfiguration = MixinConfiguration.ActiveConfiguration;
    }

    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.SetMasterConfiguration (_oldMasterConfiguration);
      MixinConfiguration.SetActiveConfiguration (_oldActiveConfiguration);
    }

    [Test]
    public void Initialization_Empty ()
    {
      var configuration = new MixinConfiguration();
      Assert.That (configuration.ClassContexts, Is.Empty);
    }

    [Test]
    public void GetContext_Configured ()
    {
      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));
      Assert.That (context, Is.Not.Null);
    }

    [Test]
    public void GetContext_Configured_ButEmpty ()
    {
      var configuration = MixinConfiguration.BuildNew ().ForClass<NullTarget> ().BuildConfiguration ();
      Assert.That (configuration.ClassContexts.ContainsExact (typeof (NullTarget)), Is.True);

      var context = configuration.GetContext (typeof (NullTarget));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_ReturnsNull_IfNotConfigured ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);

      var context = MixinConfiguration.ActiveConfiguration.GetContext (typeof (object));
      Assert.That (context, Is.Null);
    }

    [Test]
    public void GetContext_NoNewContext_GeneratedForGeneratedType ()
    {
      var expectedContext = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));

      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      var actualContext = MixinConfiguration.ActiveConfiguration.GetContext (generatedType);
      Assert.That (actualContext, Is.EqualTo (expectedContext));
    }

    [Test]
    public void SetMasterConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration ();
      MixinConfiguration.SetMasterConfiguration (mixinConfiguration);
      Assert.That (MixinConfiguration.GetMasterConfiguration (), Is.SameAs (mixinConfiguration));
    }

    [Test]
    public void GetMasterConfiguration_Default ()
    {
      var oldMasterConfiguration = MixinConfiguration.GetMasterConfiguration();
      
      MixinConfiguration.SetMasterConfiguration (null);
      var newMasterConfiguration = MixinConfiguration.GetMasterConfiguration ();

      Assert.That (newMasterConfiguration, Is.Not.Null);
      Assert.That (newMasterConfiguration, Is.Not.SameAs (oldMasterConfiguration));
      Assert.That (newMasterConfiguration.GetContext (typeof (BaseType1)), Is.Not.Null);
    }
  }
}
