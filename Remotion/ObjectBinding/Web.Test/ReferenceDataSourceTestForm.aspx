<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ReferenceDataSourceTestForm.aspx.cs"
  Inherits="OBWTest.ReferenceDataSourceTestForm" MasterPageFile="~/StandardMode.Master" %>

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
                <remotion:BocTextValue ID="LevelOneStringValueField" runat="server" DataSourceControl="LevelOneDataSource"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelOneIntValueField" runat="server" DataSourceControl="LevelOneDataSource"
                  PropertyIdentifier="IntValue" />
              </td>
            </tr>
          </table>

          <remotion:BusinessObjectReferenceDataSourceControl ID="LevelTwoDataSource" runat="server" DataSourceControl="LevelOneDataSource" PropertyIdentifier="ReferenceValue" />
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
                <remotion:BocTextValue ID="LevelTwoStringValueField" runat="server" DataSourceControl="LevelTwoDataSource"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelTwoIntValueField" runat="server" DataSourceControl="LevelTwoDataSource"
                  PropertyIdentifier="IntValue" />
              </td>
            </tr>
          </table>

          <remotion:BusinessObjectReferenceDataSourceControl ID="LevelThreeDataSource" runat="server" DataSourceControl="LevelTwoDataSource" PropertyIdentifier="ReferenceValue" />
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
                <remotion:BocTextValue ID="LevelThreeStringValueField" runat="server" DataSourceControl="LevelThreeDataSource"
                  PropertyIdentifier="StringValue" />
              </td>
            </tr>
            <tr>
              <td>
              </td>
              <td>
                <remotion:BocTextValue ID="LevelThreeIntValueField" runat="server" DataSourceControl="LevelThreeDataSource"
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
