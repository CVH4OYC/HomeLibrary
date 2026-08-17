using System;
using System.ComponentModel.DataAnnotations;

namespace HomeLibrary.WebForms.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название")]
        [Display(Name = "Название")]
        [StringLength(300)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Укажите автора")]
        [Display(Name = "Автор")]
        [StringLength(200)]
        public string Author { get; set; }

        [Display(Name = "Год издания")]
        public int? PublicationYear { get; set; }

        [Display(Name = "Издательство")]
        [StringLength(200)]
        public string Publisher { get; set; }

        [Display(Name = "ISBN")]
        [StringLength(20)]
        public string ISBN { get; set; }

        [Display(Name = "Число страниц")]
        public int? PageCount { get; set; }

        [Display(Name = "Жанр")]
        [StringLength(100)]
        public string Genre { get; set; }

        [Display(Name = "Оглавление (XML)")]
        public string TableOfContents { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
