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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoUserControl.ascx.cs" Inherits="Test.AutoUserControl" %>
<%@ Register TagPrefix="remotion" Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" %>
<div>
<h1>Auto User Control</h1>
            <table>
            <tr><td>IsPostBack</td><td><asp:Label ID="IsPostBackLabel" runat="server" /></td></tr>
            <tr>
                <td>
                    In
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InArgField" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    InOut
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InOutArgField" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    Out
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="OutArgField" runat="server"></asp:TextBox></td>
            </tr>
        </table>

<remotion:WebButton ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" /></div>
