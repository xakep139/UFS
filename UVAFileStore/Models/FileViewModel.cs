using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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