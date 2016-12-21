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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="DomainPropertyControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IDomainPropertyControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class DomainPropertyControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IDomainPropertyControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly string _domainProperty;
    private readonly string _domainClass;

    public DomainPropertyControlSelectionCommandBuilder ([NotNull] string domainProperty, [CanBeNull] string domainClass = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      _domainProperty = domainProperty;
      _domainClass = domainClass;
    }

    /// <inheritdoc/>
    public IControlSelectionCommand<TControlObject> Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new DomainPropertyControlSelectionCommand<TControlObject> (controlSelector, _domainProperty, _domainClass);
    }
  }
}