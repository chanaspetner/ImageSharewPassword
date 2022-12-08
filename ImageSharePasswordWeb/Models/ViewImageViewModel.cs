using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageSharePasswordData;

namespace ImageSharePasswordWeb.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public int Valid { get; set; }
        public string PasswordSession { get; set; }
    }
}
