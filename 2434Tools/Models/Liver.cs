using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _2434Tools.Models
{
    public class Liver
    {
        public Int32 Id { get; set; }
        [Required]
        public String ChannelId { get; set; }
        public String UploadsId { get; set; }
        public String PictureURL { get; set; }
        public String Name { get; set; }
        public String ChannelName { get; set; }
        public String Description { get; set; }
        [Required]
        public String TwitterLink { get; set; }
        public UInt32 Subscribers { get; set; }
        public UInt64 Views { get; set; }
        public Boolean Graduated { get; set; }
        public DateTime FeedChecked { get; set; }

        public Int32 GroupId { get; set; }
        public Group Group { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
