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
        : base (controlType)
    {
    }

    /// <inheritdoc/>
    public TControlObject SelectPerDisplayName (ControlSelectionContext context, string displayName)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("displayName", displayName);

      var scope = context.Scope.FindTagWithAttributes (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributesForObjectBinding.DisplayName, displayName }
          });

      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TControlObject SelectPerDomainProperty (ControlSelectionContext context, string domainProperty, string domainClass)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("domainProperty", domainProperty);

      var diagnosticMetadata = new Dictionary<string, string>
                               {
                                   { DiagnosticMetadataAttributes.ControlType, ControlType },
                                   { DiagnosticMetadataAttributesForObjectBinding.BoundProperty, domainProperty }
                               };

      if (domainClass != null)
        diagnosticMetadata.Add (DiagnosticMetadataAttributesForObjectBinding.BoundType, domainClass);

      var scope = context.Scope.FindTagWithAttributes ("*", diagnosticMetadata);
      return CreateControlObject (context, scope);
    }
  }
}