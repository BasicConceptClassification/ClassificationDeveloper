<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Codebehind="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <h1>Contact Us</h1>
        <asp:Label runat="server" ID="Content" />
        <br />
        <br />
        <asp:Button ID="CFormButton" runat="server" Text="Contact Admin" OnClick="CForm_Click" />
    </div>
</asp:Content>
