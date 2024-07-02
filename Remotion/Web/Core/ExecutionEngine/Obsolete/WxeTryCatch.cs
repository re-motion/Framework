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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine.Obsolete
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class WxeExceptionAttribute : Attribute
  {
    public static WxeExceptionAttribute? GetAttribute (Type type)
    {
      WxeExceptionAttribute[] attributes = (WxeExceptionAttribute[])type.GetCustomAttributes(typeof(WxeExceptionAttribute), false);
      if (attributes == null || attributes.Length == 0)
        return null;
      return attributes[0];
    }

    private Type _exceptionType;

    public WxeExceptionAttribute (Type exceptionType)
    {
      _exceptionType = exceptionType;
    }

    public Type ExceptionType
    {
      get { return _exceptionType; }
    }
  }

  /// <summary>
  ///   Try-Catch block.
  /// </summary>
  public class WxeTryCatch : WxeStep
  {
    /// <summary>
    /// index of currently executing catch block, or -1 if the try block is executing, -2 if finally block is executing
    /// </summary>
    private int _executingCatchBlock = -1;

    private WxeStepList _trySteps;

    private List<WxeCatchBlock> _catchBlocks;

    private WxeStepList? _finallySteps;

    public WxeTryCatch (Type tryStepListType, Type? finallyStepListType, params Type[]? catchBlockTypes)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("tryStepListType", tryStepListType, typeof(WxeStepList));
      ArgumentUtility.CheckTypeIsAssignableFrom("finallyStepListType", finallyStepListType, typeof(WxeStepList));

      _trySteps = (WxeStepList)Activator.CreateInstance(tryStepListType)!;
      _trySteps.SetParentStep(this);

      _catchBlocks = new List<WxeCatchBlock>();
      if (catchBlockTypes != null)
      {
        foreach (Type catchBlockType in catchBlockTypes)
          Add((WxeCatchBlock)Activator.CreateInstance(catchBlockType)!);
      }

      if (finallyStepListType != null)
      {
        _finallySteps = (WxeStepList)Activator.CreateInstance(finallyStepListType)!; // TODO RM-8118: Add debug assertion
        _finallySteps.SetParentStep(this);
      }
      else
        _finallySteps = null;
    }

    public WxeTryCatch ()
    {
      InitializeSteps();
    }

    [MemberNotNull(nameof(_trySteps))]
    [MemberNotNull(nameof(_catchBlocks))]
    private void InitializeSteps ()
    {
      Type type = this.GetType();

      Type? tryBlockType = type.GetNestedType("Try", BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
      if (tryBlockType == null)
        throw new WxeException("Try/catch block type " + type.FullName + " has no nested type named \"Try\".");
      if (! typeof(WxeStepList).IsAssignableFrom(tryBlockType))
        throw new WxeException("Type " + tryBlockType.FullName + " must be derived from WxeTryBlock.");
      _trySteps = (WxeStepList)Activator.CreateInstance(tryBlockType)!;
      _trySteps.SetParentStep(this);

      Type? finallyBlockType = type.GetNestedType("Finally", BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
      if (finallyBlockType != null)
      {
        if (! typeof(WxeStepList).IsAssignableFrom(finallyBlockType))
          throw new WxeException("Type " + finallyBlockType.FullName + " must be derived from WxeFinallyBlock.");
        _finallySteps = (WxeStepList)Activator.CreateInstance(finallyBlockType)!;
        _finallySteps.SetParentStep(this);
      }
      else
        _finallySteps = null;

      MemberInfo[] catchBlockTypes = NumberedMemberFinder.FindMembers(
          type,
          "Catch",
          MemberTypes.NestedType,
          BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

      _catchBlocks = new List<WxeCatchBlock>();
      foreach (Type catchBlockType in catchBlockTypes)
      {
        if (! typeof(WxeCatchBlock).IsAssignableFrom(catchBlockType))
          throw new WxeException("Type " + catchBlockType.FullName + " must be derived from WxeCatchBlock.");
        Add((WxeCatchBlock)Activator.CreateInstance(catchBlockType)!);
      }
    }

    public override void Execute (WxeContext context)
    {
      if (_executingCatchBlock == -1) // tryBlock
      {
        try
        {
          _trySteps.Execute(context);
        }
        catch (Exception ex)
        {
          if (ex is System.Threading.ThreadAbortException)
            throw;

          Exception unwrappedException = WxeHttpExceptionPreservingException.GetUnwrappedException(ex) ?? ex;

          for (int i = 0; i < _catchBlocks.Count; ++i)
          {
            WxeCatchBlock catchBlock = (WxeCatchBlock)_catchBlocks[i]!;
            if (catchBlock.ExceptionType.IsInstanceOfType(ex))
            {
              _executingCatchBlock = i;
              catchBlock.Exception = ex;
              break;
            }
            else if (catchBlock.ExceptionType.IsInstanceOfType(unwrappedException))
            {
              _executingCatchBlock = i;
              catchBlock.Exception = unwrappedException;
              break;
            }
          }

          if (_executingCatchBlock == -1)
            throw;

          ExecutingStepList!.Execute(context);
        }

        if (_finallySteps != null)
        {
          _executingCatchBlock = -2;
          ExecutingStepList!.Execute(context);
        }
      }
      else if (_executingCatchBlock == -2) // finallyBlock
        _finallySteps!.Execute(context);
      else
      {
        ExecutingStepList!.Execute(context);
        if (_finallySteps != null)
        {
          _executingCatchBlock = -2;
          _finallySteps.Execute(context);
        }
      }
    }

    public override WxeStep ExecutingStep
    {
      get { return ExecutingStepList!.ExecutingStep; }  // TODO RM-8118: Add not null assertion
    }

    private WxeStepList? ExecutingStepList
    {
      get
      {
        switch (_executingCatchBlock)
        {
          case -1:
            return _trySteps;
          case -2:
            return _finallySteps;
          default:
            return _catchBlocks[_executingCatchBlock]!;
        }
      }
    }

    public void Add (WxeCatchBlock catchBlock)
    {
      _catchBlocks.Add(catchBlock);
      catchBlock.SetParentStep(this);
    }

    public WxeStepList TrySteps
    {
      get { return _trySteps; }
    }

    public WxeCatchBlock[] CatchBlocks
    {
      get { return _catchBlocks.ToArray(); }
    }

    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      _trySteps.Abort();

      foreach (WxeStepList catchBlock in _catchBlocks)
        catchBlock.Abort();

      if (_finallySteps != null)
        _finallySteps.Abort();
    }

    public override void ResetDirtyStateForExecutedSteps ()
    {
      base.ResetDirtyStateForExecutedSteps();

      _trySteps.ResetDirtyStateForExecutedSteps();

      foreach (var catchBlock in _catchBlocks)
        catchBlock.ResetDirtyStateForExecutedSteps();

      _finallySteps?.ResetDirtyStateForExecutedSteps();
    }

    public override bool EvaluateDirtyState ()
    {
      if (_trySteps.EvaluateDirtyState())
        return true;

      if (_catchBlocks.Any(b => b.EvaluateDirtyState()))
        return true;

      if (_finallySteps != null && _finallySteps.EvaluateDirtyState())
        return true;

      return base.EvaluateDirtyState();
    }

    public sealed override bool IsDirtyStateEnabled => base.IsDirtyStateEnabled;
  }

  public class WxeCatchBlock : WxeStepList
  {
    private Exception? _exception = null;

    public Exception? Exception
    {
      get { return _exception; }
      set { _exception = value; }
    }

    public virtual Type ExceptionType
    {
      get
      {
        WxeExceptionAttribute? exceptionAttribute = WxeExceptionAttribute.GetAttribute(this.GetType());
        if (exceptionAttribute == null)
          return typeof(Exception);
        else
          return exceptionAttribute.ExceptionType;
      }
    }

    public sealed override bool IsDirtyStateEnabled => base.IsDirtyStateEnabled;
  }
}
