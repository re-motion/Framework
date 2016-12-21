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
using System.Globalization;
using System.IO;

namespace Remotion.Security.Metadata
{
  public class LocalizationFileNameStrategy
  {
    public string GetLocalizationFileName (string metadataFilename, CultureInfo culture)
    {
      string baseFilename = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string baseFilePath = Path.Combine (basePath, baseFilename);

      if (string.IsNullOrEmpty (culture.Name))
        return baseFilePath + ".Localization.xml";

      return baseFilePath + ".Localization." + culture.Name + ".xml";
    }

    public string[] GetLocalizationFileNames (string metadataFilename)
    {
      string baseFileName = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string searchPattern = baseFileName + ".Localization.*xml";

      if (basePath == string.Empty)
        basePath = ".";

      return Directory.GetFiles (basePath, searchPattern, SearchOption.TopDirectoryOnly);
    }
  }
}
