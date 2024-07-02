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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Handles commit validation for <see cref="ClientTransaction"/> instances.
  /// </summary>
  /// <remarks>
  /// Currently, this extension only checks that all mandatory relations are set.
  /// </remarks>
  public class CommitValidationClientTransactionExtension : ClientTransactionExtensionBase
  {
    public static string DefaultKey
    {
      get { return typeof(CommitValidationClientTransactionExtension).GetFullNameChecked(); }
    }

    private IPersistableDataValidator _validator;

    public CommitValidationClientTransactionExtension (IPersistableDataValidator validator)
      : this(validator, DefaultKey)
    {
    }

    protected CommitValidationClientTransactionExtension (IPersistableDataValidator validator, string key)
        : base(key)
    {
      ArgumentUtility.CheckNotNull("validator", validator);

      _validator = validator;
    }

    public IPersistableDataValidator Validator
    {
      get { return _validator; }
    }

    public override void CommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("committedData", committedData);

      foreach (var item in committedData)
        _validator.Validate(clientTransaction, item);
    }
  }
}
