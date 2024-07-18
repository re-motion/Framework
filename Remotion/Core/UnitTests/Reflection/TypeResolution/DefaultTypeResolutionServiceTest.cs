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
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.TypeResolution;

namespace Remotion.UnitTests.Reflection.TypeResolution
{
  [TestFixture]
  public class DefaultTypeResolutionServiceTest
  {
    [Test]
    public void GetType_WithValidTypeName_ReturnsType ()
    {
      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("System.String"), Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetType_WithInvalidTypeName_ReturnsNull ()
    {
      Assert.That(Type.GetType("Foo, System"), Is.Null);

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("Foo, System"), Is.Null);
    }


    [Test]
    public void GetType_WithThrowOnErrorTrue_WithValidTypeName_ReturnsType ()
    {
      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("System.String", throwOnError: true), Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetType_WithThrowOnErrorTrue_WithInvalidTypeName_ThrowsTypeLoadException ()
    {
      Assert.That(() => Type.GetType("Foo, Remotion.UnitTests", throwOnError: true), Throws.TypeOf<TypeLoadException>());

      var service = CreateTypeResolutionService();
      Assert.That(() => service.GetType("Foo, Remotion.UnitTests", throwOnError: true), Throws.TypeOf<TypeLoadException>());
    }

    [Test]
    public void GetType_WithThrowOnErrorFalse_WithInvalidTypeName_ReturnsNull ()
    {
      Assert.That(Type.GetType("Foo, Remotion.UnitTests", throwOnError: false), Is.Null);

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("Foo, Remotion.UnitTests", throwOnError: false), Is.Null);
    }


    [Test]
    public void GetType_WithThrowOnErrorTrue_AndWithIgnoreCaseFalse_WithValidTypeName_ReturnsType ()
    {
      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("System.String", throwOnError: true, ignoreCase: false), Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetType_WithThrowOnErrorTrue_AndWithIgnoreCaseFalse_WithInvalidTypeName_ThrowsTypeLoadException ()
    {
      Assert.That(() => Type.GetType("Foo, Remotion.UnitTests", throwOnError: true, ignoreCase: false), Throws.TypeOf<TypeLoadException>());

      var service = CreateTypeResolutionService();
      Assert.That(() => service.GetType("Foo, Remotion.UnitTests", throwOnError: true, ignoreCase: false), Throws.TypeOf<TypeLoadException>());
    }

    [Test]
    public void GetType_WithThrowOnErrorTrue_AndWithIgnoreCaseTrue_WithTypeNameHavingCaseMismatch_ThrowsTypeLoadException ()
    {
      Assert.That(() => Type.GetType("system.string", throwOnError: true, ignoreCase: false), Throws.TypeOf<TypeLoadException>());

      var service = CreateTypeResolutionService();
      Assert.That(() => service.GetType("system.string", throwOnError: true, ignoreCase: false), Throws.TypeOf<TypeLoadException>());
    }

    [Test]
    public void GetType_WithThrowOnErrorTrue_AndWithIgnoreCaseTrue_WithTypeNameHavingCaseMismatch_ReturnsType ()
    {
      Assert.That(() => Type.GetType("system.string", throwOnError: true, ignoreCase: true), Is.SameAs(typeof(string)));

      var service = CreateTypeResolutionService();
      Assert.That(() => service.GetType("system.string", throwOnError: true, ignoreCase: true), Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetType_WithThrowOnErrorFalse_AndWithIgnoreCaseTrue_WithInvalidTypeName_ReturnsNull ()
    {
      Assert.That(Type.GetType("Foo, Remotion.UnitTests", throwOnError: false, ignoreCase: true), Is.Null);

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("Foo, Remotion.UnitTests", throwOnError: false, ignoreCase: true), Is.Null);
    }

    [Test]
    public void GetType_WithThrowOnErrorFalse_AndWithIgnoreCaseTrue_WithTypeNameHavingCaseMismatch_ReturnsType ()
    {
      Assert.That(Type.GetType("system.string", throwOnError: false, ignoreCase: true), Is.SameAs(typeof(string)));

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("system.string", throwOnError: false, ignoreCase: true), Is.SameAs(typeof(string)));
    }

    [Test]
    public void GetType_WithThrowOnErrorFalse_AndWithIgnoreCaseFalse_WithInvalidTypeName_ReturnsNull ()
    {
      Assert.That(Type.GetType("Foo, Remotion.UnitTests", throwOnError: false, ignoreCase: false), Is.Null);

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("Foo, Remotion.UnitTests", throwOnError: false, ignoreCase: false), Is.Null);
    }

