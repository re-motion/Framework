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
  public class ComposedInterfaceAnalyisTest
  {
    [Test]
    public void ComposedInterface_ViaAttribute ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassWithComposedInterface))
          .AddType (typeof (ClassWithComposedInterface.IComposedInterface))
          .BuildConfiguration ();

      var classContext = result.GetContext (typeof (ClassWithComposedInterface));
      Assert.That (classContext.ComposedInterfaces, Has.Member (typeof (ClassWithComposedInterface.IComposedInterface)));
    }

    [Test]
    [Ignore ("TODO 3536")]
    public void ComposedInterface_ViaAttribute_Derived ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassWithComposedInterface))
          .AddType (typeof (ClassWithComposedInterface.IComposedInterface))
          .AddType (typeof (DerivedClassWithComposedInterface))
          .BuildConfiguration ();

      var baseClassContext = result.GetContext (typeof (ClassWithComposedInterface));
      Assert.That (baseClassContext.ComposedInterfaces, Has.Member (typeof (ClassWithComposedInterface.IComposedInterface)));

      var derivedClassContext = result.GetContext (typeof (DerivedClassWithComposedInterface));
      Assert.That (derivedClassContext.ComposedInterfaces, Has.Member (typeof (ClassWithComposedInterface.IComposedInterface)));
    }

    [Test]
    public void ComposedInterface_ViaIHasComposedInterface ()
    {
      var result = new DeclarativeConfigurationBuilder (null).AddType (typeof (ClassWithHasComposedInterfaces)).BuildConfiguration ();

      var classContext = result.GetContext (typeof (ClassWithHasComposedInterfaces));
      Assert.That (classContext.ComposedInterfaces, Has.Member (typeof (ClassWithHasComposedInterfaces.IComposedInterface1)));
      Assert.That (classContext.ComposedInterfaces, Has.Member (typeof (ClassWithHasComposedInterfaces.IComposedInterface2)));
    }

    [Test]
    public void ComposedInterface_ViaIHasComposedInterface_ViaGenericBaseClass ()
    {
      var result = new DeclarativeConfigurationBuilder (null).AddType (typeof (ClassDerivedFromBaseClassWithHasComleteInterface)).BuildConfiguration ();

      var classContext = result.GetContext (typeof (ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (classContext.ComposedInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)));
    }

    [Test]
    public void ComposedInterface_ViaIHasComposedInterface_Derived ()
    {
      var result = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (ClassDerivedFromBaseClassWithHasComleteInterface))
          .AddType (typeof (DerivedClassDerivedFromBaseClassWithHasComleteInterface)).BuildConfiguration ();

      var baseClassContext = result.GetContext (typeof (ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (baseClassContext.ComposedInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)));

      var derivedClassContext = result.GetContext (typeof (DerivedClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That (derivedClassContext.ComposedInterfaces, Has.Member (typeof (ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)));
    }
  }
}