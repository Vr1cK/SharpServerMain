using Newtonsoft.Json;
namespace SharpServerMain.Models
{

    public class RequestVideo
    {
        [JsonProperty("Bytevideo")]
        public byte[] Bytevideo { get; set; }

        [JsonProperty("Hash")]
        public uint Hash { get; set; }

        public RequestVideo(byte[] bytevideo, uint hash)
        {
            Bytevideo = bytevideo;
            Hash = hash;
        }
    }
}
