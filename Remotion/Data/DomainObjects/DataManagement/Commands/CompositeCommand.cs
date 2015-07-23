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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Composes several <see cref="IDataManagementCommand"/> instances into a single command.
  /// </summary>
  /// <remarks>
  /// This can, for example, be used to model bidirectional relation modifications. Such modifications always comprise multiple steps: they need to 
  /// be performed on either side of the relation being changed, and usually they also invole one "previous" or "new" related object. (Eg. an insert 
  /// modificaton has a previous related object (possibly <see langword="null" />), a remove modification has an old related object.)
  /// <see cref="CompositeCommand"/> aggregates these modification steps and allows executing and raising events for them all at once.
  /// </remarks>
  public class CompositeCommand : IDataManagementCommand
  {
    private readonly ReadOnlyCollection<IDataManagementCommand> _commands;
    private readonly ReadOnlyCollection<Exception> _exceptions;

    public CompositeCommand (IEnumerable<IDataManagementCommand> commands)
    {
      ArgumentUtility.CheckNotNull ("commands", commands);

      // Manual iteration instead of List ctor + LINQ to avoid having to iterate the commands twice
      var commandList = new List<IDataManagementCommand> ();
      var exceptionList = new List<Exception>();
      foreach (var command in commands)
      {
        commandList.Add (command);
        var exceptions = Assertion.IsNotNull (command.GetAllExceptions(), "GetAllExceptions must return a non-null sequence.");
        exceptionList.AddRange (exceptions);
      }
      _commands = commandList.AsReadOnly();
      _exceptions = exceptionList.AsReadOnly();
    }

    public CompositeCommand (params IDataManagementCommand[] commands)
        : this ((IEnumerable<IDataManagementCommand>) commands)
    {
    }

    public ReadOnlyCollection<IDataManagementCommand> GetNestedCommands ()
    {
      return _commands;
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _exceptions;
    }

    public void Perform ()
    {
      this.EnsureCanExecute ();

      foreach (var command in _commands)
        command.Perform ();
    }

    public void Begin ()
    {
      this.EnsureCanExecute ();

      foreach (var command in _commands)
        command.Begin ();
    }

    public void End ()
    {
      this.EnsureCanExecute ();

      for (int i = _commands.Count - 1; i >= 0; i--)
        _commands[i].End ();
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (_commands.SelectMany (nestedCommand => nestedCommand.ExpandToAllRelatedObjects().GetNestedCommands()));
    }

    public CompositeCommand CombineWith (IEnumerable<IDataManagementCommand> commands)
    {
      return new CompositeCommand (_commands.Concat (commands));
    }

    public CompositeCommand CombineWith (params IDataManagementCommand[] commands)
    {
      return CombineWith ((IEnumerable<IDataManagementCommand>) commands);
    }
  }
}