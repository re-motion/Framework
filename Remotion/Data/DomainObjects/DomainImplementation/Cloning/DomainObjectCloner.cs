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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Cloning
{
  /// <summary>
  /// Assists in cloning <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCloner
  {
    private ClientTransaction? _cloneTransaction;

    /// <summary>
    /// Gets or sets the transaction to be used for the clone. If this is set to <see langword="null"/>, the current transaction is used.
    /// </summary>
    /// <value>The clone transaction.</value>
    public ClientTransaction CloneTransaction
    {
      get { return _cloneTransaction ?? ClientTransactionScope.CurrentTransaction; }
      set { _cloneTransaction = value; }
    }

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Relations are not cloned, foreign key properties default to null.
    /// </summary>
    /// <returns>A clone of the given <paramref name="source"/> object.</returns>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. The actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <remarks>
    /// The clone is created in the <see cref="CloneTransaction"/>. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public virtual T CreateValueClone<T> (T source)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("source", source);

      T clone = CreateCloneHull(source);
      CopyProperties(source, clone, null, null);
      InvokeClonerCallback(source, clone, _cloneTransaction);
      return clone;
    }

    /// <summary>
    /// Creates a clone hull, which is a <see cref="DomainObject"/> of the same type as a given source object, but with no properties or
    /// relations being set.
    /// </summary>
    /// <returns>A clone of the given <paramref name="source"/> object.</returns>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. The actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <remarks>
    /// The clone is created in the <see cref="CloneTransaction"/>. No constructor is called on the clone object.
    /// </remarks>
    public virtual T CreateCloneHull<T> (T source)
        where T : DomainObject
    {
      var type = source.ID.ClassDefinition.Type;
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(type);

      // Get an object reference for a non-existing object, then register a new DataContainer for it. This, in effect, creates a New object that has
      // no ctor called.
      var cloneObjectID = CloneTransaction.CreateNewObjectID(classDefinition);
      var instance = CloneTransaction.GetObjectReference(cloneObjectID);

      var cloneDataContainer = DataContainer.CreateNew(cloneObjectID);
      cloneDataContainer.SetDomainObject(instance);
      CloneTransaction.DataManager.RegisterDataContainer(cloneDataContainer);

      return (T)instance;
    }

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Referenced objects are cloned according to the given strategy.
    /// </summary>
    /// <returns>A clone of the given <paramref name="source"/> object.</returns>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. The actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <param name="strategy">The <see cref="ICloneStrategy"/> to be used when cloning the object's references.</param>
    /// <remarks>
    /// The clone is created in the c<see cref="CloneTransaction"/>. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public T CreateClone<T> (T source, ICloneStrategy strategy)
    where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("source", source);
      ArgumentUtility.CheckNotNull("strategy", strategy);

      return CreateClone(source, strategy, new CloneContext(this));
    }

    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Referenced objects are cloned according to the given strategy, the given context is used instead of creating a new one.
    /// </summary>
    /// <returns>A clone of the given <paramref name="source"/> object.</returns>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. The actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <param name="strategy">The <see cref="ICloneStrategy"/> to be used when cloning the object's references.</param>
    /// <param name="context">The <see cref="CloneContext"/> to be used by the cloner. (Must have been created for this <see cref="DomainObjectCloner"/>.</param>
    /// <remarks>
    /// The clone is created in the <see cref="CloneTransaction"/>. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public T CreateClone<T> (T source, ICloneStrategy strategy, CloneContext context)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("source", source);
      ArgumentUtility.CheckNotNull("strategy", strategy);
      ArgumentUtility.CheckNotNull("context", context);

      if (context.Cloner != this)
        throw new ArgumentException("The given CloneContext must have been created for this DomainObjectCloner.", "context");

      T clone = context.GetCloneFor(source);
      while (context.CloneHulls.Count > 0)
      {
        var shallowClone = context.CloneHulls.Dequeue();
        CopyProperties(source: shallowClone.Item1, clone: shallowClone.Item2, strategy: strategy, context: context);
        InvokeClonerCallback(source: shallowClone.Item1, clone: shallowClone.Item2, cloneTransaction: _cloneTransaction);
      }
      return clone;
    }

    private void CopyProperties<T> (T source, T clone, ICloneStrategy? strategy, CloneContext? context)
        where T : IDomainObject
    {
      var sourceTransaction = source.GetDefaultTransactionContext().ClientTransaction;
      var sourceProperties = new PropertyIndexer(source);
      var cloneProperties = new PropertyIndexer(clone);
      CopyProperties(sourceProperties, sourceTransaction, cloneProperties.AsEnumerable(CloneTransaction), strategy, context);
    }

    private void CopyProperties (
        PropertyIndexer sourceProperties,
        ClientTransaction sourceTransaction,
        IEnumerable<PropertyAccessor> cloneProperties,
        ICloneStrategy? strategy,
        CloneContext? context)
    {
      foreach (PropertyAccessor cloneProperty in cloneProperties)
      {
        PropertyAccessor sourceProperty = sourceProperties[cloneProperty.PropertyData.PropertyIdentifier, sourceTransaction];
        if (cloneProperty.PropertyData.Kind == PropertyKind.PropertyValue)
        {
          object? sourceValue = sourceProperty.GetValueWithoutTypeCheck();
          cloneProperty.SetValueWithoutTypeCheck(sourceValue);
        }
        else if (strategy != null && !cloneProperty.HasBeenTouched)
        {
          Assertion.DebugIsNotNull(context, "context != null when strategy != null");
          strategy.HandleReference(sourceProperty, cloneProperty, context);
        }
      }
    }

    private void InvokeClonerCallback (DomainObject source, DomainObject clone, ClientTransaction? cloneTransaction)
    {
      var callback = clone as IClonerCallback;
      if (callback != null)
        callback.OnObjectCreatedAsClone(cloneTransaction, source);
    }
  }
}
