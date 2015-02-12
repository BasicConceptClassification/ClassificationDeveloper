<%@ Page Title="Classifacation" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Classification.aspx.cs" Inherits="Classification" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>The Classification</h1></center>
        <dt>Some brief text about the classification? But not like the help/about page...</dt>
    </div>
    <div id="listContainer">
        <div class="listControl">
            <a id="expandList">Expand All</a>
            <a id="collapseList">Collapse All</a>
        </div>
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
    <div>
        <h3><a href="mailto:admin@example.com?subject=Term Suggestion">Suggest new term?</a></h3>
    </div>
</asp:Content>
