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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Instantiable]
  [DBTable]
  public abstract class SecurableClassDefinition : MetadataObject, ISupportsGetObject
  {
    public static SecurableClassDefinition NewObject ()
    {
      return NewObject<SecurableClassDefinition>();
    }

    public static SecurableClassDefinition? FindByName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                   where c.Name == name
                   select c;

      return result.SingleOrDefault();
    }

    public static ObjectList<SecurableClassDefinition> FindAll ()
    {
      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                   orderby c.Index
                   select c;

      return result.ToObjectList();
    }

    public static ObjectList<SecurableClassDefinition> FindAllBaseClasses ()
    {
      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                   where c.BaseClass == null
                   orderby c.Index
                   select c;

      return result.ToObjectList();
    }

    private DomainObjectDeleteHandler? _deleteHandler;

    protected SecurableClassDefinition ()
    {
    }

    [DBBidirectionalRelation("DerivedClasses")]
    [DBColumn("BaseSecurableClassID")]
    public abstract SecurableClassDefinition? BaseClass { get; set; }

    [DBBidirectionalRelation("BaseClass", SortExpression = "Index ASC")]
    public abstract ObjectList<SecurableClassDefinition> DerivedClasses { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DBBidirectionalRelation("Class")]
    protected abstract ObjectList<StatePropertyReference> StatePropertyReferences { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StatePropertyDefinition> StateProperties
    {
      get { return StatePropertyReferences.Select(propertyReference => propertyReference.StateProperty).ToList().AsReadOnly(); }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DBBidirectionalRelation("Class", SortExpression = "Index ASC")]
    protected abstract ObjectList<AccessTypeReference> AccessTypeReferences { get; }

    [StorageClassNone]
    public ReadOnlyCollection<AccessTypeDefinition> AccessTypes
    {
      get { return AccessTypeReferences.Select(accessTypeReference => accessTypeReference.AccessType).ToList().AsReadOnly(); }
    }

    [StorageClassNone]
    [ObjectBinding(ReadOnly = true)]
    public ReadOnlyCollection<StateCombination> StateCombinations
    {
      get { return StatefulAccessControlLists.SelectMany(acl => acl.StateCombinations).ToList().AsReadOnly(); }
    }

    //TODO RM-5636: Add tests
    public bool AreStateCombinationsComplete ()
    {
      if (StateProperties.Count > 1)
        throw new NotSupportedException("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (StateProperties.Count > 0)
        possibleStateCombinations = StateProperties[0].DefinedStates.Count;
      return StateCombinations.Count < possibleStateCombinations;
    }

    [DBBidirectionalRelation("MyClass")]
    public abstract StatelessAccessControlList? StatelessAccessControlList { get; set; }

    [DBBidirectionalRelation("MyClass", SortExpression = "Index ASC")]
    public abstract ObjectList<StatefulAccessControlList> StatefulAccessControlLists { get; }

    /// <summary>
    /// Adds an <see cref="AccessTypeDefinition"/> at end of the <see cref="AccessTypes"/> list.
    /// </summary>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be added. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to include a <see cref="Permission"/> entry for the new <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> already exists on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void AddAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      InsertAccessType(AccessTypeReferences.Count, accessType);
    }

    /// <summary>
    /// Inserts an <see cref="AccessTypeDefinition"/> at the specified <paramref name="index"/>. 
    /// </summary>
    /// <param name="index">The zero-based index at which the <paramref name="accessType"/> should be inserted.</param>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be inserted. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to include a <see cref="Permission"/> entry for the new <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> already exists on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="index"/> is less than 0.</para>
    /// <para> -or-</para>
    /// <para><paramref name="index"/> is greater than the total number of <see cref="AccessTypes"/>.</para>
    /// </exception>
    public void InsertAccessType (int index, AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);
      if (index < 0 || index > AccessTypeReferences.Count)
      {
        throw CreateArgumentOutOfRangeException(
            "index", index, "The index must not be less than 0 or greater than the total number of access types for the securable class definition.");
      }

      if (AccessTypeReferences.Where(r => r.AccessType == accessType).Any())
      {
        throw CreateArgumentException(
            "accessType", "The access type '{0}' has already been added to the securable class definition.", accessType.Name);
      }

      var reference = AccessTypeReference.NewObject();
      reference.AccessType = accessType;
      AccessTypeReferences.Insert(index, reference);
      for (int i = 0; i < AccessTypeReferences.Count; i++)
        AccessTypeReferences[i].Index = i;

      foreach (var ace in GetAccessControlLists().SelectMany(acl => acl.AccessControlEntries))
        ace.AddAccessType(accessType);

      RegisterForCommit();
    }

    /// <summary>
    /// Removes an <see cref="AccessTypeDefinition"/> from of the <see cref="AccessTypes"/> list.
    /// </summary>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be removed. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to remove the <see cref="Permission"/> entry for the <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> does not exist on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void RemoveAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);

      var accessTypeReference = AccessTypeReferences.SingleOrDefault(r => r.AccessType == accessType);
      if (accessTypeReference == null)
      {
        throw CreateArgumentException(
            "accessType", "The access type '{0}' is not associated with the securable class definition.", accessType.Name);
      }

      accessTypeReference.Delete();
      for (int i = 0; i < AccessTypeReferences.Count; i++)
        AccessTypeReferences[i].Index = i;

      foreach (var ace in GetAccessControlLists().SelectMany(acl => acl.AccessControlEntries))
        ace.RemoveAccessType(accessType);

      RegisterForCommit();
    }

    /// <summary>
    /// Moves an <see cref="AccessTypeDefinition"/> to the specified <paramref name="index"/>. 
    /// </summary>
    /// <param name="index">The zero-based index to which the <paramref name="accessType"/> should be moved.</param>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be moved. Must not be <see langword="null" />.</param>
    /// <remarks> Does not alter the <see cref="Permission"/> entries of the <see cref="AccessControlEntry"/> objects associated 
    /// with the <see cref="SecurableClassDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> does not exist on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="index"/> is less than 0.</para>
    /// <para> -or-</para>
    /// <para><paramref name="index"/> is greater than the top index of the <see cref="AccessTypes"/>.</para>
    /// </exception>
    public void MoveAccessType (int index, AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull("accessType", accessType);
      if (index < 0 || index >= AccessTypeReferences.Count)
      {
        throw CreateArgumentOutOfRangeException(
            "index", index, "The index must not be less than 0 or greater than the top index of the access types for the securable class definition.");
      }

      var accessTypeReference = AccessTypeReferences.SingleOrDefault(r => r.AccessType == accessType);
      if (accessTypeReference == null)
      {
        throw CreateArgumentException(
            "accessType", "The access type '{0}' is not associated with the securable class definition.", accessType.Name);
      }

      AccessTypeReferences.Remove(accessTypeReference);
      AccessTypeReferences.Insert(index, accessTypeReference);
      for (int i = 0; i < AccessTypeReferences.Count; i++)
        AccessTypeReferences[i].Index = i;

      RegisterForCommit();
    }

    /// <summary>
    /// Adds a <see cref="StatePropertyDefinition"/> to the <see cref="StateProperties"/> list.
    /// </summary>
    /// <param name="stateProperty">The <see cref="StatePropertyDefinition"/> to be added. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    /// The <paramref name="stateProperty"/> already exists on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void AddStateProperty (StatePropertyDefinition stateProperty)
    {
      ArgumentUtility.CheckNotNull("stateProperty", stateProperty);

      if (StatePropertyReferences.Where(r => r.StateProperty == stateProperty).Any())
      {
        throw CreateArgumentException(
            "stateProperty", "The property '{0}' has already been added to the securable class definition.", stateProperty.Name);
      }

      var reference = StatePropertyReference.NewObject();
      reference.StateProperty = stateProperty;

      StatePropertyReferences.Add(reference);

      RegisterForCommit();
    }

    /// <summary>
    /// Removes a <see cref="StatePropertyDefinition"/> from of the <see cref="StateProperties"/> list.
    /// </summary>
    /// <param name="stateProperty">The <see cref="StatePropertyDefinition"/> to be removed. Must not be <see langword="null" />.</param>
    /// <remarks> 
    /// Also deletes all entries from the <see cref="StatefulAccessControlLists"/> list that use only the removed <see cref="StatePropertyDefinition"/>
    /// as a selection criteria.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="stateProperty"/> does not exist on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void RemoveStateProperty (StatePropertyDefinition stateProperty)
    {
      ArgumentUtility.CheckNotNull("stateProperty", stateProperty);

      var statePropertyReference = StatePropertyReferences.SingleOrDefault(r => r.StateProperty == stateProperty);
      if (statePropertyReference == null)
      {
        throw CreateArgumentException(
            "stateProperty", "The property '{0}' does not exist on the securable class definition.", stateProperty.Name);
      }

      statePropertyReference.Delete();

      foreach (var acl in StatefulAccessControlLists.ToList())
      {
        var stateCombinationsContainingRemovedStateProperty
            = acl.StateCombinations.Where(sc => sc.GetStates().Any(sd => sd.StateProperty == stateProperty)).ToList();
        foreach (var stateCombination in stateCombinationsContainingRemovedStateProperty)
        {
          stateCombination.Delete();
          if (!acl.StateCombinations.Any())
            acl.Delete();
        }
      }

      RegisterForCommit();
    }

    /// <summary>Retrieves the <see cref="StatePropertyDefinition"/> with the passed name.</summary>
    /// <param name="propertyName">Name of the <see cref="StatePropertyDefinition"/> to retrieve.Must not be <see langword="null" /> or empty. </param>
    /// <exception cref="ArgumentException">Thrown if the specified property does not exist on this <see cref="SecurableClassDefinition"/>.</exception>
    public StatePropertyDefinition GetStateProperty (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      return StateProperties.Single(
          p => p.Name == propertyName,
          () => CreateArgumentException(
              "propertyName",
              "A state property with the name '{0}' is not defined for the secureable class definition '{1}'.",
              propertyName,
              Name));
    }

    public StateCombination? FindStateCombination (IList<StateDefinition> states)
    {
      return StateCombinations.Where(sc => sc.MatchesStates(states)).SingleOrDefault();
    }

    public StatelessAccessControlList CreateStatelessAccessControlList ()
    {
      if (StatelessAccessControlList != null)
        throw new InvalidOperationException("A SecurableClassDefinition only supports a single StatelessAccessControlList at a time.");

      var accessControlList = StatelessAccessControlList.NewObject();
      StatelessAccessControlList = accessControlList;
      accessControlList.CreateAccessControlEntry();

      return accessControlList;
    }

    public StatefulAccessControlList CreateStatefulAccessControlList ()
    {
      var accessControlList = StatefulAccessControlList.NewObject();
      StatefulAccessControlLists.Add(accessControlList);
      accessControlList.CreateStateCombination();
      accessControlList.CreateAccessControlEntry();

      return accessControlList;
    }

    public SecurableClassValidationResult Validate ()
    {
      var result = new SecurableClassValidationResult();

      ValidateUniqueStateCombinations(result);

      ValidateStateCombinationsAgainstStateProperties(result);

      return result;
    }

    public void ValidateUniqueStateCombinations (SecurableClassValidationResult result)
    {
      Assertion.IsFalse(
          State.IsDeleted && StateCombinations.Count != 0, "StateCombinations of object '{0}' are not empty but the object is deleted.", ID);

      var duplicateStateCombinations = StateCombinations
          .GroupBy(sc => sc, new StateCombinationComparer())
          .Where(g => g.Count() > 1)
          .SelectMany(g => g);

      foreach (var stateCombination in duplicateStateCombinations)
        result.AddDuplicateStateCombination(stateCombination);
    }

    public void ValidateStateCombinationsAgainstStateProperties (SecurableClassValidationResult result)
    {
      Assertion.IsFalse(
          State.IsDeleted && StateCombinations.Count != 0, "StateCombinations of object '{0}' are not empty but the object is deleted.", ID);

      foreach (var stateCombination in StateCombinations.Where(sc => sc.GetStates().Length != StateProperties.Count))
        result.AddInvalidStateCombination(stateCombination);
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      SecurableClassValidationResult result = Validate();
      if (!result.IsValid)
      {
        if (result.DuplicateStateCombinations.Count > 0)
        {
          throw new ConstraintViolationException(
              String.Format("The securable class definition '{0}' contains at least one state combination that has been defined twice.", Name));
        }
        else
        {
          Assertion.IsTrue(result.InvalidStateCombinations.Count > 0);
          throw new ConstraintViolationException(
              String.Format("The securable class definition '{0}' contains at least one state combination that does not match the class's properties.", Name));
        }
      }

      RegisterForCommit();

      base.OnCommitting(args);
    }

    protected override void OnRelationChanged (RelationChangedEventArgs args)
    {
      base.OnRelationChanged(args);
      if (args.IsRelation(this, nameof(StatefulAccessControlLists)))
        HandleStatefulAccessControlListsChanged();
    }

    private void HandleStatefulAccessControlListsChanged ()
    {
      var acls = StatefulAccessControlLists;
      for (int i = 0; i < acls.Count; i++)
        acls[i].Index = i;
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting(args);

      //TODO: Rewrite with test
      _deleteHandler = new DomainObjectDeleteHandler(
          StatefulAccessControlLists,
          EnumerableUtility.Singleton(StatelessAccessControlList).Where(o => o != null),
          StatePropertyReferences,
          AccessTypeReferences);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      //TODO: Rewrite with test
      _deleteHandler?.Delete();
    }

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException(String.Format(format, args), argumentName);
    }

    private ArgumentException CreateArgumentOutOfRangeException (string argumentName, object actualValue, string format, params object[] args)
    {
      return new ArgumentOutOfRangeException(argumentName, actualValue, String.Format(format, args));
    }

    private IEnumerable<AccessControlList> GetAccessControlLists ()
    {
      if (StatelessAccessControlList != null)
        yield return StatelessAccessControlList;

      foreach (var acl in StatefulAccessControlLists)
        yield return acl;
    }
  }
}
