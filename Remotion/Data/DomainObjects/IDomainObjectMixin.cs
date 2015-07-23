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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Allows a mixin applied to a <see cref="DomainObject"/> to react on events related to the <see cref="DomainObject"/> instance.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implement this interface on a mixin applied to a <see cref="DomainObject"/> to be informed about when the <see cref="DomainObject"/> instance
  /// is created or loaded.
  /// </para>
  /// <para>
  /// The hook methods defined on
  /// this interface are called by the <see cref="DomainObjects"/> infrastructure at points of time when it is safe to access the domain object's
  /// ID and properties. Use them instead of <see cref="Mixin{TTarget}.OnInitialized"/> to execute mixin initialization code that must access
  /// the domain object's properties.
  /// </para>
  /// </remarks>
  public interface IDomainObjectMixin
  {
    /// <summary>
    /// Called when the mixin's target domain object has been initialized. This is executed right after 
    /// <see cref="DomainObject.OnReferenceInitializing"/>, see <see cref="DomainObject.OnReferenceInitializing"/> for details.
    /// </summary>
    void OnDomainObjectReferenceInitializing ();

    /// <summary>
    /// Called when the mixin's target domain object has been newly created, after the constructors have finished execution.
    /// </summary>
    void OnDomainObjectCreated ();
    
    /// <summary>
    /// Called when the mixin's target domain object has been loaded.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="DataContainer"/> has been
    /// newly loaded.</param>
    void OnDomainObjectLoaded (LoadMode loadMode);
  }
}
