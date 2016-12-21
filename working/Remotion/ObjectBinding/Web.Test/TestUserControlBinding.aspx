<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>

<%@ Page Language="c#" CodeBehind="TestUserControlBinding.aspx.cs" AutoEventWireup="false"
  Inherits="OBWTest.TestUserControlBinding" MasterPageFile="~/StandardMode.Master" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <remotion:FormGridManager ID="FormGridManager" runat="server">
  </remotion:FormGridManager>
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
  Personendaten:
  <remotion:BocTextValue ID="FirstNameField" runat="server" PropertyIdentifier="FirstName"
    DataSourceControl="CurrentObject" ReadOnly="True" style="width:auto">
  </remotion:BocTextValue>
  <remotion:BocTextValue ID="LastNameField" runat="server" PropertyIdentifier="LastName"
    DataSourceControl="CurrentObject" ReadOnly="True" style="width:auto">
  </remotion:BocTextValue>
  <remotion:UserControlBinding ID="UserControlBinding1" runat="server" UserControlPath="TestTabbedPersonDetailsUserControl.ascx"
    DataSourceControl="CurrentObject" PropertyIdentifier="Partner" />
</asp:Content>
