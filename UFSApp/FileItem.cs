using System.IO;
using System.Security.Principal;

namespace UFSApp
{
    public class FileItem : PropertyChangedBase
    {
        public FileInfo FInfo { get; set; }

        public string SizeString
        {
            get
            {
                if (FInfo != null)
                {
                    return (FInfo.Length / 1024).ToString() + " КБ";
                }
                return "0 КБ";
            }
        }

        public string Owner
        {
            get
            {
                if (FInfo != null)
                {
                    return File.GetAccessControl(FInfo.FullName).GetOwner(typeof(NTAccount)).ToString();
                }
                return "<неизвестно>";
            }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set { SetField(ref _checked, value); }
        }

        public string Image { get; set; }
    }
}
