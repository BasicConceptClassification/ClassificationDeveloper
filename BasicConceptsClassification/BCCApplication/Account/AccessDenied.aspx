<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="BCCApplication.Account.AccessDenied" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h1>Access Denied</h1>
    <p class="lead">You don't have permission to access that resource :^(</p>
    <p>If you think this is a mistake, you can <a href="../Contact.aspx">contact</a> an administrator.</p>
</asp:Content>
