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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="Script"/> holds a setup- and teardown script for a relational database.
  /// </summary>
  public class Script
  {
    private readonly RdbmsProviderDefinition _storageProviderDefinition;
    private readonly string _setUpScript;
    private readonly string _tearDownScript;

    public Script (RdbmsProviderDefinition storageProviderDefinition, string setUpScript, string tearDownScript)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("setUpScript", setUpScript);
      ArgumentUtility.CheckNotNull ("tearDownScript", tearDownScript);

      _storageProviderDefinition = storageProviderDefinition;
      _setUpScript = setUpScript;
      _tearDownScript = tearDownScript;
    }

    public RdbmsProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string SetUpScript
    {
      get { return _setUpScript; }
    }

    public string TearDownScript
    {
      get { return _tearDownScript; }
    }
  }
}