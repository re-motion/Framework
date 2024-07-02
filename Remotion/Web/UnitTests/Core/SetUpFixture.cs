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
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.Infrastructure;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Development.Web.UnitTesting.Utilities;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.Core.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private ServiceLocatorScope _serviceLocatorScope;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      XmlNodeExtensions.Helper = new HtmlHelper();

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle<IInfrastructureResourceUrlFactory>(() => new FakeInfrastructureResourceUrlFactory());
      serviceLocator.RegisterSingle<IScriptUtility>(() => new FakeScriptUtility());
      serviceLocator.RegisterSingle<IResourceUrlFactory>(() => new FakeResourceUrlFactory());
      serviceLocator.RegisterSingle<IFallbackNavigationUrlProvider>(() => new FakeFallbackNavigationUrlProvider());
      serviceLocator.RegisterMultiple<IWebSecurityAdapter>();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();

      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown ()
    {
      _serviceLocatorScope.Dispose();
    }
  }
}
