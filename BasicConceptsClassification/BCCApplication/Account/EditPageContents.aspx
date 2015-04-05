<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditPageContents.aspx.cs" Inherits="BCCApplication.Account.EditPageContents" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Edit Page Contents</h3>
    <div>
        <h4>About Page</h4>
        <asp:TextBox runat="server" id="AboutBox" TextMode="MultiLine" Height="150px" Width="450px"/>
        <asp:Button runat="server" Text="Save" id="AboutBoxSave" OnClick="AboutBoxSave_Click"/>
    </div>
    

</asp:Content>
