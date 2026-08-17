<%@ Page Title="Книга" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="HomeLibrary.WebForms.Books.Edit" %>
<asp:Content ID="head" ContentPlaceHolderID="MainContent" runat="server">
    <h1><asp:Literal ID="HeadingLiteral" runat="server" /></h1>

    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger" />

    <div class="row">
        <div class="col-md-6 mb-3">
            <label class="form-label">Название</label>
            <asp:TextBox ID="TitleBox" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="TitleBox"
                ErrorMessage="Укажите название" CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="col-md-6 mb-3">
            <label class="form-label">Автор</label>
            <asp:TextBox ID="AuthorBox" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="AuthorBox"
                ErrorMessage="Укажите автора" CssClass="text-danger" Display="Dynamic" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-3 mb-3">
            <label class="form-label">Год издания</label>
            <asp:TextBox ID="YearBox" runat="server" CssClass="form-control" TextMode="Number" />
        </div>
        <div class="col-md-3 mb-3">
            <label class="form-label">Число страниц</label>
            <asp:TextBox ID="PagesBox" runat="server" CssClass="form-control" TextMode="Number" />
        </div>
        <div class="col-md-6 mb-3">
            <label class="form-label">Издательство</label>
            <asp:TextBox ID="PublisherBox" runat="server" CssClass="form-control" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-6 mb-3">
            <label class="form-label">ISBN</label>
            <asp:TextBox ID="IsbnBox" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-6 mb-3">
            <label class="form-label">Жанр</label>
            <asp:TextBox ID="GenreBox" runat="server" CssClass="form-control" />
        </div>
    </div>

    <div class="mb-3">
        <label class="form-label fw-bold">Оглавление</label>
        <p class="text-muted small mb-2">
            Заполните главы книги. У каждой главы можно добавить разделы (подпункты).
            Номер страницы указывать необязательно. Оглавление сохраняется в XML-поле базы данных.
        </p>

        <div id="toc-editor" class="border rounded p-3 bg-light">
            <div id="toc-chapters"></div>
            <button type="button" class="btn btn-outline-primary btn-sm mt-2" id="toc-add-chapter">
                + Добавить главу
            </button>
        </div>
        <asp:HiddenField ID="TocJsonHidden" runat="server" ClientIDMode="Static" />
    </div>

    <asp:Button ID="SaveButton" runat="server" Text="Сохранить" CssClass="btn btn-primary" OnClick="SaveButton_Click" OnClientClick="syncTocJson();" />
    <a class="btn btn-secondary" href="<%= ResolveUrl("~/Books/List.aspx") %>">Отмена</a>

    <script>
        (function () {
            var chaptersContainer = document.getElementById('toc-chapters');
            var addChapterBtn = document.getElementById('toc-add-chapter');
            var hiddenField = document.getElementById('TocJsonHidden');

            function escapeHtml(str) {
                if (!str) return '';
                return String(str).replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
            }

            function buildChapter(title, page) {
                var card = document.createElement('div');
                card.className = 'toc-chapter card mb-2';
                card.innerHTML =
                    '<div class="card-body py-2">' +
                    '  <div class="row g-2 align-items-center">' +
                    '    <div class="col-auto"><span class="badge bg-secondary">Глава</span></div>' +
                    '    <div class="col"><input class="form-control form-control-sm toc-ch-title" placeholder="Название главы" value="' + escapeHtml(title || '') + '" /></div>' +
                    '    <div class="col-3"><input class="form-control form-control-sm toc-ch-page" placeholder="Стр." value="' + escapeHtml(page || '') + '" /></div>' +
                    '    <div class="col-auto"><button type="button" class="btn btn-outline-danger btn-sm toc-remove-chapter" title="Удалить главу">✕</button></div>' +
                    '  </div>' +
                    '  <div class="toc-sections mt-2 ms-4"></div>' +
                    '  <button type="button" class="btn btn-outline-secondary btn-sm mt-1 ms-4 toc-add-section">+ Добавить раздел</button>' +
                    '</div>';
                return card;
            }

            function buildSection(title, page) {
                var row = document.createElement('div');
                row.className = 'toc-section row g-2 align-items-center mb-1';
                row.innerHTML =
                    '<div class="col-auto"><span class="badge bg-light text-dark">Раздел</span></div>' +
                    '<div class="col"><input class="form-control form-control-sm toc-sec-title" placeholder="Название раздела" value="' + escapeHtml(title || '') + '" /></div>' +
                    '<div class="col-3"><input class="form-control form-control-sm toc-sec-page" placeholder="Стр." value="' + escapeHtml(page || '') + '" /></div>' +
                    '<div class="col-auto"><button type="button" class="btn btn-outline-danger btn-sm toc-remove-section" title="Удалить раздел">✕</button></div>';
                return row;
            }

            window.syncTocJson = function () {
                var chapters = [];
                var chCards = chaptersContainer.querySelectorAll('.toc-chapter');
                chCards.forEach(function (card) {
                    var chTitle = (card.querySelector('.toc-ch-title').value || '').trim();
                    var chPage = (card.querySelector('.toc-ch-page').value || '').trim();
                    var sections = [];
                    var secRows = card.querySelectorAll('.toc-section');
                    secRows.forEach(function (row) {
                        var sTitle = (row.querySelector('.toc-sec-title').value || '').trim();
                        var sPage = (row.querySelector('.toc-sec-page').value || '').trim();
                        if (sTitle || sPage) {
                            sections.push({ Title: sTitle, Page: sPage || null });
                        }
                    });
                    if (chTitle || chPage || sections.length > 0) {
                        chapters.push({ Title: chTitle, Page: chPage || null, Sections: sections });
                    }
                });
                hiddenField.value = JSON.stringify(chapters);
            };

            // Init from existing JSON
            if (hiddenField && hiddenField.value) {
                try {
                    var initialData = JSON.parse(hiddenField.value);
                    if (Array.isArray(initialData)) {
                        initialData.forEach(function (ch) {
                            var chCard = buildChapter(ch.Title, ch.Page);
                            var secContainer = chCard.querySelector('.toc-sections');
                            if (ch.Sections && Array.isArray(ch.Sections)) {
                                ch.Sections.forEach(function (sec) {
                                    secContainer.appendChild(buildSection(sec.Title, sec.Page));
                                });
                            }
                            chaptersContainer.appendChild(chCard);
                        });
                    }
                } catch (e) { }
            }

            if (addChapterBtn) {
                addChapterBtn.addEventListener('click', function () {
                    chaptersContainer.appendChild(buildChapter('', ''));
                });
            }

            chaptersContainer.addEventListener('click', function (e) {
                var target = e.target;
                if (target.classList.contains('toc-remove-chapter')) {
                    var chapter = target.closest('.toc-chapter');
                    if (chapter) { chapter.remove(); }
                    return;
                }
                if (target.classList.contains('toc-add-section')) {
                    var card = target.closest('.toc-chapter');
                    if (!card) { return; }
                    var sectionsBox = card.querySelector('.toc-sections');
                    sectionsBox.appendChild(buildSection('', ''));
                    return;
                }
                if (target.classList.contains('toc-remove-section')) {
                    var section = target.closest('.toc-section');
                    if (section) { section.remove(); }
                    return;
                }
            });

            // Ensure sync before form submit
            var form = document.forms[0];
            if (form) {
                form.addEventListener('submit', window.syncTocJson);
            }
        })();
    </script>
</asp:Content>
