<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True" Inherits="SearchResults" CodeBehind="SearchResults.aspx.cs" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            Search Results:
            <asp:TextBox ID="searching_textbox" runat="server" placeholder ="(term1),(term2),(term3)"/>
            <asp:Button ID="btnclick" onclick="btnclick_Click" Text="Search" runat="server" />
            <br />
            <asp:Button ID="name_sort" runat="server" Text="Sort_Name" OnClick="name_sort_Click" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="relev_sort" runat="server" OnClick="relev_sort_Click" Text="Sort_relev" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;<asp:Button ID="order_sort" runat="server" OnClick="order_sort_Click" Text="Sort_order" />
            &nbsp;&nbsp;&nbsp;
            <br />
        </center>
        <div id="SearchReCon" runat="server"></div>
    </div>
</asp:Content>

