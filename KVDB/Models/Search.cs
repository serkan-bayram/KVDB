namespace KVDB.Models
{
    public class Search
    {
            public int ItemsFound { get; set; }
            public List<Transcript> TranscriptsList { get; set; }
            public int CurrentPage { get; set; }    
            public string SearchString { get; set; }    
    }
}
