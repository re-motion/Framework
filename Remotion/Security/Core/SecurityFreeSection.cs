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
using Remotion.Context;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a scope within no security will be evaluated. Use <see cref="SecurityFreeSection"/>.<see cref="Activate"/> to enter the scope 
  /// and <see cref="Scope.Dispose"/> to leave the scope.
  /// </summary>
  public static class SecurityFreeSection
  {
    /// <summary>
    /// The <see cref="Scope"/> struct can be used to mark a section of code where no security will be evaluated. 
    /// The section should be exited by invoking the <see cref="Dispose"/> method.
    /// </summary>
    [CannotApplyEqualityOperator]
    public struct Scope : IDisposable
    {
      private readonly int _currentSectionCount;
      private readonly bool _previousIsActive;
      private bool _isDisposed;

      internal Scope (int currentSectionCount, bool previousIsActive)
      {
        Assertion.DebugAssert (currentSectionCount > 0);

        _currentSectionCount = currentSectionCount;
        _previousIsActive = previousIsActive;
        _isDisposed = false;
      }

      /// <summary>
      /// Exits the <see cref="SecurityFreeSection"/> <see cref="Scope"/>.
      /// </summary>
      /// <exception cref="InvalidOperationException">
      /// The <see cref="Scope"/> was not exited in the correct nesting order.
      /// <para>- or -</para>
      /// The <see cref="Scope"/> was initialized using the value type's default construtor.
      /// </exception>
      public void Dispose ()
      {
        if (!_isDisposed)
        {
          if (_currentSectionCount == 0)
            throw new InvalidOperationException ("The SecurityFreeSection scope has not been entered by invoking SecurityFreeSection.Create().");

          PopSectionState (_currentSectionCount, _previousIsActive);
          _isDisposed = true;
        }
      }

      [Obsolete ("Use Dispose() instead. (Version 1.15.21.0)")]
      public void Leave ()
      {
        Dispose();
      }
    }

    private struct PushResult
    {
      private readonly int _currentSectionCount;
      private readonly bool _previousIsActive;

      public PushResult (int currentSectionCount, bool previousIsActive)
      {
        _currentSectionCount = currentSectionCount;
        _previousIsActive = previousIsActive;
      }

      public int CurrentSectionCount
      {
        get { return _currentSectionCount; }
      }

      public bool PreviousIsActive
      {
        get { return _previousIsActive; }
      }
    }

    private class State
    {
      // Mutable to avoid object allocation during increment and decrement
      public int CurrentSectionCount { get; set; }

      // Mutable to avoid object allocation during increment and decrement
      public bool IsActive { get; set; }

      public State ()
      {
        CurrentSectionCount = 0;
        IsActive = false;
      }
    }

    private static readonly SafeContextSingleton<State> s_sectionState =
        new SafeContextSingleton<State> (SafeContextKeys.SecuritySecurityFreeSection, () => new State());

    /// <summary>
    /// Enables a <see cref="SecurityFreeSection"/> until the <see cref="Scope"/> is disabled. 
    /// The <see cref="Scope"/> should always be used via a using-block, or if that is not possible, disposed inside a finally-block.
    /// </summary>
    public static Scope Activate ()
    {
      var result = PushSectionState (newIsActive: true);
      return new Scope (result.CurrentSectionCount, result.PreviousIsActive);
    }
    
    /// <summary>
    /// Disables any active <see cref="SecurityFreeSection"/> until the <see cref="Scope"/> is disposed. 
    /// The <see cref="Scope"/> should always be used via a using-block, or if that is not possible, disposed inside a finally-block.
    /// </summary>
    public static Scope Deactivate ()
    {
      var result = PushSectionState (newIsActive: false);
      return new Scope (result.CurrentSectionCount, result.PreviousIsActive);
    }

    /// <summary>
    /// Enters a new <see cref="SecurityFreeSection"/> <see cref="Scope"/>. 
    /// The <see cref="Scope"/> should always be used via a using-block, or if that is not possible, disposed inside a finally-block.
    /// </summary>
    [Obsolete ("Use Activate() instead. (Version 1.15.26.0)")]
    public static Scope Create ()
    {
      return Activate();
    }

    public static bool IsActive
    {
      get
      {
        var sectionState = GetSectionState();
        return sectionState.IsActive;
      }
    }

    private static State GetSectionState ()
    {
      return s_sectionState.Current;
    }

    private static PushResult PushSectionState (bool newIsActive)
    {
      var sectionState = GetSectionState();

      var newSectionCount = sectionState.CurrentSectionCount + 1;
      var previousIsActive = sectionState.IsActive;

      sectionState.CurrentSectionCount = newSectionCount;
      sectionState.IsActive = newIsActive;

      return new PushResult (newSectionCount, previousIsActive);
    }

    private static void PopSectionState (int numberOfSectionsExpected, bool previousIsActive)
    {
      var sectionState = GetSectionState();

      if (sectionState.CurrentSectionCount != numberOfSectionsExpected)
      {
        throw new InvalidOperationException (
            "Nested SecurityFreeSection scopes have been exited out-of-sequence. "
            + "Entering a SecurityFreeSection should always be combined with a using-block for the scope, or if this is not possible, a finally-block for leaving the scope.");
      }

      sectionState.CurrentSectionCount--;
      sectionState.IsActive = previousIsActive;
    }
  }
}