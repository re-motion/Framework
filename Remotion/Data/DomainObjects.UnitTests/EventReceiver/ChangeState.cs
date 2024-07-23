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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class ChangeState
  {
    // types

    // static members and constants

    // member fields

    private object _sender;
    private string _message;

    // construction and disposing

    public ChangeState (object sender)
      : this(sender, null)
    {
    }

    public ChangeState (object sender, string message)
    {
      ArgumentUtility.CheckNotNull("sender", sender);

      _sender = sender;
      _message = message;
    }

    // methods and properties

    public object Sender
    {
      get { return _sender; }
    }

    public string Message
    {
      get { return _message; }
    }

    public virtual void Check (ChangeState expectedState)
    {
      ArgumentUtility.CheckNotNull("expectedState", expectedState);

      if (this.GetType() != expectedState.GetType())
      {
        throw CreateApplicationException(
            "Type of actual state '{0}' does not match type of expected state '{1}'.",
            this.GetType(),
            expectedState.GetType());
      }

      if (!ReferenceEquals(_sender, expectedState.Sender))
      {
        throw CreateApplicationException(
            "Actual sender '{0}' does not match expected sender '{1}'.", _sender, expectedState.Sender);
      }
    }

    [MustUseReturnValue]
    protected ApplicationException CreateApplicationException (string message, params object[] args)
    {
      return new ApplicationException(string.Format(message, args));
    }
  }
}
