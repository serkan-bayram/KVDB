using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KVDB.Models;

namespace KVDB.Data
{
    public class KVDBContext : DbContext
    {
        public KVDBContext (DbContextOptions<KVDBContext> options)
            : base(options)
        {
        }

        public DbSet<KVDB.Models.Transcript> Transcript { get; set; } = default!;
        public DbSet<KVDB.Models.Episode> Episode { get; set; } = default!;
    }
}
