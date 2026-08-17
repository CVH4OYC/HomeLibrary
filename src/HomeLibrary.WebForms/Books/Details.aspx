<%@ Page Title="Карточка книги" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="HomeLibrary.WebForms.Books.Details" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false">
        <div class="alert alert-warning">Книга не найдена.</div>
        <a class="btn btn-secondary" href="<%= ResolveUrl("~/Books/List.aspx") %>">К списку</a>
    </asp:Panel>

    <asp:Panel ID="CardPanel" runat="server">
        <h1><asp:Literal ID="TitleLiteral" runat="server" /></h1>
        <h5 class="text-muted"><asp:Literal ID="AuthorLiteral" runat="server" /></h5>

        <dl class="row mt-3">
            <dt class="col-sm-3">Год издания</dt>
            <dd class="col-sm-9"><asp:Literal ID="YearLiteral" runat="server" /></dd>
            <dt class="col-sm-3">Издательство</dt>
            <dd class="col-sm-9"><asp:Literal ID="PublisherLiteral" runat="server" /></dd>
            <dt class="col-sm-3">ISBN</dt>
            <dd class="col-sm-9"><asp:Literal ID="IsbnLiteral" runat="server" /></dd>
            <dt class="col-sm-3">Число страниц</dt>
            <dd class="col-sm-9"><asp:Literal ID="PagesLiteral" runat="server" /></dd>
            <dt class="col-sm-3">Жанр</dt>
            <dd class="col-sm-9"><asp:Literal ID="GenreLiteral" runat="server" /></dd>
        </dl>

        <h4>Оглавление</h4>
        <div class="border rounded p-3 mb-3 bg-light">
            <asp:Literal ID="TocLiteral" runat="server" />
        </div>

        <a class="btn btn-primary" id="EditLink" runat="server">Изменить</a>
        <a class="btn btn-secondary" href="<%= ResolveUrl("~/Books/List.aspx") %>">К списку</a>
    </asp:Panel>
</asp:Content>
