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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>Collects all security-specific information for a securable object, and is passed as parameter during the permission check.</summary>
  public sealed class SecurityContext : ISecurityContext, IEquatable<SecurityContext>
  {
    private static readonly ConcurrentDictionary<Type, bool> s_validAbstractRoleTypeCache = new ConcurrentDictionary<Type, bool>();
    private static readonly ConcurrentDictionary<Type, bool> s_validSecurityStateTypeCache = new ConcurrentDictionary<Type, bool>();

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for a stateless scenario, i.e. before an actual instance of the
    /// specified <paramref name="type"/> is available to supply state information. One such occurance would be the creation of a new instance of 
    /// specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext CreateStateless (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(ISecurableObject));

      return new SecurityContext(type, null, null, null, true, new Dictionary<string, EnumWrapper>(), new EnumWrapper[0]);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for stateful scenarios, i.e. when a concrete instance of the 
    /// specified <paramref name="type"/> is available to supply state information (e.g. the <paramref name="owner"/>, ...). This overload 
    /// requires that all state and abstract role values must be defined by a dot.NET <see cref="Enum"/> type.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <param name="owner">The name of the user that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerGroup">The name of the group that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerTenant">The name of the tenant that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="states">
    /// A dictionary containing a combination of property names and values, each of which describe a single security relevant property of the 
    /// securable object for which this <see cref="SecurityContext"/> is created. Must not be <see langword="null" />. Each <see cref="Enum"/> value
    /// must be of an <see cref="Enum"/> type that has the <see cref="SecurityStateAttribute"/> applied.
    /// </param>
    /// <param name="abstractRoles">
    /// The list abstract roles the current user has in regards to the securable object for which this <see cref="SecurityContext"/> is created.
    /// Must not be <see langword="null" />. Each <see cref="Enum"/> value must be of an <see cref="Enum"/> type that has the 
    /// <see cref="AbstractRoleAttribute"/> applied.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext Create (
        Type type, string? owner, string? ownerGroup, string? ownerTenant, IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(ISecurableObject));
      ArgumentUtility.CheckNotNull("states", states);
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      return new SecurityContext(type, owner, ownerGroup, ownerTenant, false, InitializeStates(states), InitializeAbstractRoles(abstractRoles));
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SecurityContext"/> type initialized for stateful scenarios, i.e. when a concrete instance of the 
    /// specified paramref name is available to supply state information (e.g. the <paramref name="owner"/>, ...). Use this overload to supply
    /// states and abstract roles that are not necessarily defined by a dot.NET <see cref="Enum"/> type.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the securable class for which this <see cref="SecurityContext"/> is created. 
    /// Must implement the <see cref="ISecurableObject"/> interface and not be <see langword="null" />.
    /// </param>
    /// <param name="owner">The name of the user that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerGroup">The name of the group that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="ownerTenant">The name of the tenant that owns the securable object for which this <see cref="SecurityContext"/> is created.</param>
    /// <param name="states">
    /// A dictionary containing a combination of property names and values, each of which describe a single security relevant property of the 
    /// securable object for which this <see cref="SecurityContext"/> is created. Must not be <see langword="null" />.
    /// </param>
    /// <param name="abstractRoles">
    /// The list abstract roles the current user has in regards to the securable object for which this <see cref="SecurityContext"/> is created.
    /// Must not be <see langword="null" />.
    /// </param>
    /// <returns>A new instance of the <see cref="SecurityContext"/> type.</returns>
    public static SecurityContext Create (
        Type type,
        string owner,
        string ownerGroup,
        string ownerTenant,
        IDictionary<string, EnumWrapper> states,
        ICollection<EnumWrapper> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(ISecurableObject));
      ArgumentUtility.CheckNotNull("states", states);
      ArgumentUtility.CheckNotNull("abstractRoles", abstractRoles);

      return new SecurityContext(type, owner, ownerGroup, ownerTenant, false, new Dictionary<string, EnumWrapper>(states), abstractRoles.ToArray());
    }

    private static EnumWrapper[] InitializeAbstractRoles (ICollection<Enum> abstractRoles)
    {
      List<EnumWrapper> abstractRoleList = new List<EnumWrapper>();

      foreach (Enum abstractRole in abstractRoles)
      {
        Type roleType = abstractRole.GetType();
        // C# compiler 7.2 already provides caching for anonymous method.
        if (!s_validAbstractRoleTypeCache.GetOrAdd(roleType, key => AttributeUtility.IsDefined<AbstractRoleAttribute>(key, false)))
        {
          string message = string.Format(
              "Enumerated Type '{0}' cannot be used as an abstract role. Valid abstract roles must have the {1} applied.",
              roleType,
              typeof(AbstractRoleAttribute).GetFullNameSafe());

          throw new ArgumentException(message, "abstractRoles");
        }

        abstractRoleList.Add(EnumWrapper.Get(abstractRole));
      }
      return abstractRoleList.ToArray();
    }

    private static Dictionary<string, EnumWrapper> InitializeStates (IDictionary<string, Enum> states)
    {
      Dictionary<string, EnumWrapper> securityStates = new Dictionary<string, EnumWrapper>();

      foreach (KeyValuePair<string, Enum> valuePair in states)
      {
        Type stateType = valuePair.Value.GetType();
        // C# compiler 7.2 already provides caching for anonymous method.
        if (!s_validSecurityStateTypeCache.GetOrAdd(stateType, key => AttributeUtility.IsDefined<SecurityStateAttribute>(key, false)))
        {
          string message = string.Format(
              "Enumerated Type '{0}' cannot be used as a security state. Valid security states must have the {1} applied.",
              stateType,
              typeof(SecurityStateAttribute).GetFullNameSafe());

          throw new ArgumentException(message, "states");
        }

        securityStates.Add(valuePair.Key, EnumWrapper.Get(valuePair.Value));
      }
      return securityStates;
    }

    private readonly string _class;
    private readonly string? _owner;
    private readonly string? _ownerGroup;
    private readonly string? _ownerTenant;
    private readonly bool _isStateless;
    private readonly Dictionary<string, EnumWrapper> _states;
    private readonly EnumWrapper[] _abstractRoles;
    private readonly int _hashCode;

    private SecurityContext (
        Type classType,
        string? owner,
        string? ownerGroup,
        string? ownerTenant,
        bool isStateless,
        Dictionary<string, EnumWrapper> states,
        EnumWrapper[] abstractRoles)
    {
      _class = TypeUtility.GetPartialAssemblyQualifiedName(classType);
      _owner = StringUtility.EmptyToNull(owner);
      _ownerGroup = StringUtility.EmptyToNull(ownerGroup);
      _ownerTenant = StringUtility.EmptyToNull(ownerTenant);
      _isStateless = isStateless;
      _states = states;
      _abstractRoles = abstractRoles;
      _hashCode = EqualityUtility.GetRotatedHashCode(_class, _owner, _ownerGroup, _ownerTenant);
    }

    public string Class
    {
      get { return _class; }
    }

    public string? Owner
    {
      get { return _owner; }
    }

    public string? OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string? OwnerTenant
    {
      get { return _ownerTenant; }
    }

    public IEnumerable<EnumWrapper> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      return _states.ContainsKey(propertyName);
    }

    public bool IsStateless
    {
      get { return _isStateless; }
    }

    public int GetNumberOfStates ()
    {
      return _states.Count;
    }

    public override int GetHashCode ()
    {
      return _hashCode;
    }

    public override bool Equals (object? obj)
    {
      return EqualityUtility.EqualsEquatable(this, obj);
    }

    bool IEquatable<SecurityContext>.Equals (SecurityContext? other)
    {
      if (other == null)
        return false;

      if (this._isStateless != other._isStateless)
        return false;

      if (!string.Equals(this._class, other._class, StringComparison.Ordinal))
        return false;

      if (!string.Equals(this._owner, other._owner, StringComparison.Ordinal))
        return false;

      if (!string.Equals(this._ownerGroup, other._ownerGroup, StringComparison.Ordinal))
        return false;

      if (!string.Equals(this._ownerTenant, other._ownerTenant, StringComparison.Ordinal))
        return false;

      if (!EqualsStates(this._states, other._states))
        return false;

      return EqualsAbstractRoles(this._abstractRoles, other._abstractRoles);
    }

    private bool EqualsStates (IDictionary<string, EnumWrapper> leftStates, IDictionary<string, EnumWrapper> rightStates)
    {
      if (leftStates.Count != rightStates.Count)
        return false;

      foreach (KeyValuePair<string, EnumWrapper> leftValuePair in leftStates)
      {
        EnumWrapper rightValue;
        if (!rightStates.TryGetValue(leftValuePair.Key, out rightValue))
          return false;
        if (!leftValuePair.Value.Equals(rightValue))
          return false;
      }

      return true;
    }

    private bool EqualsAbstractRoles (EnumWrapper[] leftAbstractRoles, EnumWrapper[] rightAbstractRoles)
    {
      if (leftAbstractRoles.Length != rightAbstractRoles.Length)
        return false;

      HashSet<EnumWrapper> left = new HashSet<EnumWrapper>(leftAbstractRoles);
      return left.SetEquals(rightAbstractRoles);
    }
  }
}
