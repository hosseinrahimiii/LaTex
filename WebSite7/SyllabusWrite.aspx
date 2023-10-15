<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SyllabusWrite.aspx.cs" Inherits="CS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 10pt;
        }
        table
        {
            border: 1px solid #ccc;
            border-collapse: collapse;
            background-color: #fff;
        }
        table th
        {
            background-color: #B8DBFD;
            color: #333;
            font-weight: bold;
        }
        table th, table td
        {
            padding: 5px;
            border: 1px solid #ccc;
        }
        table, table table td
        {
            border: 0px solid #ccc;
        }
        .text-danger {
            color: #ff0e0e;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:PlaceHolder runat="server" ID="fillInsert" Visible="true">
            <div id="dvGrid" style="padding: 0px; width: 1200px">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowDataBound="OnRowDataBound"
                            DataKeyNames="Id" OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit"
                            PageSize="3" AllowPaging="true" OnPageIndexChanging="OnPaging" OnRowUpdating="OnRowUpdating"
                            OnRowDeleting="OnRowDeleting" EmptyDataText="No records has been added." Width="500" Height="150">
                            <Columns>
                                <asp:TemplateField HeaderText="User Name" ItemStyle-Width="70%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("user_name") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtUserName" runat="server" Text='<%# Eval("user_name") %>' Width="80%"></asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ButtonType="Link" ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="30"
                                    HeaderText="Actions" ItemStyle-HorizontalAlign="Center"/>
                            </Columns>
                        </asp:GridView>
                        <table border="1" cellpadding="0" cellspacing="0" style="border-collapse: collapse" width="500" height="70">
                            <tr>
                                <td style="width: 53%">
                                    <asp:Label Text='User Name:' Width="40%" runat="server"></asp:Label>
                                    <asp:TextBox ID="txtUserName" runat="server" Width="55%" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUserName"
                                        CssClass="text-danger" ErrorMessage="The User Name field is required."
                                        Display="Dynamic" ValidationGroup="fillInsert" />
                                </td>
                                <td style="width: 12%" align="center">
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" Width="120" OnClick="Insert" ValidationGroup="fillInsert" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:PlaceHolder>
    </form>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.js"></script>
    <script type="text/javascript">
        $(function () {
            BlockUI("dvGrid");
            $.blockUI.defaults.css = {};
        });
        function BlockUI(elementID) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                $("#" + elementID).block({ message: '<div align = "center">' + '<img src="images/loadingAnim.gif"/></div>',
                    css: {},
                    overlayCSS: { backgroundColor: '#000000', opacity: 0.6, border: '3px solid #63B2EB' }
                });
            });
            prm.add_endRequest(function () {
                $("#" + elementID).unblock();
            });
        };
    </script>
</body>
</html>
