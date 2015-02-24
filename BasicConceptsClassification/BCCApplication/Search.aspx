<%@ Page Title="Search" Language="C#" CodeBehind ="Search.aspx.cs" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Search" %>


<asp:Searchpart id="Search_part" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <center><h1>Search</h1></center>
        <p>Quick explanation on how to use with or w/out the buttons. For more information please go <a href="About.aspx" target="_self">about/help page</a>.</p>
        <br />
        <center>
            <form action="SearchResults.aspx" method="get" target="_self">
                Search:&nbsp;<asp:TextBox ID="Search_input" runat="server"></asp:TextBox>
&nbsp;&nbsp;</form>
            <asp:Button ID="Search_button" runat="server" OnClick="Search_button_Click1" Text="Search" />
        </center>
    </div>
    <br />
    <div id="listContainer">
        <div class="listControl">
            <a id="expandList">Expand All</a>
            <a id="collapseList">Collapse All</a>
        </div>
        <ul id="expList">
            <li>artiface
                <ul>
                    <li>from
                        <ul>
                            <li>antler
                                <ul>
                                    <li>
                                        <b>Antler Artifact</b><br />
                                        Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>blade
                <ul>
                    <li>of
                        <ul>
                            <li>tool
                                <ul>
                                    <li>for
                                        <ul>
                                            <li>carving
                                                <ul>
                                                    <li>wood
                                                        <ul>
                                                            <li>
                                                                <b>Adze Blade</b> <br />
                                                                Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a>
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>clay
                <ul>
                    <li>for
                        <ul>
                            <li>building
                                <ul>
                                    <li>
                                        <b>Adobe</b> <br />
                                        Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>head
                <ul>
                    <li>of
                        <ul>
                            <li>tool
                                <ul>
                                    <li>for
                                        <ul>
                                            <li>carving
                                                <ul>
                                                    <li>wood
                                                        <ul>
                                                            <li>
                                                                <b>Adze Head</b> <br />
                                                                Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a>
                                                            </li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>tool
                <ul>
                    <li>for
                        <ul>
                            <li>smoothing
                                <ul>
                                    <li>
                                        <b>Abrader</b> <br />
                                        Source/Stored at: <a href="" target="_blank">http://www.someplace.com</a>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                </ul>
            </li>
        </ul>
        </div>
</asp:Searchpart>
