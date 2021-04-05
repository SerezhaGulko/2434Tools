using System;

namespace _2434Tools.Models
{
    public class Video
    {
        public String Id                { get; set; }
        public String Title             { get; set; }
        public String PictureUrl        { get; set; }
        public String Description       { get; set; }
        public DateTime? Published      { get; set; }
        public UInt32 Duration          { get; set; }
        public UInt32 Views             { get; set; }
        public UInt32 Viewers           { get; set; }
        public UInt32 PeakViewers       { get; set; }
        public DateTime? LiveStartTime  { get; set; }
        public DateTime? LiveEndTime    { get; set; }
        public VideoStatus Status       { get; set; }

        public Int32 LiverId            { get; set; }
        public Liver Creator            { get; set; }
    }
}
