<%@ Page Title="Контакты" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="HomeLibrary.WebForms.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <h3>Домашняя библиотека.</h3>
        <address>
            Тестовый проект<br />
            MS SQL Server + ASP.NET Web Forms
        </address>
    </main>
</asp:Content>
