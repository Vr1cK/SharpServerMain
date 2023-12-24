using Newtonsoft.Json;

namespace SharpServerMain.Models
{
    public class Videoinfo
    {
        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("Flag")]
        public int Flag { get; set; }

        [JsonProperty("Bytevideo")]
        public byte[] ByteVideo { get; set; }

        [JsonProperty("Starttime")]
        public string StartTime { get; set; }

        [JsonProperty("Endtime")]
        public string EndTime { get; set; }

        [JsonProperty("Hash")]
        public uint Hash { get; set; }

        [JsonProperty("Newheight")]
        public int? NewHeight { get; set; }

        [JsonProperty("Newwidth")]
        public int? NewWidth { get; set; }

        [JsonProperty("URL")]
        public string URL { get; set; }
        public Videoinfo(int userID, int flag, byte[] byteVideo, string startTime, string endTime, uint hash, int? newHeight, int? newWidth, string url)
        {
            UserID = userID;
            Flag = flag;
            ByteVideo = byteVideo;
            StartTime = startTime;
            EndTime = endTime;
            Hash = hash;
            NewHeight = newHeight;
            NewWidth = newWidth;
            URL = url;
        }

    }

}
