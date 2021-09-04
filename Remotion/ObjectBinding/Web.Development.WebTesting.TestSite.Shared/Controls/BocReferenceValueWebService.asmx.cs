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
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Web.Services;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls
{
  [WebService (Namespace = "http://re-motion.org/ObjectBinding.Web/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class BocReferenceValueWebService : WebService, IBocReferenceValueWebService
  {
    private readonly IconServiceImplementation _iconServiceImplementation;

    public BocReferenceValueWebService ()
    {
      _iconServiceImplementation = new IconServiceImplementation();
    }

    [WebMethod]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
    public IconProxy GetIcon (string businessObjectClass, string businessObject, string arguments)
    {
      return _iconServiceImplementation.GetIcon (new HttpContextWrapper (Context), businessObjectClass, businessObject, arguments);
    }

    [WebMethod]
    [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
    public WebMenuItemProxy[] GetMenuItemStatusForOptionsMenu (
        string controlID,
        string controlType,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string arguments,
        string[] itemIDs)
    {
      return itemIDs.Select (itemID => WebMenuItemProxy.Create (itemID, isDisabled: false)).ToArray();
    }
  }
}