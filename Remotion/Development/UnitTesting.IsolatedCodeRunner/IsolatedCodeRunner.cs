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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner
{
  /// <summary>
  /// Provides methods for executing code in an isolated environment (new process).
  /// </summary>
  /// <remarks>
  /// Exceptions are marshalled and rethrown if possible.
  /// If an exception cannot be recreated, a <see cref="IsolatedCodeException"/> is thrown instead.
  /// </remarks>
  public sealed class IsolatedCodeRunner
  {
    internal const string ExceptionMarker = "##exception:";

    internal const string TargetAssemblyPath = "ICR_TARGET_ASSEMBLY";
    internal const string TargetTypeName = "ICR_TARGET_TYPE";
    internal const string TargetMethodName = "ICR_TARGET_METHOD";
    internal const string TargetBaseDirectory = "ICR_BASE_DIRECTORY";

    internal const string TargetConfigFile = "ICR_CONFIG_FILE";

    private readonly TimeSpan _timeout;
    private readonly StringBuilder _output = new();

    private readonly MethodInfo _targetMethod;
    private readonly string _assemblyLocation;
    private readonly string _typeName;
    private readonly string _methodName;

    private Exception? _exception;

    public string? ConfigFile { get; set; }

    public IsolatedCodeRunner (Action<string[]> testAction)
        : this(testAction, TimeSpan.FromSeconds(15))
    {
    }

    public IsolatedCodeRunner (Action<string[]> testAction, TimeSpan timeout)
    {
      ArgumentUtility.CheckNotNull(nameof(testAction), testAction);

      var testActionMethod = testAction.Method;
      if (!testActionMethod.IsStatic)
        throw new ArgumentException("The specified test method must be static.");

      _timeout = timeout;

      _targetMethod = testActionMethod;
      _assemblyLocation = testActionMethod.DeclaringType!.Assembly.Location;
      _typeName = testActionMethod.DeclaringType!.FullName!;
      _methodName = testActionMethod.Name;
    }

    public string Output
    {
      get
      {
        lock (_output)
        {
          return _output.ToString();
        }
      }
    }

    public void Run (params string[] args)
    {
      ArgumentUtility.CheckNotNull(nameof(args), args);

      var isolatedCodeRunnerExePath = Path.ChangeExtension(typeof(IsolatedCodeRunner).Assembly.Location, "exe");
      if (!File.Exists(isolatedCodeRunnerExePath))
        throw new InvalidOperationException("Cannot find the isolated code runner exe.");

      var startInfo = new ProcessStartInfo
                      {
                          WindowStyle = ProcessWindowStyle.Hidden,
                          CreateNoWindow = false,
                          RedirectStandardOutput = true
                      };

      startInfo.FileName = isolatedCodeRunnerExePath;
      startInfo.Arguments = string.Join(" ", args.Select(e => $@"""{e.Replace("\"", "\"\"")}"""));
      startInfo.WorkingDirectory = Path.GetDirectoryName(_targetMethod.DeclaringType!.Assembly.Location);

      startInfo.Environment[TargetAssemblyPath] = _assemblyLocation;
      startInfo.Environment[TargetTypeName] = _typeName;
      startInfo.Environment[TargetMethodName] = _methodName;

      startInfo.Environment[TargetBaseDirectory] = AppContext.BaseDirectory;

      if (!string.IsNullOrEmpty(ConfigFile))
        startInfo.Environment[TargetConfigFile] = ConfigFile;

      using var process = new Process();
      process.StartInfo = startInfo;
      process.OutputDataReceived += DataReceived;

      process.Start();
      process.BeginOutputReadLine();

      if (process.WaitForExit((int)_timeout.TotalMilliseconds))
      {
        // Need to call .WaitForExit() again to make sure that standard out is flushed
        process.WaitForExit();
      }
      else
      {
        process.Kill();
        throw new InvalidOperationException("Isolated code runner execution failed to stop within the specified timeout.");
      }

      if (_exception != null)
        throw _exception;

      if (process.ExitCode != 0)
        throw new InvalidOperationException($"Isolated code runner execution failed with exit code {process.ExitCode}.");
    }

    private void DataReceived (object sender, DataReceivedEventArgs args)
    {
      var line = args.Data;
      if (line == null)
        return;

      if (line.StartsWith(ExceptionMarker))
      {
        // Serialization format (keep in sync with Program.cs):
        _exception = ExceptionSerializer.DeserializeException(line[ExceptionMarker.Length..]);
        return;
      }

      lock (_output)
      {
        _output.AppendLine(line);
      }

    }
  }
}
