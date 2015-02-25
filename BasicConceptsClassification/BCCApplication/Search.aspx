﻿<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="Search" Codebehind="Search.aspx.cs" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search</h1></center>
        <p>Quick explanation on how to use with or w/out the buttons. For more information please go <a href="About.aspx" target="_self">about/help page</a>.</p>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get" target="_self">
                Search:
                <input type="search" name="BCCSearch" placeholder ="[term1] [term2]">
                <input type="button" onclick="location.href = 'SearchResults.aspx'" value='Search' />
            </form>
        </center>
    </div>
    <br />
    <div id="listContainer">
        <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
            <ParentNodeStyle Font-Bold="False" />
            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
         </asp:TreeView>
    </div>
</asp:Content>