    [Test]
    public void GetType_WithThrowOnErrorFalse_AndWithIgnoreCaseFalse_WithTypeNameHavingCaseMismatch_ReturnsNull ()
    {
      Assert.That(Type.GetType("system.string", throwOnError: false, ignoreCase: false), Is.Null);

      var service = CreateTypeResolutionService();
      Assert.That(service.GetType("system.string", throwOnError: false, ignoreCase: false), Is.Null);
    }

    [Test]
    public void GetAssembly_WithValidAssemblyName_ReturnsAssembly ()
    {
      var service = CreateTypeResolutionService();

      var assembly = typeof(string).Assembly;
      Assert.That(service.GetAssembly(assembly.GetName()), Is.SameAs(assembly));
    }

    [Test]
    public void GetAssembly_WithInvalidAssemblyName_ReturnsNull ()
    {
      var service = CreateTypeResolutionService();

      Assert.That(service.GetAssembly(new AssemblyName("Foo")), Is.Null);
    }

    [Test]
    public void GetAssembly_WithThrowOnErrorTrue_WithValidAssemblyName_ReturnsAssembly ()
    {
      var service = CreateTypeResolutionService();

      var assembly = typeof(string).Assembly;
      Assert.That(service.GetAssembly(assembly.GetName(), throwOnError: true), Is.SameAs(assembly));
    }

    [Test]
    public void GetAssembly_WithThrowOnErrorTrue_WithInvalidAssemblyName_ThrowsFileNotFoundException ()
    {
      var service = CreateTypeResolutionService();

      Assert.That(() => service.GetAssembly(new AssemblyName("Foo"), throwOnError: true), Throws.TypeOf<FileNotFoundException>());
    }

    [Test]
    public void GetAssembly_WithThrowOnErrorFalse_WithValidAssemblyName_ReturnsAssembly ()
    {
      var service = CreateTypeResolutionService();

      var assembly = typeof(string).Assembly;
      Assert.That(service.GetAssembly(assembly.GetName(), throwOnError: false), Is.SameAs(assembly));
    }

    [Test]
    public void GetAssembly_WithThrowOnErrorFalse_WithInvalidAssemblyName_ReturnsNull ()
    {
      var service = CreateTypeResolutionService();

      Assert.That(service.GetAssembly(new AssemblyName("Foo"), throwOnError: false), Is.Null);
    }

    [Test]
    public void GetPathOfAssembly_WithValidAssemblyName_ReturnsLocationBeforeShadowCopy ()
    {
      var service = CreateTypeResolutionService();

      var assembly = typeof(string).Assembly;

      // Shadow Copying is only supported in .NET Framework.
      // Given that we are using mscorlib.dll, we may assume the assembly is never shadow copied and thus Assembly.Location and Assembly.CodeBase are equal

      Assert.That(assembly.Location, Is.Not.Null);
      Assert.That(service.GetPathOfAssembly(assembly.GetName()), Is.EqualTo(assembly.Location));
    }

    [Test]
    public void GetPathOfAssembly_WithInvalidAssemblyName_ReturnsNull ()
    {
      var service = CreateTypeResolutionService();

      Assert.That(service.GetPathOfAssembly(new AssemblyName("Foo")), Is.Null);
    }

    private ITypeResolutionService CreateTypeResolutionService ()
    {
      return new DefaultTypeResolutionService();
    }
  }
}
