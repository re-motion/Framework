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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// The <see cref="FileGenerator"/> is responsible to write database scripts to the file system.
  /// </summary>
  public class FileGenerator
  {
    private readonly string _outputPath;

    public FileGenerator (string outputPath)
    {
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      _outputPath = outputPath;
    }

    public void WriteScriptsToDisk (Script script, bool includeStorageProviderName)
    {
      ArgumentUtility.CheckNotNull ("script", script);

      CreateOutputPath();

      var setupDbFileName = GetFileName (script.StorageProviderDefinition, includeStorageProviderName , "SetupDB");
      var tearDownDbFileName = GetFileName (script.StorageProviderDefinition, includeStorageProviderName, "TearDownDB");
      
      File.WriteAllText (setupDbFileName, script.SetUpScript);
      File.WriteAllText (tearDownDbFileName, script.TearDownScript);
    }

    private string GetFileName (StorageProviderDefinition storageProviderDefinition, bool multipleStorageProviders, string fileNamePrefix)
    {
      string fileName;
      if (multipleStorageProviders)
        fileName = String.Format ("{0}_{1}.sql", fileNamePrefix, storageProviderDefinition.Name);
      else
        fileName = fileNamePrefix + ".sql";

      return Path.Combine (_outputPath, fileName);
    }

    private void CreateOutputPath ()
    {
      if (_outputPath != String.Empty && !Directory.Exists (_outputPath))
        Directory.CreateDirectory (_outputPath);
    }
  }
}