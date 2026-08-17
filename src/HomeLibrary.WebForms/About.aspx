<%@ Page Title="Информация" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="HomeLibrary.WebForms.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <h3>Домашняя библиотека — ASP.NET Web Forms.</h3>
        <p>Приложение для управления домашней библиотекой книг.</p>
    </main>
</asp:Content>
