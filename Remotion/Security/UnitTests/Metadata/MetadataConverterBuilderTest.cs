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
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataConverterBuilderTest
  {
    private MetadataConverterBuilder _converterBuilder;

    [SetUp]
    public void SetUp ()
    {
      _converterBuilder = new MetadataConverterBuilder();
    }

    [Test]
    public void Create_SimpleMetadataToXmlConverter ()
    {
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(MetadataToXmlConverter)));
    }

    [Test]
    public void Create_LocalizingConverterForOneLanguage ()
    {
      _converterBuilder.AddLocalization("de");

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.Null);
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(1));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
    }

    [Test]
    public void Create_LocalizingConverterForLanguageWithWhitespaces ()
    {
      _converterBuilder.AddLocalization(" de ");

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.Null);
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(1));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
    }

    [Test]
    public void Create_LocalizingConverterForOneCultureInfo ()
    {
      _converterBuilder.AddLocalization(new CultureInfo("de"));

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.Null);
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(1));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
    }

    [Test]
    public void Create_LocalizingConverterForTwoLanguages ()
    {
      _converterBuilder.AddLocalization("de");
      _converterBuilder.AddLocalization("fr");

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.Null);
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(2));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
      Assert.That(localizingConverter.Cultures[1].TwoLetterISOLanguageName, Is.EqualTo("fr"));
    }

    [Test]
    public void Create_LocalizingConverterWithMetadataToXmlConverter ()
    {
      _converterBuilder.AddLocalization("de");
      _converterBuilder.AddLocalization("fr");
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.InstanceOf(typeof(MetadataToXmlConverter)));
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(2));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
      Assert.That(localizingConverter.Cultures[1].TwoLetterISOLanguageName, Is.EqualTo("fr"));
    }

    [Test]
    public void Create_LocalizingConverterWithInvariantCulture ()
    {
      _converterBuilder.AddLocalization("de");
      _converterBuilder.AddLocalization(CultureInfo.InvariantCulture);

      IMetadataConverter converter = _converterBuilder.Create();

      Assert.That(converter, Is.InstanceOf(typeof(LocalizingMetadataConverter)));
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter)converter;
      Assert.That(localizingConverter.MetadataConverter, Is.Null);
      Assert.That(localizingConverter.Cultures.Length, Is.EqualTo(2));
      Assert.That(localizingConverter.Cultures[0].TwoLetterISOLanguageName, Is.EqualTo("de"));
      Assert.That(localizingConverter.Cultures[1].TwoLetterISOLanguageName, Is.EqualTo(CultureInfo.InvariantCulture.TwoLetterISOLanguageName));
    }

    [Test]
    public void Create_ExceptionWithoutLocalizationAndMetadataConverter ()
    {
      Assert.That(
          () => _converterBuilder.Create(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "You must specify at least a localization or a metadata converter."));
    }
  }
}
