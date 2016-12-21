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

namespace Remotion.Data.DomainObjects
{
  /// <summary>The <see cref="StorageGroupAttribute"/> is the base class for defining storage groups in the domain layer.</summary>
  /// <remarks>
  /// <para>
  /// A storage group is a logical grouping of all classes within a domain that should be persisted with the same storage provider.
  /// </para><para>
  /// Define the <see cref="StorageGroupAttribute"/> at the base classes of the domain layer to signify the root for the persistence hierarchy.
  /// </para><para>
  /// If no storage group is deifned for a persistence hierarchy, the domain object classes are assigned to the default storage provider.
  /// </para> 
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class StorageGroupAttribute: Attribute
  {
    protected StorageGroupAttribute()
    {
    }
  }
}
