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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Base implementation for <see cref="ControlObject"/> selector implementations.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class ControlSelectorBase<TControlObject> : IHtmlIDControlSelector<TControlObject>, ILocalIDControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    /// <inheritdoc/>
    public TControlObject SelectPerHtmlID (ControlSelectionContext context, string htmlID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var scope = FindPerHtmlID (context, htmlID);
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TControlObject SelectOptionalPerHtmlID (ControlSelectionContext context, string htmlID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var scope = FindPerHtmlID (context, htmlID);

      if (!scope.ExistsWorkaround())
        return null;

      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public bool ExistsPerHtmlID (ControlSelectionContext context, string htmlID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var scope = FindPerHtmlID (context, htmlID);

      return scope.ExistsWorkaround();
    }

    /// <inheritdoc/>
    public TControlObject SelectPerLocalID (ControlSelectionContext context, string localID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      var scope = FindPerLocalID (context, localID);
      if (!scope.ExistsWorkaround())
        scope = FindPerHtmlID (context, localID);

      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public TControlObject SelectOptionalPerLocalID (ControlSelectionContext context, string localID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      var scope = FindPerLocalID (context, localID);
      if (scope.ExistsWorkaround())
        return CreateControlObject (context, scope);

      scope = FindPerHtmlID (context, localID);
      if (scope.ExistsWorkaround())
        return CreateControlObject (context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerLocalID (ControlSelectionContext context, string localID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNullOrEmpty ("localID", localID);

      var scope = FindPerLocalID (context, localID);
      if (scope.ExistsWorkaround())
        return true;

      scope = FindPerHtmlID (context, localID);
      return scope.ExistsWorkaround();
    }

    /// <summary>
    /// Creates a control object of type <typeparamref name="TControlObject"/> using the given <paramref name="context"/> and <paramref name="scope"/>,
    /// making help of the template method <see cref="CreateControlObject(ControlObjectContext,ControlSelectionContext)"/>.
    /// </summary>
    protected TControlObject CreateControlObject ([NotNull] ControlSelectionContext context, [NotNull] ElementScope scope)
    {
      var newControlObjectContext = context.CloneForControl (context.PageObject, scope);
      return CreateControlObject (newControlObjectContext, context);
    }

    /// <summary>
    /// Template method for derived control selectors to instantiate the control object.
    /// </summary>
    protected abstract TControlObject CreateControlObject (
        [NotNull] ControlObjectContext newControlObjectContext,
        [NotNull] ControlSelectionContext controlSelectionContext);

    private ElementScope FindPerHtmlID (ControlSelectionContext context, string htmlID)
    {
      return context.Scope.FindId (htmlID);
    }

    private ElementScope FindPerLocalID (ControlSelectionContext context, string localID)
    {
      return context.Scope.FindIdEndingWith ("_" + localID);
    }
  }
}