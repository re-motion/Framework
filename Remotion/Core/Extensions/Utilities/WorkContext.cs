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
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Remotion.Utilities
{

/// <summary>
///   Provides context information for error messages.
/// </summary>
/// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Class/*' />
public class WorkContext: IDisposable
{
  public class ContextStack
  {
    /// <summary> ArrayList &lt;WorkContext&gt; </summary>
    private ArrayList _stack = new ArrayList();
    private WorkContext _lastLeft = null;

    /// <summary>
    ///   The last WorkContext on the stack that was left by calling <see cref="Leave"/>.
    /// </summary>
    public WorkContext LastLeft
    {
      get { return _lastLeft; }
    }

    /// <summary>
    ///   Returns the context stack as an array of WorkContext objects.
    /// </summary>
    /// <returns>
    ///   The items on the context thread, with the top-level stack items first.
    /// </returns>
    public WorkContext[] ToArray()
    {
      return (WorkContext[]) _stack.ToArray (typeof (WorkContext)); 
    }

    /// <summary>
    ///   Returns a string representation of the context stack.
    /// </summary>
    /// <returns>
    ///   An string with the description of each context on the stack on a separate line. Top-level contexts appear first.
    ///   Contexts that are already left, but not done, are marked with a question mark. See <see cref="Leave"/> and 
    ///   <see cref="Done"/>.
    /// </returns>
    public override string ToString()
    {
      bool pastLast = false;
      StringBuilder sb = new StringBuilder();
      foreach (WorkContext context in _stack)
      {
        if (context == _lastLeft)
          pastLast = true;
        if (sb.Length > 0)
          sb.Append ('\n');
        if (pastLast)
          sb.Append ("? ");
        sb.Append (context.Text);
      }
      return sb.ToString();
    }

    internal void Push (WorkContext context)
    {
      if (_lastLeft != null)
      {
        PopIncluding (_lastLeft);
        _lastLeft = null;
      }

      _stack.Add (context);
    }

    internal void PopIncluding (WorkContext context)
    {
      int contextIndex = -1;
      for (int i = _stack.Count - 1; i >= 0; --i)
      {
        if (_stack[i] == context)
        {
          contextIndex = i;
          break;
        }
      }
      if (contextIndex == -1)
        return;

      object[] newStack = new object[contextIndex];
      _stack.CopyTo (0, newStack, 0, contextIndex);
      _stack = new ArrayList (newStack);
    }

    internal void Leave (WorkContext context)
    {
      _lastLeft = context;
    }
  }

  // static members

  /// <summary> Stack&lt;WorkContext&gt; </summary>
  [ThreadStatic]
  private static ContextStack s_stack; // defaults to null for each new thread
  private static bool s_enableTracingFlagInitialized = false;
  private static bool s_enableTracing = false;
  private static object s_syncRoot = new object();

  /// <summary>
  /// Use this flag to specify (or learn) whether trace output should be generated when contexts are entered, left or done.
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/EnableTracing/*' />
  public static bool EnableTracing
  {
    get 
    {
      if (! s_enableTracingFlagInitialized)
      {
        lock (s_syncRoot)
        {
          if (! s_enableTracingFlagInitialized)
          {
            if (0 == string.Compare (ConfigurationManager.AppSettings["Remotion.WorkContext.EnableTracing"], "true", true, CultureInfo.InvariantCulture))
              s_enableTracing = true;
            s_enableTracingFlagInitialized = true;
          }
        }
      }
      return s_enableTracing;      
    }

    set
    {
      lock (s_syncRoot)
      {
        s_enableTracing = value;
        s_enableTracingFlagInitialized = true;
      }
    }
  }

  /// <summary>
  ///   Gets the work context stack of the current thread.
  /// </summary>
  /// <remarks>
  ///   The stack provides diagnostic information about the current execution context.
  /// </remarks>
  public static ContextStack Stack
  {
    get 
    {
      ContextStack stack = s_stack;
      if (stack == null)
      {
        stack = new ContextStack();
        s_stack = stack;
      }
      return stack;
    }    
  }

  /// <summary>
  /// Creates a new context and puts it on the stack.
  /// </summary>
  /// <param name="text">The description of the context.</param>
  public static WorkContext EnterNew (string text)
  {
    WorkContext context = new WorkContext ();
    context.Enter (text);
    return context;
  }

  /// <summary>
  /// Creates a new context and puts it on the stack.
  /// </summary>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  public static WorkContext EnterNew (string format, params object[] args)
  {
    return EnterNew (string.Format (format, args));
  }

  // member fields

  private string _text;
  private bool _entered = false;

  // construction and disposal

  public WorkContext ()
  {
  }

  // methods and properties

  /// <summary>
  /// Enters a context.
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Text/*' />
  void Enter (string text)
  {
    _text = text;
    _entered = true;
    if (EnableTracing)
    {
      Trace.WriteLine ("Enter Context: " + text);
      Trace.Indent();
    }
    Stack.Push (this);
  }

  /// <summary>
  /// Enters a context.
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Format_Args/*' />
  void Enter (string format, params object[] args)
  {
    Enter (string.Format (format, args));
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol DEBUG is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Text/*' />
  void EnterIfDebug (string text)
  {
    Enter (text);
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol DEBUG is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Format_Args/*' />
  void EnterIfDebug (string format, params object[] args)
  {
    Enter (format, args);
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol TRACE is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Text/*' />
  void EnterIfTrace (string text)
  {
    Enter (text);
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol TRACE is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Format_Args/*' />
  void EnterIfTrace (string format, params object[] args)
  {
    Enter (format, args);
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol WORKCONTEXT is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Text/*' />
  void EnterIfWorkContext (string text)
  {
    Enter (text);
  }

  /// <summary>
  /// Enters a context. Calls to this method are only compiled if the symbol WORKCONTEXT is defined.
  /// <see cref="ConditionalAttribute"/>
  /// </summary>
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Method/*' />
  /// <include file='..\doc\include\Utilities/WorkContext.xml' path='WorkContext/Enter/Signature_Format_Args/*' />
  void EnterIfWorkContext (string format, params object[] args)
  {
    Enter (format, args);
  }

  /// <summary>
  /// Leaves the context.
  /// <seealso cref="Leave"/>.
  /// </summary>
  void IDisposable.Dispose ()
  {
    Leave();
  }

  /// <summary>
  ///   Leaves the context.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   The context is left, but remains on the stack until the method <see cref="Done"/> is called, or a new context is 
  ///   entered. Therefore, the left context is still available for inspection (e.g. in an exception handling block).
  ///   </para><para>
  ///   A context that is left, but not done, is prefixed with a question mark in the context stack output. Use this 
  ///   information if you are not sure whether all calls to <see cref="Done"/> were coded correctly.
  ///   </para><para>
  ///   In C# it is generally recommended to use a <c>using</c> statement rather than calling <c>Leave</c> explicitly.
  ///   </para>
  /// </remarks>
  public void Leave ()
  {
    if (_entered)
    {
      if (EnableTracing)
      {
        Trace.Unindent();
        Trace.WriteLine ("Leave Context: " + _text);
      }
      Stack.Leave (this);
      _entered = false;
      }
  }

  /// <summary>
  ///   Marks the context as done.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Call this method if the work within a context has been processed successfully (i.e., no uncaught exception has been 
  ///     raised).
  ///   <para></para>
  ///     For C# users, it is recommended to call this method at the end of the <c>using</c> block that contains the context.
  ///   </para>
  /// </remarks>
  public void Done()
  {
    if (_entered)
    {
      if (EnableTracing)
        Trace.WriteLine ("Work done in Context: " + _text);
      Stack.PopIncluding (this);
    }
  }

  /// <summary>
  /// The description of the context.
  /// </summary>
  public string Text
  {
    get { return _text; }
    set { _text = value; }
  }
}

}
