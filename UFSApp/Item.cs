using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFSApp
{
    public class Item
    {
        public string Name { get; set; }

        public FileInfo FInfo { get; set; }

        public long Size
        {
            get
            {
                if (FInfo != null)
                {
                    return FInfo.Length / 1024;
                }
                return 0;
            }
        }

        public string Image { get; set; }
    }
}
