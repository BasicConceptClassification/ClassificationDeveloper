<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="BCCApplication.Account.AdminPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        *{list-style:none;margin:0;padding:0;overflow:hidden}

        .tab1{width:401px;border-top:#A8C29F solid 1px;border-bottom:#A8C29F solid 1px;margin:50px 200px;}

        .menu{width:400px;background:#eee;height:28px;border-right:#A8C29F solid 1px;border-bottom:#A8C29F solid 1px;}

        li{float:left;width:99px;text-align:center;line-height:28px;height:28px;cursor:pointer;border-left:#A8C29F solid 1px;color:#666;font-size:14px;overflow:hidden}

        .menudiv{width:399px;height:300px;border-left:#A8C29F solid 1px;border-right:#A8C29F solid 1px;border-top:0;background:#fefefe}

        .menudiv div{padding:15px;line-height:28px;}

        .off{background:#E0E2EB;color:#336699;font-weight:bold}

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
                    <td>
                        <center>
                            <h1>The Classification</h1>
                        </center>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>You can edit the Classification by doing the following....</p>
                    </td>
                </tr>
                <tr>
                    <td id="Datalist1">
                         <asp:TreeView ID="DataSet" runat="server" ImageSet="Arrows" OnSelectedNodeChanged="DataSet_SelectedNodeChanged">
                            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                            <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            <ParentNodeStyle Font-Bold="False" />
                            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px" VerticalPadding="0px" />
                         </asp:TreeView>
                    </td>
                    <td>
                        <div class="tab1" id="tab1">
                            <div class="menu">
                                <ul>
                                    <li id="one1" onclick="setTab('one',1)">ADD</li>
                                    <li id="one2" onclick="setTab('one',2)">MOVE</li>
                                    <li id="one3" onclick="setTab('one',3)">RENAME</li>
                                    <li id="one4" onclick="setTab('one',4)">DELETE</li>
                                </ul>
                            </div>
                        </div>
                        <div class="menudiv">
                            <div id="con_one_1">
                                <table>
                                    <tr>
                                        <td>
                                            <p>New Term Name:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="NewTermTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>Put Term Under:</p>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ParentTermTextBox" runat="server"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Button ID="AddUpdateButton" runat="server" Text="Update" OnClick="AddUpdateButton_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td />
                                        <td />
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
                                        </td>
                                        <td>
                                            <asp:Button ID="MoveUpdateButton2" runat="server" Text="Update" OnClick="MoveUpdateButton2_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td />
                                        <td />
                                        <td>
                                            <asp:Button ID="MoveAddButton" runat="server" Text="ADD" OnClick="MoveAddButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="con_one_3" style="display:none"><p>Hello</p></div>
                            <div id="con_one_4" style="display:none"><p>Ni Hao</p></div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>