// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
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
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;
    private readonly PropertyDefinition _userNamePropertyDefinition;
    private readonly PropertyDefinition _substitutionUserPropertyDefinition;

    public RevisionStorageProviderExtension (
        IDomainRevisionProvider revisionProvider,
        IUserRevisionProvider userRevisionProvider,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull ("userRevisionProvider", userRevisionProvider);
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);

      _revisionProvider = revisionProvider;
      _userRevisionProvider = userRevisionProvider;
      _storageProviderCommandFactory = storageProviderCommandFactory;

      _userNamePropertyDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (User))
          .GetPropertyDefinition (GetPropertyIdentifierFromTypeAndShortName (typeof (User), "UserName"));

      _substitutionUserPropertyDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Substitution))
          .GetPropertyDefinition (GetPropertyIdentifierFromTypeAndShortName (typeof (Substitution), "SubstitutingUser"));
    }

    public virtual void Saved (IRdbmsProviderCommandExecutionContext executionContext, IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      var securityManagerDataContainers =
          dataContainers.Where (dataContainer => typeof (BaseSecurityManagerObject).IsAssignableFrom (dataContainer.DomainObjectType));

      bool isDomainRevisionInvalidated = false;
      var usersToInvalidate = new HashSet<IDomainObjectHandle<User>>();
      var loadedUsers = new Dictionary<IDomainObjectHandle<User>, DataContainer>();
      foreach (var dataContainer in securityManagerDataContainers)
      {
        if (!isDomainRevisionInvalidated && IsDomainRevisionRelevant (dataContainer))
        {
          var revisionKey = new RevisionKey();
          IncrementRevision (executionContext, revisionKey);
          _revisionProvider.InvalidateRevision (revisionKey);

          isDomainRevisionInvalidated = true;
        }

        if (IsUserRevisionRelevant (dataContainer))
        {
          var user = GetUserForInvalidation (dataContainer);
          if (user != null)
            usersToInvalidate.Add (user);
        }

        if (typeof (User).IsAssignableFrom (dataContainer.DomainObjectType))
          loadedUsers.Add (dataContainer.ID.GetHandle<User>(), dataContainer);
      }

      var userNamesToInvalidate = new List<string>();
      var notLoadedUsers = new List<IDomainObjectHandle<User>>();
      foreach (var userID in usersToInvalidate)
      {
        var userDataContainer = loadedUsers.GetValueOrDefault (userID);
        if (userDataContainer != null)
          userNamesToInvalidate.Add (GetValue<string> (userDataContainer, _userNamePropertyDefinition));
        else
          notLoadedUsers.Add (userID);
      }

      userNamesToInvalidate.AddRange (LoadUserNames (executionContext, notLoadedUsers));

      foreach (var userName in userNamesToInvalidate)
      {
        var revisionKey = new UserRevisionKey (userName);
        IncrementRevision (executionContext, revisionKey);
        _userRevisionProvider.InvalidateRevision (revisionKey);
      }

      if (userNamesToInvalidate.Any())
      {
        IncrementRevision (executionContext, UserRevisionKey.Global);
        _userRevisionProvider.InvalidateRevision (UserRevisionKey.Global);
      }
    }

    private IEnumerable<string> LoadUserNames (IRdbmsProviderCommandExecutionContext executionContext, ICollection<IDomainObjectHandle<User>> users)
    {
      if (!users.Any())
        yield break;

      //TODO RM-5702: Support for more than 2000 parameters
      var userIDs = users.Select (u => (Guid) u.ObjectID.Value).ToList();
      var query = QueryFactory.CreateQuery<string> (
          "load usernames",
          QueryFactory.CreateLinqQuery<User>()
              .Where (u => userIDs.Contains ((Guid) u.ID.Value))
              .Select (u => u.UserName));

      var storageProviderCommand = _storageProviderCommandFactory.CreateForCustomQuery (query);
      foreach (var queryResultRow in storageProviderCommand.Execute (executionContext))
        yield return queryResultRow.GetConvertedValue<string> (0);
    }

    private bool IsDomainRevisionRelevant (DataContainer dataContainer)
    {
      if (typeof (MetadataObject).IsAssignableFrom (dataContainer.DomainObjectType))
        return true;

      if (typeof (Position).IsAssignableFrom (dataContainer.DomainObjectType))
        return true;

      if (typeof (GroupType).IsAssignableFrom (dataContainer.DomainObjectType))
        return true;

      if (typeof (Group).IsAssignableFrom (dataContainer.DomainObjectType))
        return true; // Group.Parent, Group.GroupType, Group.UniqueIdentifier

      if (typeof (Tenant).IsAssignableFrom (dataContainer.DomainObjectType))
        return true; //Tenant.UniqueIdentifier, Tenant.Parent

      return false;
    }

    private bool IsUserRevisionRelevant (DataContainer dataContainer)
    {
      if (typeof (User).IsAssignableFrom (dataContainer.DomainObjectType))
        return true; //user.Username

      if (typeof (Substitution).IsAssignableFrom (dataContainer.DomainObjectType))
        return true;

      return false;
    }

    private IDomainObjectHandle<User> GetUserForInvalidation (DataContainer dataContainer)
    {
      if (typeof (User).IsAssignableFrom (dataContainer.DomainObjectType))
        return dataContainer.ID.GetHandle<User>();

      if (typeof (Substitution).IsAssignableFrom (dataContainer.DomainObjectType))
      {
        var objectID = GetValue<ObjectID> (dataContainer, _substitutionUserPropertyDefinition);
        if (objectID != null)
          return objectID.GetHandle<User>();
        else
          return null;
      }

      throw new ArgumentException (
          string.Format ("DataContainer type can only be User or Substitution but was '{0}'.", dataContainer.DomainObjectType),
          "dataContainer");
    }

    private TResult GetValue<TResult> (DataContainer dataContainer, PropertyDefinition propertyDefinition)
    {
      var valueAccess = dataContainer.State != StateType.Deleted ? ValueAccess.Current : ValueAccess.Original;
      return (TResult) dataContainer.GetValueWithoutEvents (propertyDefinition, valueAccess);
    }

    private string GetPropertyIdentifierFromTypeAndShortName (Type domainObjectType, string shortPropertyName)
    {
      return domainObjectType.FullName + "." + shortPropertyName;
    }

    private void IncrementRevision (IRdbmsProviderCommandExecutionContext executionContext, IRevisionKey revisionKey)
    {
      var query = Revision.GetIncrementRevisionQuery (revisionKey);
      Assertion.IsTrue (query.QueryType == QueryType.Scalar);
      var storageProviderCommand = _storageProviderCommandFactory.CreateForScalarQuery (query);
      storageProviderCommand.Execute (executionContext);
    }
  }
}