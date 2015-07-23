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
using System.Linq;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests
{
  [TestFixture]
  public class BocLegacyServiceConfigurationServiceTest
  {
    [Test]
    public void GetConfiguration ()
    {
      var discoveryService = DefaultServiceConfigurationDiscoveryService.Create();
      var allServiceTypes = discoveryService.GetDefaultConfiguration (new[] { typeof (IBocList).Assembly })
          .Select (e => e.ServiceType).ToList();
      var nonLegacyServices = new[] { typeof (BocListCssClassDefinition), typeof (IDateTimeFormatter) };
      var expectedLegacyServiceTypes = allServiceTypes
          .Except (nonLegacyServices)
          .Concat (typeof (BocListQuirksModeCssClassDefinition));

      Assert.That (
          BocLegacyServiceConfigurationService.GetConfiguration().Select (e => e.ServiceType),
          Is.EquivalentTo (expectedLegacyServiceTypes));

      Assert.That (
          BocLegacyServiceConfigurationService.GetConfiguration()
              .Where (e => !nonLegacyServices.Contains (e.ServiceType))
              .Select (e => GetSingleServiceImplementationInfo(e).ImplementationType.Assembly)
              .ToArray(),
          Is.All.EqualTo (typeof (BocLegacyServiceConfigurationService).Assembly));
    }

    [Test]
    public void RegisterLegacyTypesToNewDefaultServiceLocator ()
    {
      var legacyServiceTypes = BocLegacyServiceConfigurationService.GetConfiguration();

      var locator = DefaultServiceLocator.Create();
      foreach (var serviceConfigurationEntry in legacyServiceTypes)
        locator.Register (serviceConfigurationEntry);

      foreach (var legacyServiceType in legacyServiceTypes)
        Assert.That (locator.GetInstance (legacyServiceType.ServiceType), Is.TypeOf (GetSingleServiceImplementationInfo (legacyServiceType).ImplementationType));
    }

    private ServiceImplementationInfo GetSingleServiceImplementationInfo (ServiceConfigurationEntry e)
    {
      Assert.That (e.ImplementationInfos, Has.Count.EqualTo (1));
      return e.ImplementationInfos.Single ();
    }
  }
}