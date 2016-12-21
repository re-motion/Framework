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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Base implementation for <see cref="ControlObject"/> selector implementations which can identify the <typeparamref name="TControlObject"/> via
  /// <see cref="DiagnosticMetadataAttributes.ControlType"/> metadata.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class TypedControlSelectorBase<TControlObject>
      : ControlSelectorBase<TControlObject>,
          IFirstControlSelector<TControlObject>,
          IIndexControlSelector<TControlObject>,
          ISingleControlSelector<TControlObject>
      where TControlObject : WebFormsControlObjectWithDiagnosticMetadata
  {
    private readonly string _controlType;

    /// <param name="controlType">The <see cref="DiagnosticMetadataAttributes.ControlType"/> identifying the <typeparamref name="TControlObject"/>.</param>
    protected TypedControlSelectorBase ([NotNull] string controlType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("controlType", controlType);

      _controlType = controlType;
    }

    /// <summary>
    /// Returns the <see cref="DiagnosticMetadataAttributes.ControlType"/> identifying the <typeparamref name="TControlObject"/>.
    /// </summary>
    protected string ControlType
    {
      get { return _controlType; }
    }

    /// <inheritdoc/>
    public TControlObject SelectFirst (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindTagWithAttribute ("*", DiagnosticMetadataAttributes.ControlType, _controlType);
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TControlObject SelectSingle (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindTagWithAttribute ("*", DiagnosticMetadataAttributes.ControlType, _controlType);
      scope.EnsureSingle();
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TControlObject SelectPerIndex (ControlSelectionContext context, int index)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var hasAttributeCheck = XPathUtils.CreateHasAttributeCheck (DiagnosticMetadataAttributes.ControlType, _controlType);
      var scope = context.Scope.FindXPath (string.Format ("(.//*{0})[{1}]", hasAttributeCheck, index));
      return CreateControlObject (context, scope);
    }
  }
}