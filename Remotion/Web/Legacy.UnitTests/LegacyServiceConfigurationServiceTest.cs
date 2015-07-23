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
using Remotion.ServiceLocation;
using Remotion.Web.Compilation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.Resources;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UnitTests
{
  [TestFixture]
  public class LegacyServiceConfigurationServiceTest
  {
    [Test]
    public void GetConfiguration ()
    {
      var nonLegacyServices = new[]
                              {
                                  typeof (IWebTabRenderer),
                                  typeof (IScriptUtility), 
                                  typeof (IMenuTabRenderer), 
                                  typeof (ResourceTheme),
                                  typeof (IControlBuilderCodeProcessor),
                                  typeof (IHotkeyFormatter),
                                  typeof (IResourcePathBuilder),
                                  typeof (IHttpContextProvider),
                                  typeof (IInternalControlMemberCaller),
                                  typeof (IBuildManager),
                                  typeof (IRenderingFeatures),
                                  typeof (IWebSecurityAdapter),
                                  typeof (IWxeSecurityAdapter),
                              };

      var discoverySerivce = DefaultServiceConfigurationDiscoveryService.Create();
      var allServiceTypes = discoverySerivce.GetDefaultConfiguration (new[] { typeof (IResourceUrl).Assembly })
          .Select (e => e.ServiceType);
      var expectedLegacyServiceTypes = allServiceTypes.Except (nonLegacyServices);

      Assert.That (
          LegacyServiceConfigurationService.GetConfiguration().Select (e => e.ServiceType), 
          Is.EquivalentTo (expectedLegacyServiceTypes),
          "New service was added in Remotion.Web. Either the Service must also be added to Remotion.Web.Legacy or added to the exclude list in 'nonLegacyServices'.");

      Assert.That (
          LegacyServiceConfigurationService.GetConfiguration()
              .Where (e => !nonLegacyServices.Contains (e.ServiceType))
              .Select (e => GetSingleServiceImplementationInfo(e).ImplementationType.Assembly)
              .ToArray(),
          Is.All.EqualTo (typeof (LegacyServiceConfigurationService).Assembly));
    }

    [Test]
    public void RegisterLegacyTypesToNewDefaultServiceLocator ()
    {
      var legacyServiceTypes = LegacyServiceConfigurationService.GetConfiguration();

      var locator = DefaultServiceLocator.Create();
      foreach (var serviceConfigurationEntry in legacyServiceTypes)
        locator.Register (serviceConfigurationEntry);

      foreach (var legacyServiceType in legacyServiceTypes)
      {
        Assert.That (
            locator.GetInstance (legacyServiceType.ServiceType),
            Is.TypeOf (GetSingleServiceImplementationInfo (legacyServiceType).ImplementationType));
      }
    }

    private ServiceImplementationInfo GetSingleServiceImplementationInfo (ServiceConfigurationEntry e)
    {
      Assert.That (e.ImplementationInfos, Has.Count.EqualTo (1));
      return e.ImplementationInfos.Single ();
    }
  }
}