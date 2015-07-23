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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  public class SecurableClassDefinitionData
  {
    private readonly string _baseClass;
    private readonly IDomainObjectHandle<StatelessAccessControlList> _statelessAccessControlList;
    private readonly ReadOnlyCollectionDecorator<StatefulAccessControlListData> _statefulAccessControlLists;

    public SecurableClassDefinitionData (
        [CanBeNull] string baseClass,
        [CanBeNull] IDomainObjectHandle<StatelessAccessControlList> statelessAccessControlList,
        IEnumerable<StatefulAccessControlListData> statefulAccessControlLists)
    {
      ArgumentUtility.CheckNotNull ("statefulAccessControlLists", statefulAccessControlLists);

      _baseClass = baseClass;
      _statelessAccessControlList = statelessAccessControlList;
      _statefulAccessControlLists = statefulAccessControlLists.ToArray().AsReadOnly();
    }

    [CanBeNull]
    public string BaseClass
    {
      get { return _baseClass; }
    }

    [CanBeNull]
    public IDomainObjectHandle<StatelessAccessControlList> StatelessAccessControlList
    {
      get { return _statelessAccessControlList; }
    }

    public ReadOnlyCollectionDecorator<StatefulAccessControlListData> StatefulAccessControlLists
    {
      get { return _statefulAccessControlLists; }
    }
  }
}