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

namespace Remotion.Diagnostics
{
  /// <summary>
  /// Convenience class to derive OuterProductIndexGenerator-processors from. Already supplies ProcessingState-functionality,
  /// just requiring override implementation of <see cref="DoBeforeLoop"/> and <see cref="DoAfterLoop"/>.
  /// </summary>
  public class OuterProductProcessorBase : IOuterProductProcessor 
  { 
    private OuterProductProcessingState _processingState;
    /// <summary>
    /// The current <see cref="ProcessingState"/> to be used during callbacks.
    /// </summary>
    public OuterProductProcessingState ProcessingState
    {
      get { return _processingState; }
    }

    /// <summary>
    /// Default implementation for the callback before a new for loop starts. Simply keeps on looping.
    /// Override to implement your own functionality.
    /// </summary>
    /// <returns><see cref="IOuterProductProcessor.DoBeforeLoop"/></returns>
    public virtual bool DoBeforeLoop ()
    {
      return true;
    }

    /// <summary>
    /// Default implementation for the callback after a for loop finishes. Simply keeps on looping.
    /// Override to implement your own functionality.
    /// </summary>
    /// <returns><see cref="IOuterProductProcessor.DoAfterLoop"/></returns>
    public virtual bool DoAfterLoop ()
    {
      return true;
    }
      
    /// <summary>
    /// Internal use only: Used by OuterProductIndexGenerator class to set the current <see cref="ProcessingState"/> before invoking a callback.
    /// </summary>
    /// <param name="processingState"></param>
    public void SetProcessingState (OuterProductProcessingState processingState)
    {
      _processingState = processingState;
    } 
  }
}
