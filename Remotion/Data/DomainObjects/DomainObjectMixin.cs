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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Base class for mixins adding persistent properties to domain objects.
  /// </summary>
  /// <typeparam name="TDomainObject">
  /// The type of the <see cref="Mixin{TTarget}.Target"/> property within the mixin. All members of this type must be available on the concrete mixed type. 
  /// Note that an <see cref="ExtendsAttribute"/> or <see cref="UsesAttribute"/> is still required to actually apply the mixin to the domain object type.
  /// See <see cref="Mixin{TTarget, TNext}"/> for additional information.
  /// </typeparam>
  /// <remarks>Use this base class to implement a mixin adding persistent properties to a domain object which does not need to call the base 
  /// implementation of any overridden methods. Use <see cref="DomainObjectMixin{TDomainObject,TNextCallRequirements}"/> if the mixin needs to call overridden
  /// base methods.</remarks>
  public class DomainObjectMixin<TDomainObject> : DomainObjectMixin<TDomainObject, IDomainObject>
    where TDomainObject : class, IDomainObject
  {
  }

  /// <summary>
  /// Base class for mixins adding persistent properties to domain objects.
  /// </summary>
  /// <typeparam name="TDomainObject">
  /// The type of the <see cref="Mixin{TTarget}.Target"/> property within the mixin. All members of this type must be available on the concrete mixed type. 
  /// Note that an <see cref="ExtendsAttribute"/> or <see cref="UsesAttribute"/> is still required to actually apply the mixin to the domain object type.
  /// See <see cref="Mixin{TTarget, TNext}"/> for additional information.
  /// </typeparam>
  /// <typeparam name="TNextCallRequirements">
  /// An interface type specifying the members whose base implementation needs to be called via the <see cref="Mixin{TTarget,TNext}.Next"/> property 
  /// when overridden by this mixin. The interface needs to implement <see cref="IDomainObject"/>. 
  /// See <see cref="Mixin{TTarget, TNext}"/> for additional information.
  /// </typeparam>
  /// <remarks><para>Use this base class to implement a mixin adding persistent properties to a domain object which overrides mixin members and needs to
  /// call the base implementations of these members on its target object. Specify those members you need to call via the
  /// <see cref="Mixin{TTarget,TNext}.Next"/> property via the <typeparamref name="TNextCallRequirements"/> type parameter; the target object does not
  /// have to actually implement this interface.</para>
  /// <para>Use <see cref="DomainObjectMixin{TDomainObject}"/> if the mixin does not need to call any base implementations of overridden members.</para></remarks>
  [NonIntroduced(typeof(IDomainObjectMixin))]
  public class DomainObjectMixin<TDomainObject, TNextCallRequirements>
      : Mixin<TDomainObject, TNextCallRequirements>, IDomainObjectMixin
      where TDomainObject : class, IDomainObject
      where TNextCallRequirements : class
  {
    private bool _domainObjectReferenceInitialized;

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of this mixin's target object.
    /// </summary>
    /// <value>The <see cref="ObjectID"/> of this mixin's target object.</value>
    [StorageClassNone]
    protected ObjectID ID
    {
      get { return Target.ID; }
    }

    /// <summary>
    /// Gets the type returned by <see cref="DomainObject.GetPublicDomainObjectType"/> when called on this mixin's target object.
    /// </summary>
    /// <value>The public domain object type of this mixin's target object.</value>
    protected Type GetPublicDomainObjectType ()
    {
      return Target.GetPublicDomainObjectType();
    }

    /// <summary>
    /// Gets the <see cref="DomainObjectState"/> returned by this mixin's target object's <see cref="DomainObject.State"/> property.
    /// </summary>
    /// <value>The state of this mixin's target object.</value>
    [StorageClassNone]
    protected DomainObjectState State
    {
      get { return Target.GetState(); }
    }

    /// <summary>
    /// Gets a value indicating whether this mixin's target object is invalid in its default transaction.
    /// </summary>
    /// <value><see langword="true" /> if this mixin's target object is invalid; otherwise, <see langword="false" />.</value>
    [Obsolete("Use State.IsInvalid instead. (Version: 1.21.8)", false)]
    [StorageClassNone]
    protected bool IsInvalid
    {
      get { return State.IsInvalid; }
    }

    /// <summary>
    /// Gets the properties of this mixin's target object, as returned by the <see cref="DomainObject.Properties"/> property.
    /// </summary>
    /// <value>The properties of the mixin's targetr object.</value>
    [StorageClassNone]
    protected PropertyIndexer Properties
    {
      get
      {
        if (!_domainObjectReferenceInitialized)
          throw new InvalidOperationException("This member cannot be used until the OnDomainObjectReferenceInitializing event has been executed.");

        return new PropertyIndexer(Target);
      }
    }

    void IDomainObjectMixin.OnDomainObjectReferenceInitializing ()
    {
      OnDomainObjectReferenceInitializing();
      _domainObjectReferenceInitialized = true;
    }

    /// <summary>
    /// Called when the mixin's target domain object is being initialized. This is executed right after 
    /// <see cref="DomainObject.OnReferenceInitializing"/>, see <see cref="DomainObject.OnReferenceInitializing"/> for details.
    /// </summary>
    protected virtual void OnDomainObjectReferenceInitializing ()
    {
    }

    void IDomainObjectMixin.OnDomainObjectCreated ()
    {
      OnDomainObjectCreated();
    }

    /// <summary>
    /// Called when the mixin's target domain object has been newly created, after the constructors have finished execution.
    /// </summary>
    protected virtual void OnDomainObjectCreated ()
    {
    }

    void IDomainObjectMixin.OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoaded(loadMode);
    }

    /// <summary>
    /// Called when the mixin's target domain object has been loaded.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="DataContainer"/> has been
    /// newly loaded.</param>
    protected virtual void OnDomainObjectLoaded (LoadMode loadMode)
    {
    }
  }
}
