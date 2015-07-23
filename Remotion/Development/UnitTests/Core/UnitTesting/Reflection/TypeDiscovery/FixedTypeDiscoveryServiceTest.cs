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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Reflection.TypeDiscovery
{
  [TestFixture]
  public class FixedTypeDiscoveryServiceTest
  {
    [Test]
    public void GetTypes ()
    {
      var service = new FixedTypeDiscoveryService (new[] { typeof (FixedTypeDiscoveryServiceTest) });

      Assert.That (service.GetTypes (null, false), Is.EqualTo (new[] { typeof (FixedTypeDiscoveryServiceTest) }));
    }

    [Test]
    public void GetTypes_BaseTypeFiltering ()
    {
      var service = new FixedTypeDiscoveryService (new[] { typeof (FixedTypeDiscoveryServiceTest), typeof (FixedTypeDiscoveryService) });

      Assert.That (service.GetTypes (typeof (ITypeDiscoveryService), false), Is.EqualTo (new[] { typeof (FixedTypeDiscoveryService) }));
    }

    [Test]
    public void GetTypes_ExcludeGlobalTypes_True ()
    {
      var service = new FixedTypeDiscoveryService (new[] { typeof (FixedTypeDiscoveryServiceTest), typeof (object) });

      Assert.That (service.GetTypes (null, true), Is.EqualTo (new[] { typeof (FixedTypeDiscoveryServiceTest) }));
    }

    [Test]
    public void GetTypes_ExcludeGlobalTypes_False ()
    {
      var service = new FixedTypeDiscoveryService (new[] { typeof (FixedTypeDiscoveryServiceTest), typeof (object) });

      Assert.That (service.GetTypes (null, false), Is.EqualTo (new[] { typeof (FixedTypeDiscoveryServiceTest), typeof (object) }));
    }
  }
}