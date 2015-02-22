<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="SearchResults" Codebehind="SearchResults.aspx.cs" %>

<script runat="server">
    private void searchResultprint(object sender, EventArgs e)
    {
        
    }
</script>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get">
                Search Results:
                <input type="search" name="BCCSearch" placeholder ="[term1] [term2]"/>
                <input type="button" onclick="location.href='SearchResults.aspx'" value='Search Again' />
                <br />
            </form>
        </center>
        <div id ="SearchReCon" runat="server"></div>
            <br />
    </div>
</asp:Content>