using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace KVDB.Models
{
    public class Transcript
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public required decimal Start { get; set; }
        public required decimal Duration { get; set; }

        public int EpisodeId { get; set; }
        //This is the foreign key field.It stores the ID of the related Episode entity in the database.
        public required virtual Episode Episode { get; set; }
        //This is the navigation property.It provides a way to navigate from a Transcript entity to its associated Episode entity.
    }

    // transcript.json has the following structure
    public class TranscriptLine
    {
        public required string Text { get; set; }
        public decimal Start { get; set; }
        public decimal Duration { get; set; }
    }

}
