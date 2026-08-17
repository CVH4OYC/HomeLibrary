<%@ Page Title="Главная" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HomeLibrary.WebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="text-center my-4">
        <p>Перенаправление в каталог книг...</p>
        <p><a href="<%= ResolveUrl("~/Books/List.aspx") %>" class="btn btn-primary">Перейти к книгам</a></p>
    </div>
</asp:Content>
