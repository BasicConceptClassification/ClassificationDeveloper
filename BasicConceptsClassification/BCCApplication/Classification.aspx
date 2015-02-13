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
    <div>
        <h3><a href="mailto:admin@example.com?subject=Term Suggestion">Suggest new term?</a></h3>
    </div>
</asp:Content>
