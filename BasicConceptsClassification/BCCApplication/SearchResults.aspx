﻿<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="SearchResults" CodeBehind="SearchResults.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            Search Results:
            <asp:TextBox ID="searching_textbox" runat="server" placeholder ="Enter text to print here"/>
            <asp:Button ID="btnclick" onclick="btnclick_Click" Text="Test Button!" runat="server" />
            <br />
            <asp:Button ID="name_sort" runat="server" Text="Sort_Name" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="relev_sort" runat="server" OnClick="relev_sort_Click" Text="Sort_relev" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
        </center>
        <div id="SearchReCon" runat="server"></div>
    </div>
</asp:Content>

