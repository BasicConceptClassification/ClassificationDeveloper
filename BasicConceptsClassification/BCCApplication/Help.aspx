<%@ Page Title="Help" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Codebehind="Help.aspx.cs" Inherits="Help" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <div>
        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
    </div>
</asp:Content>