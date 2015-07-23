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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderInheritanceTest
  {
    [Uses (typeof (NullMixin))]
    public class Base { }

    public class Derived : Base { }

    [Uses (typeof (NullMixin2))]
    public class DerivedWithOwnMixin : Base { }

    public class DerivedDerived : Derived { }

    [Uses (typeof (NullMixin2))]
    public class DerivedDerivedWithOwnMixin : Derived { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedWithOverride : Base { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedDerivedWithOwnMixinAndOverride : DerivedDerivedWithOwnMixin { }

    [Test]
    public void BaseContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly());
      ClassContext classContext = configuration.GetContext (typeof (Base));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void DerivedContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (Derived));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void DerivedContextWithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedWithOwnMixin));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerived));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order1 ()
    {
      var builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (Base));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));

      MixinConfiguration configuration = builder.BuildConfiguration();
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order2 ()
    {
      var builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (Base));

      MixinConfiguration configuration = builder.BuildConfiguration ();
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }


    [Test]
    public void DerivedContextWithOverride ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedWithOverride));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void DerivedDerivedContextWithOverrideAndOverride ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedWithOwnMixinAndOverride));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }
  }
}
