using System;
using System.Collections.Generic;

namespace _2434Tools.Models
{
    public class Group
    {
        public Int32 Id                     { get; set; }
        public String Name                  { get; set; }

        public ICollection<Liver> Livers    { get; set; }
    }
}
