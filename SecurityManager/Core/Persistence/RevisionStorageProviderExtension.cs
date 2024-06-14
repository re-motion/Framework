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
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Reflection;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  public class RevisionStorageProviderExtension
  {
    //TODO RM-5521: rewrite with test

    private readonly IDomainRevisionProvider _revisionProvider;
    private readonly IUserRevisionProvider _userRevisionProvider;
    private readonly IUserNamesRevisionProvider _userNamesRevisionProvider;
    private readonly IRdbmsProviderCommandFactory _rdbmsProviderCommandFactory;
    private readonly PropertyDefinition _userNamePropertyDefinition;
    private readonly PropertyDefinition _substitutionUserPropertyDefinition;

    public RevisionStorageProviderExtension (
        IDomainRevisionProvider revisionProvider,
        IUserRevisionProvider userRevisionProvider,
        IUserNamesRevisionProvider userNamesRevisionProvider,
        IRdbmsProviderCommandFactory rdbmsProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull("userRevisionProvider", userRevisionProvider);
      ArgumentUtility.CheckNotNull("userNamesRevisionProvider", userNamesRevisionProvider);
      ArgumentUtility.CheckNotNull("rdbmsProviderCommandFactory", rdbmsProviderCommandFactory);

      _revisionProvider = revisionProvider;
      _userRevisionProvider = userRevisionProvider;
      _userNamesRevisionProvider = userNamesRevisionProvider;
      _rdbmsProviderCommandFactory = rdbmsProviderCommandFactory;

      _userNamePropertyDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(User))
          .GetMandatoryPropertyDefinition(GetPropertyIdentifierFromTypeAndShortName(typeof(User), "UserName"));

      _substitutionUserPropertyDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Substitution))
          .GetMandatoryPropertyDefinition(GetPropertyIdentifierFromTypeAndShortName(typeof(Substitution), "SubstitutingUser"));
    }

    public virtual void Saved (IRdbmsProviderReadWriteCommandExecutionContext executionContext, IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      var securityManagerDataContainers =
          dataContainers.Where(dataContainer => typeof(BaseSecurityManagerObject).IsAssignableFrom(dataContainer.DomainObjectType));

      bool isDomainRevisionInvalidated = false;
      bool isUserNamesRevisionInvalidated = false;
      var usersToInvalidate = new HashSet<IDomainObjectHandle<User>>();
      var loadedUsers = new Dictionary<IDomainObjectHandle<User>, DataContainer>();
      foreach (var dataContainer in securityManagerDataContainers)
      {
        if (!isDomainRevisionInvalidated && IsDomainRevisionRelevant(dataContainer))
        {
          var revisionKey = new RevisionKey();
          IncrementRevision(executionContext, revisionKey);
          _revisionProvider.InvalidateRevision(revisionKey);

          isDomainRevisionInvalidated = true;
        }

        if (!isUserNamesRevisionInvalidated && IsUserNamesRevisionRelevant(dataContainer))
        {
          IncrementRevision(executionContext, UserNamesRevisionKey.Global);
          _userNamesRevisionProvider.InvalidateRevision(UserNamesRevisionKey.Global);

          isUserNamesRevisionInvalidated = true;
        }

        if (IsUserRevisionRelevant(dataContainer))
        {
          var user = GetUserForInvalidation(dataContainer);
          if (user != null)
            usersToInvalidate.Add(user);
        }

        if (typeof(User).IsAssignableFrom(dataContainer.DomainObjectType))
          loadedUsers.Add(dataContainer.ID.GetHandle<User>(), dataContainer);
      }

      var userNamesToInvalidate = new List<string>();
      var notLoadedUsers = new List<IDomainObjectHandle<User>>();
      foreach (var userID in usersToInvalidate)
      {
        var userDataContainer = loadedUsers.GetValueOrDefault(userID);
        if (userDataContainer != null)
        {
          var userName = GetValue<string>(userDataContainer, _userNamePropertyDefinition);
          Assertion.IsNotNull(userName, "User{{{0}}}.UserName must not be null.", userDataContainer.ID);
          userNamesToInvalidate.Add(userName);

          // For changed user names, also invalidate the original revision to ensure that the old user name cannot be used with state cache values.
          var originalUserName = GetOriginalValueOrDefault<string>(userDataContainer, _userNamePropertyDefinition);
          if (originalUserName != null)
            userNamesToInvalidate.Add(originalUserName);
        }
        else
        {
          notLoadedUsers.Add(userID);
        }
      }

      userNamesToInvalidate.AddRange(LoadUserNames(executionContext, notLoadedUsers));

      foreach (var userName in userNamesToInvalidate)
      {
        var revisionKey = new UserRevisionKey(userName);
        IncrementRevision(executionContext, revisionKey);
        _userRevisionProvider.InvalidateRevision(revisionKey);
      }

      if (userNamesToInvalidate.Any())
      {
        IncrementRevision(executionContext, UserRevisionKey.Global);
        _userRevisionProvider.InvalidateRevision(UserRevisionKey.Global);
      }
    }

    private IEnumerable<string> LoadUserNames (IRdbmsProviderReadWriteCommandExecutionContext executionContext, ICollection<IDomainObjectHandle<User>> users)
    {
      if (!users.Any())
        yield break;

      //TODO RM-5702: Support for more than 2000 parameters
      var userIDs = users.Select(u => (Guid)u.ObjectID.Value).ToList();
      var query = QueryFactory.CreateQuery<string>(
          "load usernames",
          QueryFactory.CreateLinqQuery<User>()
              .Where(u => userIDs.Contains((Guid)u.ID.Value))
              .Select(u => u.UserName));

      var storageProviderCommand = _rdbmsProviderCommandFactory.CreateForCustomQuery(query);
      foreach (var queryResultRow in storageProviderCommand.Execute(executionContext))
      {
        var loadUserName = queryResultRow.GetConvertedValue<string>(0);
        Assertion.IsNotNull(loadUserName, "Expected database result cannot contain null values.");
        yield return loadUserName;
      }
    }

    private bool IsDomainRevisionRelevant (DataContainer dataContainer)
    {
      if (typeof(MetadataObject).IsAssignableFrom(dataContainer.DomainObjectType))
        return true;

      if (typeof(Position).IsAssignableFrom(dataContainer.DomainObjectType))
        return true;

      if (typeof(GroupType).IsAssignableFrom(dataContainer.DomainObjectType))
        return true;

      if (typeof(Group).IsAssignableFrom(dataContainer.DomainObjectType))
        return true; // Group.Parent, Group.GroupType, Group.UniqueIdentifier

      if (typeof(Tenant).IsAssignableFrom(dataContainer.DomainObjectType))
        return true; //Tenant.UniqueIdentifier, Tenant.Parent

      return false;
    }

    private bool IsUserNamesRevisionRelevant (DataContainer dataContainer)
    {
      if (typeof(User).IsAssignableFrom(dataContainer.DomainObjectType))
        return dataContainer.HasValueChanged(_userNamePropertyDefinition);

      return false;
    }

    private bool IsUserRevisionRelevant (DataContainer dataContainer)
    {
      if (typeof(User).IsAssignableFrom(dataContainer.DomainObjectType))
        return true; //user.Username, user.Roles

      if (typeof(Substitution).IsAssignableFrom(dataContainer.DomainObjectType))
        return true;

      return false;
    }

    private IDomainObjectHandle<User>? GetUserForInvalidation (DataContainer dataContainer)
    {
      if (typeof(User).IsAssignableFrom(dataContainer.DomainObjectType))
        return dataContainer.ID.GetHandle<User>();

      if (typeof(Substitution).IsAssignableFrom(dataContainer.DomainObjectType))
      {
        var objectID = GetValue<ObjectID>(dataContainer, _substitutionUserPropertyDefinition);
        if (objectID != null)
          return objectID.GetHandle<User>();
        else
          return null;
      }

      throw new ArgumentException(
          string.Format("DataContainer type can only be User or Substitution but was '{0}'.", dataContainer.DomainObjectType),
          "dataContainer");
    }

    private TResult? GetValue<TResult> (DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      var valueAccess = dataContainer.State.IsDeleted ? ValueAccess.Original : ValueAccess.Current;
      return (TResult?)dataContainer.GetValueWithoutEvents(propertyDefinition, valueAccess);
    }

    private TResult? GetOriginalValueOrDefault<TResult> (DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      if (!dataContainer.HasValueChanged(propertyDefinition))
        return default(TResult);

      if (!dataContainer.State.IsChanged)
        return default(TResult);

      return (TResult?)dataContainer.GetValueWithoutEvents(propertyDefinition, ValueAccess.Original);
    }

    private string GetPropertyIdentifierFromTypeAndShortName (Type domainObjectType, string shortPropertyName)
    {
      return domainObjectType.GetFullNameChecked() + "." + shortPropertyName;
    }

    private void IncrementRevision (IRdbmsProviderReadWriteCommandExecutionContext executionContext, IRevisionKey revisionKey)
    {
      var query = Revision.GetIncrementRevisionQuery(revisionKey);
      Assertion.IsTrue(query.QueryType == QueryType.ScalarReadWrite);
      var storageProviderCommand = _rdbmsProviderCommandFactory.CreateForScalarQuery(query);
      storageProviderCommand.Execute(executionContext);
    }
  }
}
