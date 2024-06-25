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
using System.IO;
using System.Linq;
using Remotion.Development.UnitTesting.Compilation;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  public class AssemblyCompilerBuildOutputManager : IDisposable
  {
    private readonly string _buildOutputDirectory;
    private readonly string _sourceDirectoryRoot;
    private readonly string[] _alwaysReferencedAssemblies;

    private readonly HashSet<string> _generatedAssemblyPaths = new HashSet<string>();
    private readonly HashSet<string> _generatedDirectories = new HashSet<string>();

    private bool _isDisposed = false;

    public AssemblyCompilerBuildOutputManager (
        string buildOutputDirectory,
        bool createBuildOutputDirectory,
        string sourceDirectoryRoot,
        params string[] alwaysReferencedAssemblies)
    {
      _buildOutputDirectory = buildOutputDirectory;
      _sourceDirectoryRoot = sourceDirectoryRoot;
      _alwaysReferencedAssemblies = alwaysReferencedAssemblies;

      if (createBuildOutputDirectory)
        CreateEmptyDirectory(_buildOutputDirectory);
    }

    public string BuildOutputDirectory
    {
      get { return _buildOutputDirectory; }
    }

    public string SourceDirectoryRoot
    {
      get { return _sourceDirectoryRoot; }
    }

    public void Dispose ()
    {
      if (_isDisposed)
        return;

      _isDisposed = true;

      foreach (var generatedAssemblyPath in _generatedAssemblyPaths)
        FileUtility.DeleteAndWaitForCompletion(generatedAssemblyPath);

      foreach (var generatedDirectory in _generatedDirectories)
        Directory.Delete(generatedDirectory, true);
    }

    public string Compile (string outputAssemblyFileName, params string[] referencedAssemblies)
    {
      var outputAssemblyPath = Path.Combine(_buildOutputDirectory, outputAssemblyFileName);
      var allReferencedAssemblies = _alwaysReferencedAssemblies.Concat(referencedAssemblies).ToArray();
      var fullSourceDirectory = Path.Combine(_sourceDirectoryRoot, Path.GetFileNameWithoutExtension(outputAssemblyFileName));
      var assemblyCompiler = new AssemblyCompiler(fullSourceDirectory, outputAssemblyPath, allReferencedAssemblies);

      assemblyCompiler.Compile();
      _generatedAssemblyPaths.Add(outputAssemblyPath);
      return outputAssemblyPath;
    }

    public string RenameGeneratedAssembly (string oldFileName, string newFileName)
    {
      var oldAssemblyPath = Path.Combine(_buildOutputDirectory, oldFileName);
      var newAssemblyPath = Path.Combine(_buildOutputDirectory, newFileName);
      File.Move(oldAssemblyPath, newAssemblyPath);
      _generatedAssemblyPaths.Remove(oldAssemblyPath);
      _generatedAssemblyPaths.Add(newAssemblyPath);
      return newAssemblyPath;
    }

    public void CopyAllGeneratedAssembliesToNewDirectory (string destinationDirectory)
    {
      CreateEmptyDirectory(destinationDirectory);

      foreach (var generatedAssemblyPath in _generatedAssemblyPaths.ToArray())
      {
        string fileName = Path.GetFileName(generatedAssemblyPath);
        var sourceAssemblyPath = Path.Combine(_buildOutputDirectory, fileName);
        var destinationAssemblyPath = Path.Combine(destinationDirectory, fileName);
        File.Copy(sourceAssemblyPath, destinationAssemblyPath);
        _generatedAssemblyPaths.Add(destinationDirectory);
      }
    }

    private void CreateEmptyDirectory (string directory)
    {
      if (Directory.Exists(directory))
        Directory.Delete(directory, true);

      Directory.CreateDirectory(directory);
      _generatedDirectories.Add(directory);
    }
  }
}
