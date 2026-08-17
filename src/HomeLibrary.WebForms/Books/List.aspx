<%@ Page Title="Книги" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="HomeLibrary.WebForms.Books.List" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Домашняя библиотека</h1>

    <asp:Literal ID="MessageLiteral" runat="server" />

    <div class="row g-2 mb-3">
        <div class="col-auto">
            <asp:TextBox ID="SearchBox" runat="server" CssClass="form-control" placeholder="Поиск по названию или автору" />
        </div>
        <div class="col-auto">
            <asp:Button ID="SearchButton" runat="server" Text="Найти" CssClass="btn btn-outline-primary" OnClick="SearchButton_Click" />
            <asp:Button ID="ResetButton" runat="server" Text="Сбросить" CssClass="btn btn-outline-secondary" OnClick="ResetButton_Click" CausesValidation="false" />
        </div>
        <div class="col-auto ms-auto">
            <a class="btn btn-success" href="<%= ResolveUrl("~/Books/Edit.aspx") %>">Добавить книгу</a>
        </div>
    </div>

    <asp:GridView ID="BooksGrid" runat="server" AutoGenerateColumns="false"
                  CssClass="table table-striped table-hover" GridLines="None"
                  DataKeyNames="Id" OnRowCommand="BooksGrid_RowCommand"
                  EmptyDataText="Книги не найдены.">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Название" />
            <asp:BoundField DataField="Author" HeaderText="Автор" />
            <asp:BoundField DataField="PublicationYear" HeaderText="Год" />
            <asp:BoundField DataField="Genre" HeaderText="Жанр" />
            <asp:TemplateField HeaderText="Действия">
                <ItemTemplate>
                    <a class="btn btn-sm btn-outline-info" href='<%# ResolveUrl("~/Books/Details.aspx?id=" + Eval("Id")) %>'>Просмотр</a>
                    <a class="btn btn-sm btn-outline-primary" href='<%# ResolveUrl("~/Books/Edit.aspx?id=" + Eval("Id")) %>'>Изменить</a>
                    <asp:Button runat="server" CssClass="btn btn-sm btn-outline-danger" Text="Удалить"
                                CommandName="DeleteBook" CommandArgument='<%# Eval("Id") %>'
                                OnClientClick="return confirm('Удалить книгу?');" CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
