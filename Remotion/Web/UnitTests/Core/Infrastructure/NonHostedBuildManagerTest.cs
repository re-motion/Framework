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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UnitTests.Core.Infrastructure
{
  [TestFixture]
  public class NonHostedBuildManagerTest
  {
    private IBuildManager _buildManager;

    [SetUp]
    public void SetUp ()
    {
      _buildManager = new NonHostedBuildManager();
    }

    [Test]
    public void GetType_WithValidTypeName_ReturnsType ()
    {
      Assert.That(
          _buildManager.GetType("Remotion.Web.Infrastructure.IBuildManager, Remotion.Web", true, false),
          Is.EqualTo(typeof(IBuildManager)));
    }

    [Test]
    public void GetType_WithValidTypeNameIgnoringCase_ReturnsType ()
    {
      Assert.That(
          _buildManager.GetType("Remotion.Web.Infrastructure.ibuildmanager, Remotion.Web", true, ignoreCase: true),
          Is.EqualTo(typeof(IBuildManager)));
    }

    [Test]
    public void GetType_WithInvalidTypeNameAndThrowOnErrorIsTrue_ThrowsException ()
    {
      Assert.That(
          () => _buildManager.GetType("Remotion.Web.Infrastructure.Invalid, Remotion.Web", throwOnError: true, ignoreCase: false),
          Throws.TypeOf<TypeLoadException>()
#if NET8_0_OR_GREATER
              .And.Message.StartsWith("Could not load type 'Remotion.Web.Infrastructure.Invalid' from assembly 'Remotion.Web, Version="));
#else
              .And.Message.EqualTo("Could not load type 'Remotion.Web.Infrastructure.Invalid' from assembly 'Remotion.Web'."));
#endif
    }

    [Test]
    public void GetType_WithInvalidTypeNameAndThrowOnErrorIsFalse_ReturnsNull ()
    {
      Assert.That(
          _buildManager.GetType("Remotion.Web.Infrastructure.Invalid, Remotion.Web", throwOnError: false, ignoreCase: false),
          Is.Null);
    }

    [Test]
    public void GetCodeAssemblies_ReturnsNull ()
    {
      Assert.That(_buildManager.CodeAssemblies, Is.Null);
    }

    [Test]
    public void GetCompiledType_ReturnsNull ()
    {
      Assert.That(_buildManager.GetCompiledType("SomePath"), Is.Null);
    }
  }
}
