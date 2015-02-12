<%@ Page Title="Search" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Search" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search</h1></center>
        <p>Quick explanation on how to use with or w/out the buttons. For more information please go <a href="About.aspx" target="_self">about/help page</a>.</p>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get">
                Search:
                <input type="search" name="BCCSearch" value ="[term1] [term2]"/>
                <button type="submit" onclick="">Search</button>
            </form>
        </center>
    </div>
    <br />
    <div id="listContainer">
        <div class="listControl">
            <a id="expandList">Expand All</a>
            <a id="collapseList">Collapse All</a>
        </div>
        <a href="SearchResults.aspx" target="_self">Go to temp search results</a>
        <ul id="expList">
            <li>Biological Entities
                    <ul>
                        <li>Biological Chemicals
                        </li>
                        <li>Biological Systems
                        </li>
                        <li>Cells
                            <ul>
                                <li>
                                    <a href="http://www.yahoo.com" target="_blank">Eukaryotes</a>
                                </li>
                                <li>
                                    <a href="http://www.yahoo.com" target="_blank">Prokaryotes</a>
                                </li>
                            </ul>
                        </li>
                        <li>Complex Biological Compunds
                        </li>
                    </ul>
            </li>
        </ul>
    </div>
</asp:Content>
