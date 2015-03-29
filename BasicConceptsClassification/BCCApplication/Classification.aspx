<%@ Page Title="The Classification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Classification.aspx.cs" Inherits="BCCApplication.Classification" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <div>
        <asp:Label ID="LabelDescription" runat="server"></asp:Label>
    </div>
</asp:Content>
