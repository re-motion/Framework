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
using System.Reflection;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Holds all information necessary to identify a concrete mixin type generated by <see cref="MixinParticipant"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Comparing <see cref="ConcreteMixinTypeIdentifier"/> instances requires a comparison of the sets of <see cref="Overriders"/> and
  /// <see cref="Overridden"/> and should therefore not be performed in tight loops. Getting the hash code is, however, quite fast, as it
  /// is cached.
  /// </para>
  /// </remarks>
  public sealed class ConcreteMixinTypeIdentifier
  {
    /// <summary>
    /// Deserializes an <see cref="ConcreteMixinTypeIdentifier"/> from the given deserializer.
    /// </summary>
    /// <param name="deserializer">The deserializer to use.</param>
    public static ConcreteMixinTypeIdentifier Deserialize (IConcreteMixinTypeIdentifierDeserializer deserializer)
    {
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);

      var mixinType = deserializer.GetMixinType ();
      var externalOverriders = deserializer.GetOverriders ();
      var wrappedProtectedMembers = deserializer.GetOverridden ();

      return new ConcreteMixinTypeIdentifier (mixinType, externalOverriders, wrappedProtectedMembers);
    }

    private readonly Type _mixinType;
    private readonly HashSet<MethodInfo> _overriders;
    private readonly HashSet<MethodInfo> _overridden;
    private readonly int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcreteMixinTypeIdentifier"/> class.
    /// </summary>
    /// <param name="mixinType">The mixin type for which a concrete type was generated.</param>
    /// <param name="overriders">Mixin methods that override methods of the target class.</param>
    /// <param name="overridden">Mixin methods that are overridden by the target class.</param>
    public ConcreteMixinTypeIdentifier (Type mixinType, HashSet<MethodInfo> overriders, HashSet<MethodInfo> overridden)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("overriders", overriders);
      ArgumentUtility.CheckNotNull ("overridden", overridden);

      _mixinType = mixinType;
      _overriders = overriders;
      _overridden = overridden;

      _cachedHashCode = MixinType.GetHashCode ()
          ^ EqualityUtility.GetXorHashCode (_overriders)
          ^ EqualityUtility.GetXorHashCode (_overridden);
    }

    /// <summary>
    /// Gets the mixin type for which a concrete type was generated.
    /// </summary>
    /// <value>The mixin type for which a concrete type was generated.</value>
    public Type MixinType
    {
      get { return _mixinType; }
    }

    /// <summary>
    /// Gets mixin methods that override methods of the target class. These are called by the mixin's target classes and may require public wrappers
    /// in the concrete mixin type.
    /// </summary>
    /// <value>Mixin methods that override methods of the target class.</value>
    public IEnumerable<MethodInfo> Overriders
    {
      get { return _overriders; }
    }

    /// <summary>
    /// Gets the mixin methods that are overridden by the target class. These are overridden in the concrete mixin type and call back to the target
    /// classes.
    /// </summary>
    /// <value>Mixin methods that are overridden by the target class.</value>
    public IEnumerable<MethodInfo> Overridden
    {
      get { return _overridden; }
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to this <see cref="ConcreteMixinTypeIdentifier"/>. Checks all 
    /// properties for equality, ignoring the order of the items in the <see cref="MethodInfo"/> sets.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="ConcreteMixinTypeIdentifier"/>.</param>
    /// <returns>
    /// true if the specified <see cref="T:System.Object"/> is an <see cref="ConcreteMixinTypeIdentifier"/> that corresponds to the same concrete
    /// mixin type; otherwise, false.
    /// </returns>
    public override bool Equals (object? obj)
    {
      var other = obj as ConcreteMixinTypeIdentifier;
      return other != null 
          && other.MixinType == MixinType 
          && other._overriders.SetEquals (_overriders) 
          && other._overridden.SetEquals (_overridden);
    }

    /// <summary>
    /// Serves as a hash function for this <see cref="ConcreteMixinTypeIdentifier"/>, matching the <see cref="Equals"/> implementation.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="ConcreteMixinTypeIdentifier"/>.
    /// </returns>
    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    /// <summary>
    /// Serializes this object with the specified serializer.
    /// </summary>
    /// <param name="serializer">The serializer to use.</param>
    public void Serialize (IConcreteMixinTypeIdentifierSerializer serializer)
    {
      ArgumentUtility.CheckNotNull ("serializer", serializer);

      serializer.AddMixinType (MixinType);
      serializer.AddOverriders (_overriders);
      serializer.AddOverridden (_overridden);
    }

    public override string ToString ()
    {
      return string.Format ("Generated mixin type: {0}, {1} overriders, {2} overridden methods", MixinType, _overriders.Count, _overridden.Count);
    }
  }
}
