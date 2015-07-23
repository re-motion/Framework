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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting;

namespace Remotion.Scripting.UnitTests
{
  public class ScriptingHelper
  {
    public static MethodInfo GetInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public static MethodInfo GetInstanceMethod (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }

    public static MethodInfo[] GetAnyInstanceMethodArray (Type type, string name)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.Name == name)).ToArray ();
    }



    public static MethodInfo[] GetAnyPublicInstanceMethodArray (Type type, string name)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public).Where (
        mi => (mi.Name == name)).ToArray ();
    } 

    public static MethodInfo GetAnyGenericInstanceMethod (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).Single ();
    }

    public static MethodInfo[] GetAnyGenericInstanceMethodArray (Type type, string name, int numberGenericParameters)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (
        mi => (mi.IsGenericMethodDefinition && mi.Name == name && mi.GetGenericArguments ().Length == numberGenericParameters)).ToArray ();
    }


    public static MethodInfo[] GetAnyExplicitInterfaceMethodArray (Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.NonPublic).Where (
        mi => (mi.Name.EndsWith (name) && mi.GetParameters ().Select (pi => pi.ParameterType).SequenceEqual (argumentTypes))).ToArray ();
    }

    public static MethodInfo GetExplicitInterfaceMethod (Type interfaceType, Type type, string name, Type[] argumentTypes)
    {
      return type.GetMethod (interfaceType.FullName + "." + name, BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, argumentTypes, new ParameterModifier[0]);
    }

    public static TResult ExecuteScriptExpression<TResult> (string expressionScriptSourceCode, params object[] scriptParameter)
    {
      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (expressionScriptSourceCode, SourceCodeKind.Expression);
      var compiledScript = scriptSource.Compile ();
      var scriptScope = ScriptingHost.GetScriptEngine (scriptLanguageType).CreateScope ();

      for (int i = 0; i < scriptParameter.Length; i++)
      {
        scriptScope.SetVariable ("p" + i, scriptParameter[i]);
      }
      return compiledScript.Execute<TResult> (scriptScope);
    }


    public static long ExecuteAndTime (int nrLoop, Func<Object> func)
    {
      var timings = ExecuteAndTime (new[] { nrLoop}, func);
      return timings.Single();
    }


    public static long[] ExecuteAndTime (int[] nrLoopsArray, Func<Object> func)
    {
      return ExecuteAndTimeStable (nrLoopsArray, 10, func);
    }


    public static long[] ExecuteAndTimeFast (int[] nrLoopsArray, Func<Object> func)
    {
      var timings = new System.Collections.Generic.List<long> ();

      foreach (var nrLoops in nrLoopsArray)
      {
        System.GC.Collect (2);
        System.GC.WaitForPendingFinalizers ();

        Stopwatch stopwatch = new Stopwatch ();
        stopwatch.Start ();

        for (int i = 0; i < nrLoops; i++)
        {
          func ();
        }

        stopwatch.Stop ();
        timings.Add (stopwatch.ElapsedMilliseconds);
      }

      return timings.ToArray();
    }


    /// <summary>
    /// Timing method which takes the fastest timing from <paramref name="nrRuns"/> timing runs, 
    /// thereby making the timing results more stable.
    /// </summary>
    public static long[] ExecuteAndTimeStable (int[] nrLoopsArray, int nrRuns ,Func<Object> func)
    {
      var nrLoopsArrayLength = nrLoopsArray.Length;
      var timings = new long[nrLoopsArrayLength];
      for (int iLoop = 0; iLoop < nrLoopsArrayLength; iLoop++)
      {
        timings[iLoop] = long.MaxValue;
      }

      for (int iRun = 0; iRun < nrRuns; iRun++)
      {
        for (int iLoop = 0; iLoop < nrLoopsArrayLength; iLoop++)
        {
          System.GC.Collect (2);
          System.GC.WaitForPendingFinalizers();

          Stopwatch stopwatch = new Stopwatch ();
          stopwatch.Start ();

          for (int i = 0; i < nrLoopsArray[iLoop]; i++)
          {
            func();
          }

          stopwatch.Stop();
          timings[iLoop] = Math.Min (timings[iLoop], stopwatch.ElapsedMilliseconds);
        }
      }

      return timings.ToArray ();
    }

    public static void ExecuteAndTime (string testName, int[] nrLoopsArray, Func<Object> func)
    {
      var timings = ExecuteAndTime (nrLoopsArray, func);

      Console.WriteLine ("Timings \"{0}\", nrLoopsArray={{{1}}}:", testName, string.Join (", ", (IList) nrLoopsArray));
      Console.WriteLine ("(" + string.Join (", ", timings) + ")");
    }


    public static FieldInfo GetProxiedField (object proxy)
    {
      Type proxyType = GetActualType (proxy);
      return proxyType.GetField ("_proxied", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public static object GetProxiedFieldValue (object proxy)
    {
      var proxiedField = GetProxiedField (proxy);
      return proxiedField.GetValue (proxy);
    }

    public static void SetProxiedFieldValue (object proxy, object value)
    {
      var proxiedField = GetProxiedField (proxy);
      proxiedField.SetValue (proxy, value);
    }

    public static Type GetActualType (object proxy)
    {
      var objectGetType = typeof (object).GetMethod ("GetType");
      return (Type) objectGetType.Invoke (proxy, new object[0]);
    }

  }
}
