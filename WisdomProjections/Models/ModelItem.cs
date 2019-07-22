using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomProjections.Views;

namespace WisdomProjections.Models
{
   public class ModelItem
    {
        public string Name { get; set; }
        public BaseBlob View { get; set; }
    }
}
