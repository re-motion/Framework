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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.FunctionalProgramming;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  public abstract class StatePropertyDefinition : MetadataObject
  {
    public static StatePropertyDefinition NewObject ()
    {
      return NewObject<StatePropertyDefinition>();
    }

    public static StatePropertyDefinition NewObject (Guid metadataItemID, string name)
    {
      return NewObject<StatePropertyDefinition>(ParamList.Create(metadataItemID, name));
    }

    private DomainObjectDeleteHandler? _deleteHandler;

    protected StatePropertyDefinition ()
    {
    }

    protected StatePropertyDefinition (Guid metadataItemID, string name)
    {
      MetadataItemID = metadataItemID;
      Name = name;
    }

    [DBBidirectionalRelation("StateProperty")]
    protected abstract ObjectList<StatePropertyReference> StatePropertyReferences { get; }

    [DBBidirectionalRelation("StateProperty")]
    [Mandatory]
    protected abstract ObjectList<StateDefinition> DefinedStatesInternal { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StateDefinition> DefinedStates
    {
      get { return DefinedStatesInternal.OrderBy(s => s.Index).ToList().AsReadOnly(); }
    }

    public StateDefinition GetState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      return DefinedStatesInternal.Single(
          s => s.Name == name,
          () => CreateArgumentException("name", "The state '{0}' is not defined for the property '{1}'.", name, Name));
    }

    public bool ContainsState (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      return DefinedStatesInternal.Any(s => s.Name == name);
    }

    public StateDefinition GetState (int stateValue)
    {
      return DefinedStatesInternal.Single(
          s => s.Value == stateValue,
          () => CreateArgumentException("stateValue", "A state with the value {0} is not defined for the property '{1}'.", stateValue, Name));
    }

    public bool ContainsState (int stateValue)
    {
      return DefinedStatesInternal.Any(s => s.Value == stateValue);
    }

    [StorageClassNone]
    public StateDefinition this [string stateName]
    {
      get { return GetState(stateName); }
    }

    /// <summary>
    /// Adds a <see cref="StateDefinition"/> to the <see cref="DefinedStates"/> list.
    /// </summary>
    /// <param name="state">The <see cref="StateDefinition"/> to be added. Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    /// The <paramref name="state"/> already exists on the <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public void AddState (StateDefinition state)
    {
      ArgumentUtility.CheckNotNull("state", state);
      if (ContainsState(state.Name))
        throw CreateArgumentException("state", "A state with the name '{0}' was already added to the property '{1}'.", state.Name, Name);
      if (ContainsState(state.Value))
        throw CreateArgumentException("state", "A state with the value {0} was already added to the property '{1}'.", state.Value, Name);

      DefinedStatesInternal.Add(state);
    }

    /// <summary>
    /// Removes a <see cref="StateDefinition"/> from of the <see cref="DefinedStates"/> list.
    /// </summary>
    /// <param name="state">The <see cref="StateDefinition"/> to be removed. Must not be <see langword="null" />.</param>
    /// <remarks> 
    /// Also deletes all <see cref="StatefulAccessControlList"/> objects that use only the removed <see cref="StateDefinition"/>
    /// as a selection criteria.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="state"/> does not exist on the <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public void RemoveState (StateDefinition state)
    {
      ArgumentUtility.CheckNotNull("state", state);

      if (!DefinedStatesInternal.Contains(state.ID))
          throw CreateArgumentException("state", "The state '{0}' does not exist on the property '{1}'.", state.Name, Name);

      DefinedStatesInternal.Remove(state);

      foreach (var acl in StatePropertyReferences.SelectMany(r=> r.Class.StatefulAccessControlLists).ToList())
      {
        var stateCombinationsContainingRemovedState = acl.StateCombinations.Where(sc => sc.GetStates().Contains(state)).ToList();
        foreach (var stateCombination in stateCombinationsContainingRemovedState)
        {
          stateCombination.Delete();
          if (!acl.StateCombinations.Any())
            acl.Delete();
        }
      }
    }

    protected override void OnDeleting (EventArgs args)
    {
      if (StatePropertyReferences.Any())
      {
        throw new InvalidOperationException(
            string.Format("State property '{0}' cannot be deleted because it is associated with at least one securable class definition.", Name));
      }
      base.OnDeleting(args);

      //TODO: Rewrite with test
      _deleteHandler = new DomainObjectDeleteHandler(LocalizedNames);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted(args);

      //TODO: Rewrite with test
      _deleteHandler?.Delete();
    }

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException(string.Format(format, args), argumentName);
    }
  }
}
