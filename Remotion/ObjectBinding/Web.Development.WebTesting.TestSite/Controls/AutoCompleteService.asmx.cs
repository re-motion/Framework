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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  [WebService (Namespace = "http://re-motion.org/ObjectBinding.Web/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class AutoCompleteService : WebService, ISearchAvailableObjectWebService
  {
    [WebMethod]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy[] Search (
        string searchString,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string args)
    {
      if (searchString == "throw")
        throw new InvalidOperationException ("I'm always going to throw an exception if you search for 'throw'!");

      var persons = new List<BusinessObjectWithIdentityProxy>();
      foreach (var person in XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)))
      {
        persons.Add (
            new BusinessObjectWithIdentityProxy ((IBusinessObjectWithIdentity) person) { IconUrl = GetUrl (GetIcon ((IBusinessObject) person)) });
      }

      var filteredPersons = persons.FindAll (person => person.DisplayName.StartsWith (searchString, StringComparison.OrdinalIgnoreCase));
      if (filteredPersons.Count == 0)
        filteredPersons = persons.FindAll (person => person.DisplayName.IndexOf (searchString, StringComparison.OrdinalIgnoreCase) != -1);

      filteredPersons.Sort ((left, right) => string.Compare (left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase));

      return filteredPersons.Take(completionSetCount ?? int.MaxValue).ToArray();
    }

    private IResourceUrlFactory ResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(); }
    }

    [WebMethod]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy SearchExact (
        string searchString,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string args)
    {
      if (searchString == "throw")
        throw new InvalidOperationException ("I'm always going to throw an exception if you search for 'throw'!");

      var result = Search (searchString, 2, businessObjectClass, businessObjectProperty, businessObject, args);
      if (result.Length == 0)
        return null;
      if (!string.Equals (result[0].DisplayName, searchString, StringComparison.CurrentCultureIgnoreCase))
        return null;
      return result[0];
    }

    private string GetUrl (IconInfo iconInfo)
    {
      return UrlUtility.GetAbsoluteUrl (new HttpContextWrapper (Context), iconInfo.Url);
    }

    private IconInfo GetIcon (IBusinessObject businessObject)
    {
      return BusinessObjectBoundWebControl.GetIcon (businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);
    }
  }
}