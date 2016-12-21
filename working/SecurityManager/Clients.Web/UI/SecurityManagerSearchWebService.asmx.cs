// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  /// <summary>
  /// The <see cref="SecurityManagerSearchWebService"/> is used as an interface between <see cref="BocAutoCompleteReferenceValue"/> controls and the 
  /// <see cref="ISearchAvailableObjectsService"/> implementation.
  /// </summary>
  [WebService (Namespace = "http://www.re-motion.org/SecurityManager/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class SecurityManagerSearchWebService : WebService, ISearchAvailableObjectWebService
  {
    public static void BindServiceToControl (BocAutoCompleteReferenceValue control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
      control.SearchServicePath =
          resourceUrlFactory.CreateResourceUrl (typeof (SecurityManagerSearchWebService), ResourceType.UI, "SecurityManagerSearchWebService.asmx")
              .GetUrl();
    }

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
      ArgumentUtility.CheckNotNullOrEmpty ("businessObjectClass", businessObjectClass);
      ArgumentUtility.CheckNotNullOrEmpty ("businessObjectProperty", businessObjectProperty);

      var businessObjectClassWithIdentity = GetBusinessObjectClassWithIdentity (businessObjectClass);
      var referenceProperty = GetReferenceProperty (businessObjectProperty, businessObjectClassWithIdentity);
      var securityManagerSearchArguments = GetSearchArguments (args, completionSetCount, searchString);

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransaction.Current.Extensions.Add (new SecurityClientTransactionExtension());
        var referencingObject = businessObjectClassWithIdentity.GetObject (businessObject);
        var result = referenceProperty.SearchAvailableObjects (referencingObject, securityManagerSearchArguments);
        return result.Cast<IBusinessObjectWithIdentity>().Select (o => new BusinessObjectWithIdentityProxy (o)).ToArray();
      }
    }

    [WebMethod (EnableSession = true)]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy SearchExact (
        string searchString,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObject,
        string args)
    {
      var result = Search (searchString, 2, businessObjectClass, businessObjectProperty, businessObject, args);
      var hasSingleMatch = result.Count() == 1;
      if (hasSingleMatch)
        return result.Single();

      var exactMatches = result.Where (r => string.Equals (r.DisplayName, searchString, StringComparison.CurrentCultureIgnoreCase)).ToArray();
      var hasExactMatch = exactMatches.Count() == 1;
      if (hasExactMatch)
        return exactMatches.Single();

      return null;
    }

    private IBusinessObjectReferenceProperty GetReferenceProperty (
        string businessObjectProperty, IBusinessObjectClassWithIdentity businessObjectClassWithIdentity)
    {
      var propertyDefinition = businessObjectClassWithIdentity.GetPropertyDefinition (businessObjectProperty);
      Assertion.IsNotNull (propertyDefinition);
      Assertion.IsTrue (propertyDefinition is IBusinessObjectReferenceProperty);

      return (IBusinessObjectReferenceProperty) propertyDefinition;
    }

    private IBusinessObjectClassWithIdentity GetBusinessObjectClassWithIdentity (string businessObjectClass)
    {
      var type = TypeUtility.GetType (businessObjectClass, true);
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (type);
      var bindableObjectClass = provider.GetBindableObjectClass (type);
      Assertion.IsNotNull (bindableObjectClass);
      Assertion.IsTrue (bindableObjectClass is IBusinessObjectClassWithIdentity);

      return (IBusinessObjectClassWithIdentity) bindableObjectClass;
    }

    private SecurityManagerSearchArguments GetSearchArguments (string args, int? completionSetCount, string prefixText)
    {
      return new SecurityManagerSearchArguments (
          GetTenantConstraint (args),
          GetResultSizeConstraint (completionSetCount),
          GetDisplayNameConstraint (prefixText));
    }

    private TenantConstraint GetTenantConstraint (string args)
    {
      if (string.IsNullOrEmpty (args))
        return null;

      return new TenantConstraint (ObjectID.Parse (args).GetHandle<Tenant>());
    }

    private ResultSizeConstraint GetResultSizeConstraint (int? completionSetCount)
    {
      if (!completionSetCount.HasValue)
        return null;

      return new ResultSizeConstraint  (completionSetCount.Value);
    }

    private DisplayNameConstraint GetDisplayNameConstraint (string prefixText)
    {
      if (string.IsNullOrEmpty (prefixText))
        return null;

      return new DisplayNameConstraint (prefixText);
    }
  }
}