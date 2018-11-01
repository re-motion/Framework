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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Represents the result of an <see cref="IDataManagementCommand.ExpandToAllRelatedObjects"/> operation. Similar to a 
  /// <see cref="CompositeCommand"/>, but calling  <see cref="IDataManagementCommand.ExpandToAllRelatedObjects"/> again on this object will
  /// result in the same <see cref="ExpandedCommand"/> as before.
  /// </summary>
  public class ExpandedCommand : IDataManagementCommand
  {
    private readonly CompositeCommand _compositeCommand;

    public ExpandedCommand (IEnumerable<IDataManagementCommand> commands)
      : this (new CompositeCommand (commands))
    {
    }

    public ExpandedCommand (params IDataManagementCommand[] commands)
        : this ((IEnumerable<IDataManagementCommand>) commands)
    {
    }

    private ExpandedCommand (CompositeCommand compositeCommand)
    {
      ArgumentUtility.CheckNotNull ("compositeCommand", compositeCommand);

      _compositeCommand = compositeCommand;
    }

    public ReadOnlyCollection<IDataManagementCommand> GetNestedCommands ()
    {
      return _compositeCommand.GetNestedCommands();
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _compositeCommand.GetAllExceptions();
    }

    public void Perform ()
    {
      _compositeCommand.Perform();
    }

    public void Begin ()
    {
      _compositeCommand.Begin();
    }

    public void End ()
    {
      _compositeCommand.End();
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return this;
    }

    public ExpandedCommand CombineWith (IEnumerable<IDataManagementCommand> commands)
    {
      return new ExpandedCommand (_compositeCommand.CombineWith (commands));
    }

    public ExpandedCommand CombineWith (params IDataManagementCommand[] commands)
    {
      return CombineWith ((IEnumerable<IDataManagementCommand>) commands);
    }
  }
}