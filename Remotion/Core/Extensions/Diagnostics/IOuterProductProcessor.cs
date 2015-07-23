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
  /// Interface to a class which can be passed to <see cref="OuterProductIndexGenerator.ProcessOuterProduct"/>.
  /// <see cref="OuterProductIndexGenerator.ProcessOuterProduct"/> will then call its methods <see cref="DoBeforeLoop"/>
  /// and <see cref="DoAfterLoop"/> before and after it starts iterating over a new outer product dimension.
  /// </summary>
  public interface IOuterProductProcessor {
    /// <summary>
    /// Processor callback invoked before a nested for loop starts.
    /// </summary>
    /// <returns><see langword="true" /> to continue looping, <see langword="false" /> to break from the current loop.</returns>
    //TODO: rename ProcessStateBeforeLoop?
    bool DoBeforeLoop ();
    /// <summary>
    /// Processor callback invoked after a nested for loop has finished.
    /// </summary>
    /// <returns><see langword="true" /> to continue looping, <see langword="false" /> to break from the current loop.</returns>
    //TODO: rename ProcessStateAfterLoop?
    bool DoAfterLoop ();
    /// <summary>
    /// Before each callback to the processor the OuterProductIndexGenerator class sets the current <see cref="OuterProductProcessingState"/> through a
    /// call to this method. The processor class is expected to store the <see cref="OuterProductProcessingState"/> to be able to access
    /// it during the callbacks.
    /// </summary>
    /// <param name="processingState"></param>
    void SetProcessingState (OuterProductProcessingState processingState);

    /// <summary>
    /// The current <see cref="OuterProductProcessingState"/> to be used during callbacks. Set by the OuterProductIndexGenerator class
    /// in call to <see cref="SetProcessingState"/>.
    /// </summary>
    OuterProductProcessingState ProcessingState { get; }
  }
}
