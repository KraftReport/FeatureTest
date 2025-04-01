using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieDemoWithBlazor.Model;

namespace MovieDemoWithBlazor.Data
{
    public class MovieDemoWithBlazorContext : DbContext
    {
        public MovieDemoWithBlazorContext (DbContextOptions<MovieDemoWithBlazorContext> options)
            : base(options)
        {
        }

        public DbSet<MovieDemoWithBlazor.Model.Movie> Movie { get; set; } = default!;
    }
}
