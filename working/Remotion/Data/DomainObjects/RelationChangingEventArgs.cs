﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for a <b>RelationChanging</b> event.
  /// </summary>
  public class RelationChangingEventArgs : EventArgs
  {
    private readonly IRelationEndPointDefinition _relationEndPointDefinition;
    private readonly DomainObject _oldRelatedObject;
    private readonly DomainObject _newRelatedObject;

    /// <summary>
    /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
    /// </summary>
    /// <param name="relationEndPointDefinition">The relation endpoint definition. Must not be <see langword="null"/>.</param>
    /// <param name="oldRelatedObject">The related object that is removed from the relation, or <see langword="null" /> if a new item is added without 
    /// replacing an old one.</param>
    /// <param name="newRelatedObject">The related object that is added to the relation, or <see langword="null" /> if an old item is removed without 
    /// being replaced by a new one.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointDefinition"/> is <see langword="null"/>.</exception>
    public RelationChangingEventArgs (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      _relationEndPointDefinition = relationEndPointDefinition;
      _oldRelatedObject = oldRelatedObject;
      _newRelatedObject = newRelatedObject;
    }

    /// <summary>
    /// Gets the related object that is removed from the relation, or <see langword="null" /> if a new item is added without 
    /// replacing an old one.
    /// </summary>
    public DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    /// <summary>
    /// Gets the related object that is added to the relation, or <see langword="null" /> if an old item is removed without 
    /// being replaced by a new one.
    /// </summary>
    public DomainObject NewRelatedObject
    {
      get { return _newRelatedObject; }
    }

    /// <summary>
    /// Gets the relation endpoint defintion of the <see cref="PropertyValue"/> that has been changed due to the relation change.
    /// </summary>
    public IRelationEndPointDefinition RelationEndPointDefinition
    {
      get { return _relationEndPointDefinition; }
    }
  }
}