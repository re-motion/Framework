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
using Remotion.Validation.Attributes.Validation;

namespace Remotion.Validation.Mixins.UnitTests.TestDomain
{
  public class Customer : Person
  {
    [LengthValidation(MaxLength = 8, IsRemovable = true)]
    [NotEqualValidation("Test")]
    public virtual string UserName { get; set; }

    public virtual string Email { get; set; }

    public virtual string PhoneNumber { get; set; }

    public virtual string CreditcardNumber { get; set; }

    public virtual Address BillingAddress { get; set; }

    public virtual ICollection<Address> ShippingAddresses { get; set; }

    public string Dummy ()
    {
      return "Dummy";
    }
  }
}
