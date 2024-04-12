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
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Defines how an <see cref="IDbDataParameter"/> is created from an <see cref="object"/> that serves as the <see cref="IDataParameter.Value"/>. 
/// </summary>
public interface IDataParameterDefinition
{
  /// <summary>
  /// Gets the storage-type <see cref="IDataParameter.Value"/> from the given <paramref name="value"/>.
  /// </summary>
  object GetParameterValue (object? value);

  /// <summary>
  /// Creates an <see cref="IDbDataParameter"/> with <paramref name="parameterValue"/> as the <see cref="IDataParameter.Value"/>.
  /// </summary>
  /// <remarks>
  /// The <paramref name="parameterValue"/> is built from <see cref="GetParameterValue"/>.
  /// </remarks>
  IDbDataParameter CreateDataParameter (IDbCommand command, string parameterName, object parameterValue);
}
