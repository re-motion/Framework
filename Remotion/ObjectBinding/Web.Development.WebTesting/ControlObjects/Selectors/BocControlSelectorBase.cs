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
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector base class for all business object controls.
  /// </summary>
  public abstract class BocControlSelectorBase<TControlObject>
      : TypedControlSelectorBase<TControlObject>,
        IDisplayNameControlSelector<TControlObject>,
        IDomainPropertyControlSelector<TControlObject>
      where TControlObject : BocControlObject
  {
    protected BocControlSelectorBase ([NotNull] string controlType)
        : base(controlType)
    {
    }

    /// <inheritdoc/>
    public TControlObject SelectPerDisplayName (ControlSelectionContext context, string displayName)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("displayName", displayName);

      var scope = FindScopePerDisplayName(context, displayName);

      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public TControlObject? SelectOptionalPerDisplayName (ControlSelectionContext context, string displayName)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("displayName", displayName);

      var scope = FindScopePerDisplayName(context, displayName);

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerDisplayName (ControlSelectionContext context, string displayName)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("displayName", displayName);

      var scope = FindScopePerDisplayName(context, displayName);

      return scope.ExistsWorkaround();
    }

    /// <inheritdoc/>
    public TControlObject SelectPerDomainProperty (ControlSelectionContext context, string domainProperty, string? domainClass)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty("domainClass", domainClass);

      var scope = FindScopePerDomainProperty(context, domainProperty, domainClass);

      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public TControlObject? SelectOptionalPerDomainProperty (ControlSelectionContext context, string domainProperty, string? domainClass)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty("domainClass", domainClass);

      var scope = FindScopePerDomainProperty(context, domainProperty, domainClass);

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerDomainProperty (ControlSelectionContext context, string domainProperty, string? domainClass)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty("domainClass", domainClass);

      var scope = FindScopePerDomainProperty(context, domainProperty, domainClass);

      return scope.ExistsWorkaround();
    }

    private ElementScope FindScopePerDisplayName (ControlSelectionContext context, string displayName)
    {
      var diagnosticMetadata = new Dictionary<string, string>
                               {
                                   { DiagnosticMetadataAttributes.ControlType, ControlType },
                                   { DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName }
                               };

      return context.Scope.FindTagWithAttributes("*", diagnosticMetadata);
    }

    private ElementScope FindScopePerDomainProperty (ControlSelectionContext context, string domainProperty, string? domainClass)
    {
      var diagnosticMetadata = new Dictionary<string, string>
                               {
                                   { DiagnosticMetadataAttributes.ControlType, ControlType },
                                   { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty }
                               };

      if (domainClass != null)
        diagnosticMetadata.Add(DiagnosticMetadataAttributesForObjectBinding.BoundType, domainClass);

      return context.Scope.FindTagWithAttributes("*", diagnosticMetadata);
    }
  }
}