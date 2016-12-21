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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Acts as a base class for classes decorating <see cref="IDataManagementCommand"/> instances.
  /// </summary>
  public abstract class DataManagementCommandDecoratorBase : IDataManagementCommand
  {
    private readonly IDataManagementCommand _decoratedCommand;

    protected DataManagementCommandDecoratorBase (IDataManagementCommand decoratedCommand)
    {
      ArgumentUtility.CheckNotNull ("decoratedCommand", decoratedCommand);
      _decoratedCommand = decoratedCommand;
    }

    public IDataManagementCommand DecoratedCommand
    {
      get { return _decoratedCommand; }
    }

    public virtual IEnumerable<Exception> GetAllExceptions ()
    {
      return _decoratedCommand.GetAllExceptions();
    }

    public virtual void Begin ()
    {
      _decoratedCommand.Begin();
    }

    public virtual void Perform ()
    {
      _decoratedCommand.Perform ();
    }

    public void End ()
    {
      _decoratedCommand.End();
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var expandedCommand = _decoratedCommand.ExpandToAllRelatedObjects();
      return new ExpandedCommand (Decorate (expandedCommand));
    }

    protected abstract IDataManagementCommand Decorate (IDataManagementCommand decoratedCommand);
  }
}