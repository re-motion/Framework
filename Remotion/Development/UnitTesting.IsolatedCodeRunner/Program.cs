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
using System.IO;
using System.Reflection;
using Remotion.Tools;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner
{
  internal class Program
  {
    public class IsolatedCodeAppContextRunner : CustomAppContextRunnerBase
    {
      private readonly Action<string[]> _action;
      private readonly string[] _args;

      public IsolatedCodeAppContextRunner (Action<string[]> action, string[] args, string baseDirectory, string? configFile)
          : base(baseDirectory, configFile)
      {
        _action = action;
        _args = args;
      }

      protected override void RunImplementation ()
      {
        _action.Invoke(_args);
      }
    }

    public static int Main (string[] args)
    {
      try
      {
        var isolatedCodeAppContextRunner = new IsolatedCodeAppContextRunner(
            ResolveTargetMethod(),
            args,
            GetRequiredSettingFromEnvironment(IsolatedCodeRunner.TargetBaseDirectory),
            GetSettingFromEnvironment(IsolatedCodeRunner.TargetConfigFile));
        isolatedCodeAppContextRunner.Run();

        return 0;
      }
      catch (Exception ex)
      {
        ReportException(ex);
        return 1;
      }
    }

    private static string GetRequiredSettingFromEnvironment (string name)
    {
      return Environment.GetEnvironmentVariable(name) ?? throw new InvalidOperationException($"Required environment setting '{name}' was not set.");
    }

    private static string? GetSettingFromEnvironment (string name)
    {
      return Environment.GetEnvironmentVariable(name);
    }

    private static void ReportException (Exception exception)
    {
      // Serialization format (keep in sync with IsolatedCodeRunner.cs):
      Console.WriteLine($"{IsolatedCodeRunner.ExceptionMarker}{ExceptionSerializer.SerializeException(exception)}");
    }

    private static Action<string[]> ResolveTargetMethod ()
    {
      var assemblyPath = GetRequiredSettingFromEnvironment(IsolatedCodeRunner.TargetAssemblyPath);
      var typeName = GetRequiredSettingFromEnvironment(IsolatedCodeRunner.TargetTypeName);
      var methodName = GetRequiredSettingFromEnvironment(IsolatedCodeRunner.TargetMethodName);

      if (!File.Exists(assemblyPath))
        throw new InvalidOperationException($"Could not find assembly '{assemblyPath}'.");

      var assembly = Assembly.LoadFile(assemblyPath);
      var type = assembly.GetType(typeName);
      if (type == null)
        throw new InvalidOperationException($"Could not find type '{typeName}' in assembly '{assembly.GetName()}'.");

      var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (method == null)
        throw new InvalidOperationException($"Could not locate method '{typeName}.{methodName}' in '{assembly.GetName()}'");

      return method.CreateDelegate<Action<string[]>>();
    }
  }
}
