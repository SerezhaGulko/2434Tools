using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2434Tools.Models.ViewModels
{
    public class AjaxLiveVideo
    {
        public String VideoId           { get; set; }
        public String CreatorThumbId    { get; set; }
        public Int32 CreatorId          { get; set; }
        public AjaxLiveVideo(Video v)
        {
            this.VideoId = v.Id;
            this.CreatorThumbId = v.Creator.ThumbURL;
            this.CreatorId = v.LiverId;
        }
    }
}
