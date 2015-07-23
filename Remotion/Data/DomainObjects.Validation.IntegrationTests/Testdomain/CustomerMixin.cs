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
using Remotion.Globalization;
using Remotion.Mixins;
using Remotion.Validation.Attributes.Validation;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain
{
  public interface ICustomerIntroduced
  {
    [NotEqual ("Chef1")]
    string Title { get; set; }

    Address Address { get; set; }
  }

  [Extends (typeof (Customer))]
  [MultiLingualResources ("Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain.Resources.CustomerMixin")]
  public class CustomerMixin : DomainObjectMixin<Customer>, ICustomerIntroduced
  {
    [Mandatory]
    public virtual Address Address
    {
      get { return Properties[typeof (CustomerMixin), "Address"].GetValue<Address>(); }
      set { Properties[typeof (CustomerMixin), "Address"].SetValue (value); }
    }

    public virtual string Title
    {
      get { return Properties[typeof (CustomerMixin), "Title"].GetValue<string>(); }
      set { Properties[typeof (CustomerMixin), "Title"].SetValue (value); }
    }
  }
}