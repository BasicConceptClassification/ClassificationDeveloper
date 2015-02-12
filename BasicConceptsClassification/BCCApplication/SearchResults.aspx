<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SearchResults.aspx.cs" Inherits="SearchResults" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search Results</h1></center>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get">
                Search Results:
                <input type="search" name="BCCSearch" value ="[term1] [term2]"/>
                <button type="submit" onclick="">Search</button>
            </form>
        </center>
    </div>
    <div>

    </div>
</asp:Content>