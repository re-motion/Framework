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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides extension methods for <see cref="IDataManagementCommand"/>.
  /// </summary>
  public static class DataManagementCommandExtensions
  {
    /// <summary>
    /// Raises all events and performs the action of the given <see cref="IDataManagementCommand"/>.
    /// The order of events is as follows: <see cref="IDataManagementCommand.Begin"/>,
    /// <see cref="IDataManagementCommand.Begin"/>, <see cref="IDataManagementCommand.Perform"/>, 
    /// <see cref="IDataManagementCommand.End"/>, <see cref="IDataManagementCommand.End"/>.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    public static void NotifyAndPerform (this IDataManagementCommand command)
    {
      ArgumentUtility.CheckNotNull ("command", command);

      command.Begin ();
      command.Perform ();
      command.End ();
    }

    /// <summary>
    /// Gets a value indicating whether this command can be executed (i.e., it has no associated exceptions).
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance command can be execute; otherwise, <see langword="false"/>.
    /// </value>
    public static bool CanExecute (this IDataManagementCommand command)
    {
      return !command.GetAllExceptions().Any();
    }

    /// <summary>
    /// Ensures the given command can be executed (i.e., it has no associated exceptions). If it can't, this method throws the first of the command's
    /// associated exceptions
    /// </summary>
    public static void EnsureCanExecute (this IDataManagementCommand command)
    {
      var exception = command.GetAllExceptions().FirstOrDefault();
      if (exception != null)
        throw exception;
    }
  }
}