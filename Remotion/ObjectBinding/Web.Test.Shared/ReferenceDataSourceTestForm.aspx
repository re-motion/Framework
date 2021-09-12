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
<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ReferenceDataSourceTestForm.aspx.cs"
  Inherits="Remotion.ObjectBinding.Web.Test.Shared.ReferenceDataSourceTestForm" MasterPageFile="~/StandardMode.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true"
    AsyncPostBackTimeout="3600" />
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
  <remotion:SingleView ID="SingleView" runat="server">
    <TopControls>
      <asp:PlaceHolder ID="ButtonPlaceHolder" runat="server">
        <div>
          <remotion:WebButton ID="PostBackButton" runat="server" Text="Post Back" />
          <remotion:WebButton ID="ValidateButton" runat="server" Text="Validate" OnClick="ValidateButton_OnClick" />
          <remotion:WebButton ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_OnClick" />
      </div>
      </asp:PlaceHolder>
    </TopControls>
    <View>
      <remotion:WebUpdatePanel ID="UserControlUpdatePanel" runat="server" style="height: 100%">
        <ContentTemplate>
          <remotion:FormGridManager ID="FormGridManager" runat="server" />
          <remotion:BindableObjectDataSourceControl ID="LevelOneDataSource" runat="server"
            Type="Remotion.ObjectBinding.Sample::ReferenceDataSourceTestDomain.LevelOne" />
          <table id="LevelOneFormGrid" runat="server">
            <tr>
              <td>
                Level One
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelOneStringValueField" runat="server" DataSourceControl="LevelOneDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelOneIntValueField" runat="server" DataSourceControl="LevelOneDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="IntValue" />
              </td>
            </tr>
          </table>

          <remotion:BusinessObjectReferenceDataSourceControl ID="LevelTwoDataSource" runat="server" DataSourceControl="LevelOneDataSource" EnableOptionalValidators="true" PropertyIdentifier="ReferenceValue" />
          <table id="LevelTwoFormGrid" runat="server">
            <tr>
              <td>
                Level Two
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelTwoStringValueField" runat="server" DataSourceControl="LevelTwoDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelTwoIntValueField" runat="server" DataSourceControl="LevelTwoDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="IntValue" />
              </td>
            </tr>
          </table>

          <remotion:BusinessObjectReferenceDataSourceControl ID="LevelThreeDataSource" runat="server" DataSourceControl="LevelTwoDataSource" EnableOptionalValidators="true" PropertyIdentifier="ReferenceValue" />
          <table id="LevelThreeFormGrid" runat="server">
            <tr>
              <td>
                Level Three
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelThreeStringValueField" runat="server" DataSourceControl="LevelThreeDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelThreeIntValueField" runat="server" DataSourceControl="LevelThreeDataSource" EnableOptionalValidators="true"
                  PropertyIdentifier="IntValue" />
                <asp:CustomValidator ID="LevelThreeIntValueFieldCustomValidator" runat="server" OnServerValidate="LevelThreeIntValueFieldCustomValidator_OnServerValidate" ErrorMessage="Most not be 0" ControlToValidate="LevelThreeIntValueField" />
              </td>
            </tr>
          </table>
        </ContentTemplate>
      </remotion:WebUpdatePanel>
    </View>
    <BottomControls>
      <asp:UpdatePanel ID="StackUpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
          <asp:Literal ID="Stack" runat="server" EnableViewState="false"/>
        </ContentTemplate>
      </asp:UpdatePanel>
    </BottomControls>
  </remotion:SingleView>
</asp:Content>
