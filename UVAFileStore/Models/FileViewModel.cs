using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace UVAFileStore.Models
{
    public class FileViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Файл")]
        public byte[] FileData { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string FileMimeType { get; set; }
    }
}