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
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class LocalizationFileNameStrategyTest
  {
    [Test]
    public void GetLocalizationFileNames_NoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\notexisting.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.That (localizationFileNames, Is.Not.Null);
      Assert.That (localizationFileNames.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetLocalizationFileNames_OneLocalizationFile ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\OneLocalizationFile.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.That (localizationFileNames, Is.Not.Null);
      Assert.That (localizationFileNames.Length, Is.EqualTo (1));
      Assert.That (localizationFileNames, Has.Member (@"Metadata\LocalizationFiles\OneLocalizationFile.Localization.de.xml"));
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\TwoLocalizationFiles.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.That (localizationFileNames, Is.Not.Null);
      Assert.That (localizationFileNames.Length, Is.EqualTo (2));
      Assert.That (localizationFileNames, Has.Member (@"Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.de.xml"));
      Assert.That (localizationFileNames, Has.Member (@"Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.en.xml"));
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFilesIncludingInvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.That (localizationFileNames, Is.Not.Null);
      Assert.That (localizationFileNames.Length, Is.EqualTo (2));
      Assert.That (localizationFileNames, Has.Member (@"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml"));
      Assert.That (localizationFileNames, Has.Member (@"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml"));
    }

    [Test]
    public void GetLocalizationFileNames_WithoutBaseDirectory ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string wd = Directory.GetCurrentDirectory ();
      Directory.SetCurrentDirectory (@"Metadata\LocalizationFiles");
      string metadataFileName = "TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Directory.SetCurrentDirectory (wd);

      Assert.That (localizationFileNames, Is.Not.Null);
      Assert.That (localizationFileNames.Length, Is.EqualTo (2));
      Assert.That (localizationFileNames, Has.Member (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml"));
      Assert.That (localizationFileNames, Has.Member (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml"));
    }

    [Test]
    public void GetLocalizationFileName_GermanLanguage ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, new CultureInfo ("de"));

      Assert.That (localizationFilename, Is.EqualTo ("metadata.Localization.de.xml"));
    }

    [Test]
    public void GetLocalizationFileName_InvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, CultureInfo.InvariantCulture);

      Assert.That (localizationFilename, Is.EqualTo ("metadata.Localization.xml"));
    }
  }
}
