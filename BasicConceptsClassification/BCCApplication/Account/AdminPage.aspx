<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="BCCApplication.Account.AdminPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">

        .horizontal li
        {
            float:left;
            width:99px;
            text-align:center;
            line-height:28px;
            height:28px;
            cursor:pointer;
            border-left:#A8C29F solid 1px;
            color:#666;
            font-size:14px;
            overflow:hidden;
        }

        .horizontal {
            width: 426px;
        }
        .list1 {
            width: 512px;
        }

    </style>
    <script type="text/javascript">
        function setTab(name, cursel) {

            cursel_0 = cursel;

            for (var i = 1; i <= links_len; i++) {

                var menu = document.getElementById(name + i);

                var menudiv = document.getElementById("con_" + name + "_" + i);

                if (i == cursel) {

                    menu.className = "off";

                    menudiv.style.display = "block";

                }

                else {

                    menu.className = "";

                    menudiv.style.display = "none";

                }

            }

        }

        function Next() {

            cursel_0++;

            if (cursel_0 > links_len) cursel_0 = 1

            setTab(name_0, cursel_0);

        }

        var name_0 = 'one';

        var cursel_0 = 1;

        var links_len, iIntervalId;

        onload = function () {

            var links = document.getElementById("tab1").getElementsByTagName('li')

            links_len = links.length;

            for (var i = 0; i < links_len; i++) {

                links[i].onmouseover = function () {

                    clearInterval(iIntervalId);

                }

            }

            document.getElementById("con_" + name_0 + "_" + links_len).parentNode.onmouseover = function () {

                clearInterval(iIntervalId);

            }

            setTab(name_0, cursel_0);

        }

    </script>
    <div>
        <div>
            <table style="width:100%">
                <tr>
                    <td colspan="8">
                        <center>
                            <h1>The Classification</h1>
                        </center>
                    </td>
                </tr>
                <tr>
                    <td colspan="8">
                        <p>You can edit the Classification by doing the following....</p>
                    </td>
                </tr>
                <tr>
                    <td rowspan="5" colspan="4" id="Datalist1" >
                        <div style="width:512px">
                         <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
                            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            <ParentNodeStyle Font-Bold="False" />
                            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                         </asp:TreeView>
                            </div>
                    </td>
                    <td colspan="4">
                        <div id="tab1">
                            <div>
                                <ul class="horizontal">
                                    <li id="one1" onclick="setTab('one',1)">ADD</li>
                                    <li id="one2" onclick="setTab('one',2)">MOVE</li>
                                    <li id="one3" onclick="setTab('one',3)">RENAME</li>
                                    <li id="one4" onclick="setTab('one',4)">DELETE</li>
                                </ul>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" rowspan="4">
                        <div>
                            <div id="con_one_1">
                                <table>
                                    <tr>
                                        <td>
                                            <p>New Term Name:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="NewTermTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="NewTermTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>Put Term Under:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ParentTermTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ParentTermTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />

                                        </td>
                                        <td>
                                            <asp:Button ID="AddUpdateButton" runat="server" Text="Update" OnClick="AddUpdateButton_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td />
                                        <td />
                                            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                                        <td>
                                            <asp:Button ID="AddAddButton" runat="server" Text="ADD" OnClick="AddAddButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="con_one_2" style="display:none">
                                <table>
                                    <tr>
                                        <td>
                                            <p>Move Term:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="MoveTermTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="MoveTermTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                        <td>
                                            <asp:Button ID="MoveUpdateButton1" runat="server" Text="Update" OnClick="MoveUpdateButton1_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>To Go Under Term:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="MoveTermUnderTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="MoveTermUnderTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                        <td>
                                            <asp:Button ID="MoveUpdateButton2" runat="server" Text="Update" OnClick="MoveUpdateButton2_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td />
                                        <td />
                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>

                                        <td>
                                            <asp:Button ID="MoveAddButton" runat="server" Text="ADD" OnClick="MoveAddButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="con_one_3" style="display:none">
                                <table>
                                    <tr>
                                        <td>
                                            <p>Term to Rename:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="RenameTermTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="RenameTermTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>Rename to:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="RenameToTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="RenameToTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                        <td>
                                            <asp:Button ID="RenameUpdateButton" runat="server" Text="Update" OnClick="RenameUpdateButton_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td />
                                        <td />
                                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>

                                        <td>
                                            <asp:Button ID="RenameAddButton" runat="server" Text="ADD" OnClick="RenameAddButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="con_one_4" style="display:none">
                                <table>
                                    <tr>
                                        <td>
                                            <p>Delete Term:</p>
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="DeleteTextBox" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="DeleteTextBox" CssClass="field-validation-error" ErrorMessage="you need to enter the term" />
                                        </td>
                                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>

                                        <td>
                                            <asp:Button ID="DeleteUpdateButton" runat="server" Text="Detele" OnClick="DeleteUpdateButton_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <p>GLAM Objects Affected</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="list1" colspan="2">
                                            <select id="DeleteSafely" size="8" runat="server" name="Name"></select>
                                        </td>
                                        <td class="list1" colspan="2">
                                            <select id="DeleteOverwrite" size="8" runat="server" name="ConceptString"></select>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="DeleteSafelyButton" runat="server" Text="Delete Safely" OnClick="DeleteSafelyButton_Click" />
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="DeleteOverwriteButton" runat="server" Text="Delete Term and Overwrite" OnClick="DeleteOverwriteButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>