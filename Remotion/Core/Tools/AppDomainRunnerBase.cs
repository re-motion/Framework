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
using System.IO;
using Remotion.Utilities;

namespace Remotion.Tools
{
  /// <summary>
  /// Base class for executing code in a separate <see cref="AppDomain"/>.
  /// </summary>
 #if NETFRAMEWORK
  [Serializable]
#endif
  public abstract class AppDomainRunnerBase
  {
    private readonly AppDomainSetup _appDomainSetup;

    protected AppDomainRunnerBase (AppDomainSetup appDomainSetup)
    {
      ArgumentUtility.CheckNotNull("appDomainSetup", appDomainSetup);

      _appDomainSetup = appDomainSetup;
    }

    protected abstract void CrossAppDomainCallbackHandler ();

    public AppDomainSetup AppDomainSetup
    {
      get { return _appDomainSetup; }
    }

    public void Run ()
    {
#if NETFRAMEWORK
      AppDomain? appDomain = null;

      try
      {
        // TODO RM-7785: ApplicationName & ApplicationBase should be checked for null.
        appDomain = AppDomain.CreateDomain(_appDomainSetup.ApplicationName!, AppDomain.CurrentDomain.Evidence, _appDomainSetup);
        AppDomainSetup parentAppDomainSetup = AppDomain.CurrentDomain.SetupInformation;

        var resolverInAppDomain = AppDomainAssemblyResolver.CreateInAppDomain(appDomain, parentAppDomainSetup.ApplicationBase!);
        resolverInAppDomain.Register(appDomain);

        appDomain.DoCallBack(CrossAppDomainCallbackHandler);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload(appDomain);

        if (Directory.Exists(_appDomainSetup.DynamicBase))
          Directory.Delete(_appDomainSetup.DynamicBase!, true);
      }
#else
      throw new PlatformNotSupportedException("This API is not supported on the current platform.");
#endif
    }
  }
}
