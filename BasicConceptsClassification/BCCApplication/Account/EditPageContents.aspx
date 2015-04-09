<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditPageContents.aspx.cs" Inherits="BCCApplication.Account.EditPageContents" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Edit Page Contents</h1>
    <hr />
    <div>
        <h3>About Page</h3>
        <asp:TextBox runat="server" id="AboutBox" TextMode="MultiLine" Height="150px" Width="450px"/>
        <br />
        <asp:Button runat="server" Text="Save" id="AboutBoxSave" OnClick="AboutBoxSave_Click"/>
    </div>
    <hr />
    <div>
        <h3>Contact Page</h3>
        <h5>Preview</h5>
        <asp:Label runat="server" ID="ContactPreview"/>
        <h5>Information</h5>
        <asp:TextBox runat="server" id="InformationBox" TextMode="MultiLine" Height="150px" Width="450px"/>
        <br />
        <h5>Name</h5>
        <asp:TextBox runat="server" id="NameBox" TextMode="SingleLine" Height="32px" Width="450px"/>
        <br />
        <h5>Phone</h5>
        <asp:TextBox runat="server" id="PhoneBox" TextMode="SingleLine" Height="32px" Width="450px"/>
        <br />
        <h5>Email</h5>
        <asp:TextBox runat="server" id="EmailBox" TextMode="SingleLine" Height="32px" Width="450px"/>
        <br />
        <asp:Button runat="server" Text="Save" id="ContactBoxSave" OnClick="ContactBoxSave_Click"/>
    </div>
    

</asp:Content>
