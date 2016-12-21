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
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace OBWTest.IndividualControlTests
{
  [WebService (Namespace = "http://re-motion.org/ObjectBinding.Web/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class AutoCompleteService : WebService, ISearchAvailableObjectWebService
  {
    #region Values

    private static readonly string[] s_values = new []
                                       {
                                           "aaa<b><i>iii</i></b>a'a\"a",
                                           "sdfg",
                                           "sdfgh",
                                           "sdfghj",
                                           "sdfghjk",
                                           "sdfghjkl",
                                           "sdfg 0qqqqwwww",
                                           "sdfg 1qqqqwwww",
                                           "sdfg 2qqqqwwww",
                                           "sdfg 3qqqqwwww",
                                           "sdfg 4qqqqwwww",
                                           "sdfg 5qqqqwwww",
                                           "sdfg 7qqqqwwww",
                                           "sdfg 8qqqqwwww",
                                           "sdfg 9qqqqwwww",
                                           "sdfg q",
                                           "sdfg qq",
                                           "sdfg qqq",
                                           "sdfg qqqq",
                                           "sdfg qqqqq",
                                           "sdfg qqqqqq",
                                           "sdfg qqqqqqq",
                                           "sdfg qqqqqqqq",
                                           "sdfg qqqqqqqqq",
                                           "sdfg qqqqqqqqqq",
                                           "sdfg qqqqqqqqqqq",
                                           "access control list (ACL)",
                                           "ADO.NET",
                                           "aggregate event",
                                           "alpha channel",
                                           "anchoring",
                                           "antialiasing",
                                           "application base",
                                           "application domain (AppDomain)",
                                           "application manifest",
                                           "application state",
                                           "ASP.NET",
                                           "ASP.NET application services database",
                                           "ASP.NET mobile controls",
                                           "ASP.NET mobile Web Forms",
                                           "ASP.NET page",
                                           "ASP.NET server control",
                                           "ASP.NET Web application",
                                           "assembly",
                                           "assembly cache",
                                           "assembly manifest",
                                           "assembly metadata",
                                           "assertion (Assert)",
                                           "association class",
                                           "ASSOCIATORS OF",
                                           "asynchronous method",
                                           "attribute",
                                           "authentication",
                                           "authorization",
                                           "autopostback",
                                           "bounds",
                                           "boxing",
                                           "C#",
                                           "card",
                                           "catalog",
                                           "CCW",
                                           "chevron",
                                           "chrome",
                                           "cHTML",
                                           "CIM",
                                           "CIM Object Manager",
                                           "CIM schema",
                                           "class",
                                           "client area",
                                           "client coordinates",
                                           "clip",
                                           "closed generic type",
                                           "CLR",
                                           "CLS",
                                           "CLS-compliant",
                                           "code access security",
                                           "code-behind class",
                                           "code-behind file",
                                           "code-behind page",
                                           "COM callable wrapper (CCW)",
                                           "COM interop",
                                           "Common Information Model (CIM)",
                                           "common language runtime",
                                           "common language runtime host",
                                           "Common Language Specification (CLS)",
                                           "common object file format (COFF)",
                                           "common type system (CTS)",
                                           "comparison evaluator",
                                           "composite control",
                                           "configuration file",
                                           "connection",
                                           "connection point",
                                           "constraint",
                                           "constructed generic type",
                                           "constructed type",
                                           "consumer",
                                           "container",
                                           "container control",
                                           "content page",
                                           "context",
                                           "context property",
                                           "contract",
                                           "control state",
                                           "cross-page posting",
                                           "CTS",
                                           "custom attribute (Attribute)",
                                           "custom control",
                                           "throw",
                                           "exactthrow",
                                           "very long text so there is a scrollbar",
                                           "very long text so there is a scrollbar, take 2",
                                           "very long text so there is a scrollbar, take 3",
                                           "very long text so there is a scrollbar, because there was not enough yet",
                                       };

    #endregion

    [WebMethod (EnableSession = true)]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy[] Search (
        string searchString,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string args)
    {
      if (searchString.Equals ("throw", StringComparison.OrdinalIgnoreCase))
        throw new Exception ("Test Exception");

      if (!string.IsNullOrEmpty (args))
      {
        var delay = int.Parse (args);
        Thread.Sleep (delay);
      }

      List<BusinessObjectWithIdentityProxy> persons = new List<BusinessObjectWithIdentityProxy>();
      foreach (Person person in XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)))
        persons.Add (
            new BusinessObjectWithIdentityProxy ((IBusinessObjectWithIdentity) person) { IconUrl = GetUrl (GetIcon ((IBusinessObject) person)) });

      foreach (string value in s_values)
        persons.Add (new BusinessObjectWithIdentityProxy { UniqueIdentifier = "invalid", DisplayName = value, IconUrl = GetUrl (IconInfo.CreateSpacer(ResourceUrlFactory)) });

      var filteredPersons = persons.FindAll (person => person.DisplayName.StartsWith (searchString, StringComparison.OrdinalIgnoreCase));
      if (filteredPersons.Count == 0)
        filteredPersons = persons.FindAll (person => person.DisplayName.IndexOf (searchString, StringComparison.OrdinalIgnoreCase) != -1);

      filteredPersons.Sort ((left, right) => string.Compare (left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase));

      return filteredPersons.ToArray();
    }

    private IResourceUrlFactory ResourceUrlFactory
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(); }
    }

    [WebMethod]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy SearchExact (string searchString, string businessObjectClass, string businessObjectProperty, string businessObject, string args)
    {
      if (searchString.Equals ("exactthrow", StringComparison.OrdinalIgnoreCase))
        throw new Exception ("Test Exception");

      if (!string.IsNullOrEmpty (args))
      {
        var delay = int.Parse (args);
        Thread.Sleep (delay);
      }

      var result = Search (searchString, 2, businessObjectClass, businessObjectProperty, businessObject, args);
      if (result.Length == 0)
        return null;
      if (string.Equals (result[0].DisplayName, searchString, StringComparison.CurrentCultureIgnoreCase))
        return result[0];
      return null;
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