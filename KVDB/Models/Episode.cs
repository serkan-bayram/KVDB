namespace KVDB.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string YoutubeId { get; set; }
    }
}
