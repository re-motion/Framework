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
using System.Web.UI.Design;
using Remotion.Design;
using Remotion.Utilities;

namespace Remotion.Web.UI.Design
{
  /// <summary>
  /// Implementation of the <see cref="IDesignModeHelper"/> interface for environments implementing the <see cref="IWebApplication"/> designer service.
  /// </summary>
  public class WebDesginModeHelper: DesignModeHelperBase
  {
    public WebDesginModeHelper (IDesignerHost designerHost)
        : base (designerHost)
    {
    }

    public override string GetProjectPath()
    {
      return GetWebApplication().RootProjectItem.PhysicalPath;
    }

    public override System.Configuration.Configuration GetConfiguration()
    {
      return GetWebApplication().OpenWebConfiguration (true);
    }

    private IWebApplication GetWebApplication()
    {
      IWebApplication webApplication = (IWebApplication) DesignerHost.GetService (typeof (IWebApplication));
      Assertion.IsNotNull(webApplication, "The 'IServiceProvider' failed to return an 'IWebApplication' service.");

      return webApplication;
    }
  }
}
