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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  public class NewObjectFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NewObjectFunction ()
      : base(WxeTransactionMode.CreateRootWithAutoCommit)
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public ClassWithAllDataTypes ObjectWithAllDataTypes
    {
      get { return (ClassWithAllDataTypes)Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1 ()
    {
      ObjectWithAllDataTypes = ClassWithAllDataTypes.NewObject();

      ClassWithAllDataTypes objectWithAllDataTypes2 = CreateTestObjectWithAllDataTypes();

      ClassForRelationTest objectForRelationTest1 = ClassForRelationTest.NewObject();
      objectForRelationTest1.Name = "ObjectForRelationTest1";
      objectForRelationTest1.ClassWithAllDataTypesMandatory = ObjectWithAllDataTypes;
      objectWithAllDataTypes2.ClassForRelationTestMandatory = objectForRelationTest1;

      ClassForRelationTest objectForRelationTest2 = ClassForRelationTest.NewObject();
      objectForRelationTest2.Name = "ObjectForRelationTest2";
      ObjectWithAllDataTypes.ClassForRelationTestMandatory = objectForRelationTest2;
      objectForRelationTest2.ClassWithAllDataTypesMandatory = objectWithAllDataTypes2;
    }

    private WxePageStep Step2 = new WxePageStep("NewObject.aspx");


    private ClassWithAllDataTypes CreateTestObjectWithAllDataTypes ()
    {
      ClassWithAllDataTypes test = ClassWithAllDataTypes.NewObject();

      test.ByteProperty = 23;
      test.DateProperty = DateTime.Now;
      test.DateTimeProperty = DateTime.Now;
      test.DecimalProperty = 23.2m;
      test.DoubleProperty = 23.2;
      test.GuidProperty = new Guid("{00000008-0000-0000-0000-000000000009}");
      test.Int16Property = 2;
      test.Int32Property = 4;
      test.Int64Property = 8;
      test.SingleProperty = 9.8f;
      test.StringProperty = "aasdf";

      return test;
    }
  }
}
