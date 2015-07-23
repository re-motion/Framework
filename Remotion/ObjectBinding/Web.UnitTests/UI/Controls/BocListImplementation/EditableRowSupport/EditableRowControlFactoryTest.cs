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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditableRowControlFactoryTest
  {
    private BindableObjectClass _stringValueClass;
    private IBusinessObjectPropertyPath _stringValuePropertyPath;
    private BocSimpleColumnDefinition _stringValueColumn;

    private EditableRowControlFactory _factory;

    [SetUp]
    public virtual void SetUp ()
    {
      _stringValueClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof (TypeWithString));

      _stringValuePropertyPath = BusinessObjectPropertyPath.CreateStatic (_stringValueClass, "FirstValue");

      _stringValueColumn = new BocSimpleColumnDefinition();
      _stringValueColumn.SetPropertyPath (_stringValuePropertyPath);

      _factory = EditableRowControlFactory.CreateEditableRowControlFactory();
    }

    [Test]
    public void CreateWithStringProperty ()
    {
      IBusinessObjectBoundEditableWebControl control = _factory.Create (_stringValueColumn, 0);

      Assert.That (control, Is.Not.Null);
      Assert.That (control is BocTextValue, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void CreateWithNegativeIndex ()
    {
      _factory.Create (_stringValueColumn, -1);
    }
  }
}