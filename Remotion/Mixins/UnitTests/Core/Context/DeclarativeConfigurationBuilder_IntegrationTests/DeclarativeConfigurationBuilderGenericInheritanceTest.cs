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

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGenericInheritanceTest
  {
// ReSharper disable UnusedTypeParameter
    public class GenericClass<T> { }
    public class DerivedGenericClassFromOpen<T> : GenericClass<T> { }
    public class DerivedGenericClassFromClosed<T> : GenericClass<int> { }
    public class DerivedClassFromClosed : GenericClass<int> { }
    public class DerivedDerivedGenericClassFromOpen<T> : DerivedGenericClassFromOpen<T> { }
// ReSharper restore UnusedTypeParameter

    [Extends (typeof (GenericClass<>))]
    public class MixinForOpenGeneric { }

    [Extends (typeof (GenericClass<int>))]
    public class MixinForClosedGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<>))]
    public class MixinForDerivedOpenGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedClosedGeneric { }

    [Extends (typeof (DerivedDerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedDerivedClosedGeneric { }

    [Test]
    public void OpenGenericClassContext_Open()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (GenericClass<>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void ClosedGenericClassContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (GenericClass<string>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void ClosedGenericClassContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (GenericClass<int>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedGenericClassFromOpen<>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedGenericClassFromOpen<string>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedGenericClassFromOpen<int>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (4));
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Open ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedGenericClassFromClosed<>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Closed_NoOwnMixins ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedGenericClassFromClosed<int>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedGenericClassFromOpen<>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedGenericClassFromOpen<string>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedDerivedGenericClassFromOpen<int>));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (5));
    }

    [Test]
    public void DerivedClassFromClosedContext ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = configuration.GetContext (typeof (DerivedClassFromClosed));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)), Is.True);
      Assert.That (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)), Is.True);
      Assert.That (classContext.Mixins.Count, Is.EqualTo (2));
    }
  }
}
