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
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// Contains all context information required by the <see cref="ISearchAvailableObjectWebService"/>.
  /// </summary>
  [Serializable]
  public class SearchAvailableObjectWebServiceContext
  {
    private readonly string _businessObjectClass;
    private readonly string _businessObjectProperty;
    private readonly string _businessObjectIdentifier;
    private readonly string _args;

    public static SearchAvailableObjectWebServiceContext Create (IBusinessObjectDataSource dataSource, IBusinessObjectProperty property, string args)
    {
      var dataSourceOrNull = Maybe.ForValue (dataSource);

      var businessObjectClass =
          dataSourceOrNull.Select (ds => ds.BusinessObject).Select (bo => bo.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault()
          ?? dataSourceOrNull.Select (ds => ds.BusinessObjectClass).Select (c => c.Identifier).ValueOrDefault();

      var businessObjectProperty = Maybe.ForValue (property).Select (p => p.Identifier).ValueOrDefault();

      var businessObjectIdentifier =
          dataSourceOrNull.Select (ds => ds.BusinessObject as IBusinessObjectWithIdentity).Select (o => o.UniqueIdentifier).ValueOrDefault();

      return new SearchAvailableObjectWebServiceContext (
          businessObjectClass, businessObjectProperty, businessObjectIdentifier, StringUtility.EmptyToNull (args));
    }

    private SearchAvailableObjectWebServiceContext (
        string businessObjectClass, string businessObjectProperty, string businessObjectIdentifier, string args)
    {
      _businessObjectClass = businessObjectClass;
      _businessObjectProperty = businessObjectProperty;
      _businessObjectIdentifier = businessObjectIdentifier;
      _args = args;
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectClass.Identifier"/> of the <see cref="IBusinessObjectClass"/> 
    /// the <see cref="BocAutoCompleteReferenceValue"/> is bound to.
    /// </summary>
    public string BusinessObjectClass
    {
      get { return _businessObjectClass; }
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectProperty.Identifier"/> of the <see cref="IBusinessObjectReferenceProperty"/> 
    /// the <see cref="BocAutoCompleteReferenceValue"/> is bound to.
    /// </summary>
    public string BusinessObjectProperty
    {
      get { return _businessObjectProperty; }
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the <see cref="IBusinessObjectWithIdentity"/> 
    /// the <see cref="BocAutoCompleteReferenceValue"/> is bound to.
    /// </summary>
    public string BusinessObjectIdentifier
    {
      get { return _businessObjectIdentifier; }
    }

    /// <summary>
    /// Gets the search arguments specified for the <see cref="BocAutoCompleteReferenceValue"/>.
    /// </summary>
    public string Args
    {
      get { return _args; }
    }
  }
}