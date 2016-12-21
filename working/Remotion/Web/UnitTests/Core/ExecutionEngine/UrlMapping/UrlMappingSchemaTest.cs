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
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Schemas;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{

[TestFixture]
public class UrlMappingSchemaTest
{
  [SetUp]
  public virtual void SetUp()
  {
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithMissingPath()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingPath.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithEmptyPath()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyPath.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithMissingFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaValidationException))]
  public void LoadMappingWithEmptyFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlException))]
  public void LoadMappingWithFunctionTypeHavingNoAssembly()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithFunctionTypeHavingNoAssembly.xml");
  }

  [Test]
  public void LoadSchemaSet ()
  {
    UrlMappingSchema urlMappingSchema = new UrlMappingSchema ();
    XmlSchemaSet xmlSchemaSet = urlMappingSchema.LoadSchemaSet ();
    Assert.That (xmlSchemaSet.Count, Is.EqualTo (1));
    Assert.That (xmlSchemaSet.Contains (urlMappingSchema.SchemaUri), Is.True);
  }

}

}
