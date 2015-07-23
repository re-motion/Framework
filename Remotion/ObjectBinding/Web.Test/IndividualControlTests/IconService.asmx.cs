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
using System.ComponentModel;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Services;

namespace OBWTest.IndividualControlTests
{
  [WebService (Namespace = "http://re-motion.org/ObjectBinding.Web/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class IconService : WebService, IBusinessObjectIconWebService
  {
    [WebMethod]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
    public IconProxy GetIcon (string businessObjectClass, string businessObject, string arguments)
    {
      if (businessObjectClass == null)
        return null;

      Type type = TypeUtility.GetType (businessObjectClass, true);
      var businessObjectProvider = BindableObjectProvider.GetProviderForBindableObjectType (type);
      var bindableObjectClass = businessObjectProvider.GetBindableObjectClass (type);
      IBusinessObjectWithIdentity businessObjectWithIdentity = null;
      if (!string.IsNullOrEmpty (businessObject))
      {
        var businessObjectClassWithIdentity = (IBusinessObjectClassWithIdentity) bindableObjectClass;
        businessObjectWithIdentity = businessObjectClassWithIdentity.GetObject (businessObject);
      }

      var iconInfo = BusinessObjectBoundWebControl.GetIcon (businessObjectWithIdentity, bindableObjectClass.BusinessObjectProvider);
      if (iconInfo != null)
        return IconProxy.Create (new HttpContextWrapper (Context), iconInfo);

      return null;
    }
  }
}