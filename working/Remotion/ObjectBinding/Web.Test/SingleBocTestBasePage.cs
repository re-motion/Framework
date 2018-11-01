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
using System.Globalization;
using System.Threading;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class SingleBocTestBasePage:
    SmartPage, 
    IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    if (! ControlHelper.IsDesignMode (this))
    {
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
    }
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    HtmlHeadAppender.Current.RegisterPageStylesheetLink ();

    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));
  }

  public virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    return GlobalizationService.GetResourceManager (type);
  }

  protected IGlobalizationService GlobalizationService
  {
    get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
  }

//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    if (ControlHelper.IsDesignMode (this, this.Context))
//      return resourceType.Name + "/" + relativeUrl;
//    else
//      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
//  }
}

}
