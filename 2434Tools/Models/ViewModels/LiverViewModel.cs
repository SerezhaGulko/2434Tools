using System;
using System.ComponentModel.DataAnnotations;

namespace _2434Tools.Models.ViewModels
{
    public class LiverViewModel
    {
        [Required]
        public String ChannelId     { get; set; }
        [Required]
        public String Name          { get; set; }
        [Required]
        public String TwitterLink   { get; set; }
        [Required]
        public Int32 GroupId        { get; set; }
        public Boolean Graduated    { get; set; }
    }

}
