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
using NUnit.Framework;
using Remotion.Text;

namespace Remotion.Extensions.UnitTests.Text
{

[TestFixture]
public class IdentifierGeneratorTest
{
  [Test]
	public void TestUseTemplateGenerator ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle;
	  Assert.That (
	      () => idGen.GetUniqueIdentifier ("some name"),
	      Throws.InvalidOperationException);
  }

  [Test]
	public void TestChangeUseCaseSensitiveNamesAfterGeneratingUniqueIdentifier ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();
    idGen.GetUniqueIdentifier ("some name");
	  Assert.That (
	      () => idGen.UseCaseSensitiveNames = false,
	      Throws.InvalidOperationException);
  }

  [Test]
	public void TestCStyle ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "3myid", "_myid");
    CheckValidIdentifier (idGen, "_myid3", "_myid3");
    CheckValidIdentifier (idGen, "myidä", "myid_");
    CheckValidIdentifier (idGen, "myidß", "myid_");

    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "MyÄid", "My_id");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_1");
    CheckUniqueIdentifier (idGen, "myÖid", "my_id_2");
    CheckUniqueIdentifier (idGen, "myßid", "my_id_3");
    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_1");
  }

  [Test]
	public void TestUniqueObjects ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.CStyle.Clone();
    object o1 = new object(), o2 = new object(), o3 = new object();

    CheckUniqueIdentifier (idGen, o1, "my_id", "my_id");
    CheckUniqueIdentifier (idGen, o2, "my_id", "my_id_1");
    CheckUniqueIdentifier (idGen, o3, "my_id", "my_id_2");
    CheckUniqueIdentifier (idGen, o1, "xxx", "my_id");
    CheckUniqueIdentifier (idGen, o2, "xxx", "my_id_1");
    CheckUniqueIdentifier (idGen, o3, "xxx", "my_id_2");
	}

  [Test]
	public void TestHtmlStyle ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.HtmlStyle.Clone();
    idGen.UseCaseSensitiveNames = false;

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "_myid", "myid");
    CheckValidIdentifier (idGen, "myid3", "myid3");
    CheckValidIdentifier (idGen, "myidä", "myid_");
    CheckValidIdentifier (idGen, "myidä", "myid_");
    CheckValidIdentifier (idGen, "myidß", "myid_");

    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "MyÄid", "my_id");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_1");
    CheckUniqueIdentifier (idGen, "myÖid", "my_id_2");
    CheckUniqueIdentifier (idGen, "myßid", "my_id_3");
    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_1");
  }

  [Test]
	public void TestXmlStyle ()
	{
    IdentifierGenerator idGen = IdentifierGenerator.XmlStyle.Clone();

    CheckValidIdentifier (idGen, "myid", "myid");
    CheckValidIdentifier (idGen, "_myid", "_myid");
    CheckValidIdentifier (idGen, "-myid", "_myid");
    CheckValidIdentifier (idGen, "myid3", "myid3");
    CheckValidIdentifier (idGen, "myidä", "myid_");
    CheckValidIdentifier (idGen, "myidß", "myid_");

    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "MyÄid", "My_id_1");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_2");
    CheckUniqueIdentifier (idGen, "myÖid", "my_id_3");
    CheckUniqueIdentifier (idGen, "myßid", "my_id_4");
    CheckUniqueIdentifier (idGen, "myÄid", "my_id");
    CheckUniqueIdentifier (idGen, "myÜid", "my_id_2");
  }

  public void CheckValidIdentifier (IdentifierGenerator idGen, string uniqueName, string expectedIdentifier)
  {
    Assert.That (idGen.GetValidIdentifier (uniqueName), Is.EqualTo (expectedIdentifier));
  }

  public void CheckUniqueIdentifier (IdentifierGenerator idGen, string uniqueName, string expectedIdentifier)
  {
    Assert.That (idGen.GetUniqueIdentifier (uniqueName), Is.EqualTo (expectedIdentifier));
  }

  public void CheckUniqueIdentifier (IdentifierGenerator idGen, object uniqueObject, string name, string expectedIdentifier)
  {
    Assert.That (idGen.GetUniqueIdentifier (uniqueObject, name), Is.EqualTo (expectedIdentifier));
  }
}

}
