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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlList : AccessControlObject
  {
    private DomainObjectDeleteHandler _deleteHandler;

    protected AccessControlList ()
    {
    }

    [StorageClassNone]
    public abstract SecurableClassDefinition Class { get; }

    [DBBidirectionalRelation ("AccessControlList", SortExpression = "Index ASC")]
    public abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    /// <summary>
    /// Returns the ACEs which match the passed <see cref="SecurityToken"/>
    /// </summary>
    /// <param name="token">The security token that will be matched against the ACL entries. Must not be <see langword="null" />.</param>
    /// <returns>array of ACEs</returns>
    public AccessControlEntry[] FindMatchingEntries (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      var entries = new List<AccessControlEntry>();

      foreach (var entry in AccessControlEntries)
      {
        if (entry.MatchesToken (token))
          entries.Add (entry);
      }

      return entries.ToArray();
    }

    public AccessInformation GetAccessTypes (SecurityToken token, AccessTypeStatistics accessTypeStatistics)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      var allowedAccessTypesResult = new HashSet<AccessTypeDefinition> ();
      var deniedAccessTypesResult = new HashSet<AccessTypeDefinition> ();

      foreach (var ace in FindMatchingEntries (token))
      {
        var allowedAccesTypesForCurrentAce = ace.GetAllowedAccessTypes();
        var deniedAccessTypesForCurrentAce = ace.GetDeniedAccessTypes ();

        // Add allowed/denied access types of ACE to result
        allowedAccessTypesResult.UnionWith (allowedAccesTypesForCurrentAce);
        deniedAccessTypesResult.UnionWith (deniedAccessTypesForCurrentAce);

        // Record the ACEs that contribute to the resulting AccessTypeDefinition-array.
        // The recorded information allows deduction of whether the probing ACE was matched for ACL-expansion code
        // (see AclExpander.AddAclExpansionEntry).
        if (accessTypeStatistics != null)
        {
          accessTypeStatistics.AddMatchingAce (ace);
          if (allowedAccesTypesForCurrentAce.Length > 0 || deniedAccessTypesForCurrentAce.Length > 0)
            accessTypeStatistics.AddAccessTypesContributingAce (ace);
        }
      }

      // Deny always wins => Remove allowed access types which are also denied from result.
      foreach (var deniedAccessType in deniedAccessTypesResult)
        allowedAccessTypesResult.Remove (deniedAccessType);

      return new AccessInformation (allowedAccessTypesResult.ToArray (), deniedAccessTypesResult.ToArray ());
    }

    public AccessInformation GetAccessTypes (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);
      return GetAccessTypes (token, null);
    }

    protected override void OnRelationChanged (RelationChangedEventArgs args)
    {
      base.OnRelationChanged (args);
      if (args.IsRelation (this, "AccessControlEntries"))
        HandleAccessControlEntriesChanged ((AccessControlEntry) args.NewRelatedObject);
    }

    private void HandleAccessControlEntriesChanged (AccessControlEntry ace)
    {
      if (ace != null)
        ace.Index = AccessControlEntries.IndexOf (ace);
    }

    protected override void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      base.OnCommitting (args);

      if (Class != null)
        Class.RegisterForCommit();
    }

    //TODO: Rewrite with test
    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (AccessControlEntries);
    }

    //TODO: Rewrite with test
    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }

    public AccessControlEntry CreateAccessControlEntry ()
    {
      if (Class == null)
        throw new InvalidOperationException ("Cannot create AccessControlEntry if no SecurableClassDefinition is assigned to this AccessControlList.");

      var accessControlEntry = AccessControlEntry.NewObject();
      foreach (var accessTypeDefinition in Class.AccessTypes)
        accessControlEntry.AddAccessType (accessTypeDefinition);
      accessControlEntry.AccessControlList = this;

      return accessControlEntry;
    }
  }
}
