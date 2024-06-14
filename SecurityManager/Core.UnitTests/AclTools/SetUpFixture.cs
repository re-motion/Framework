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
using System.Globalization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Globalization.AclTools.Expansion;
using Remotion.SecurityManager.UnitTests.Domain;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;
    private Culture _cultureEn;
    private Culture _cultureDe;

    public static IDomainObjectHandle<SecurableClassDefinition> OrderClassHandle { get; private set; }
    public static  List<AccessControlList> aclList { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        // Use default localization for tests
        AclToolsExpansion.Culture = new CultureInfo("");

        AccessControlTestHelper testHelper = new AccessControlTestHelper();
        using (testHelper.Transaction.EnterDiscardingScope())
        {
          _dbFixtures = new DatabaseFixtures();
          _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.Current);

          _cultureDe = Culture.NewObject("de-DE");
          _cultureEn = Culture.NewObject("en-US");

          SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
          OrderClassHandle = orderClass.GetHandle();

          testHelper.AttachAccessType(orderClass, Guid.NewGuid(), "FirstAccessType", 0);
          testHelper.AttachAccessType(orderClass, Guid.NewGuid(), "FirstAccessType2", 2);
          testHelper.AttachAccessType(orderClass, Guid.NewGuid(), "FirstAccessType3", 3);
          aclList = testHelper.CreateAclsForOrderAndPaymentAndDeliveryStates(orderClass);
          var ace = aclList[0].CreateAccessControlEntry();
          ace.GetPermissions()[0].Allowed = true; // FirstAccessType

          testHelper.CreateInvoiceClassDefinition();

          LocalizeClassEnDe(orderClass, "Order", "Bestellung");

          LocalizeStatePropertyEnDe(orderClass, "Payment", "Payment", "Bezahlstatus");
          LocalizeStateEnDe(orderClass, "Payment", (int)PaymentState.None, "None", "Offen");
          LocalizeStateEnDe(orderClass, "Payment", (int)PaymentState.Paid, "Paid", "Bezahlt");

          LocalizeStatePropertyEnDe(orderClass, "State", "Order State", "Bestellstatus");
          LocalizeStateEnDe(orderClass, "State", (int)OrderState.Delivered, "Delivered", "Ausgelifert");
          LocalizeStateEnDe(orderClass, "State", (int)OrderState.Received, "Received", "Erhalten");

          LocalizeStatePropertyEnDe(orderClass, "Delivery", "Delivery Provider", "Auslieferer");
          LocalizeStateEnDe(orderClass, "Delivery", (int)Delivery.Dhl, "DHL", "DHL");
          LocalizeStateEnDe(orderClass, "Delivery", (int)Delivery.Post, "Mail", "Post");

          ClientTransaction.Current.Commit();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }


    private void LocalizeMetadataObjectEnDe (MetadataObject metadataObject, string nameEnglish, string nameGerman)
    {
      LocalizedName.NewObject(nameGerman, _cultureDe, metadataObject);
      LocalizedName.NewObject(nameEnglish, _cultureEn, metadataObject);
    }

    private void LocalizeClassEnDe (SecurableClassDefinition classDefinition, string nameEnglish, string nameGerman)
    {
      LocalizeMetadataObjectEnDe(classDefinition, nameEnglish, nameGerman);
    }

    private void LocalizeStatePropertyEnDe (SecurableClassDefinition classDefinition,
      string statePropertyName, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty(statePropertyName);
      LocalizeMetadataObjectEnDe(stateProperty, nameEnglish, nameGerman);
    }

    private void LocalizeStateEnDe (SecurableClassDefinition classDefinition,
      string statePropertyName, int stateEnumValue, string nameEnglish, string nameGerman)
    {
      var stateProperty = classDefinition.GetStateProperty(statePropertyName);
      var state = stateProperty.GetState(stateEnumValue);
      LocalizeMetadataObjectEnDe(state, nameEnglish, nameGerman);
    }
  }
}
