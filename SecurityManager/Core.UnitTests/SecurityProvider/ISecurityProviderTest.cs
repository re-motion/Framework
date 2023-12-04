// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Security;
using Remotion.SecurityManager.SecurityProvider;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.SecurityProvider
{
  [TestFixture]
  public class ISecurityProviderTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var obj = _serviceLocator.GetInstance<ISecurityProvider>();

      Assert.That(obj, Is.Not.Null);
      Assert.That(obj, Is.TypeOf<RevisionBasedCachingSecurityProviderDecorator>());
      Assert.That(((RevisionBasedCachingSecurityProviderDecorator)obj).InnerSecurityProvider, Is.TypeOf<SecurityService>());
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var obj1 = _serviceLocator.GetInstance<ISecurityProvider>();
      var obj2 = _serviceLocator.GetInstance<ISecurityProvider>();

      Assert.That(obj1, Is.SameAs(obj2));
    }
  }
}
