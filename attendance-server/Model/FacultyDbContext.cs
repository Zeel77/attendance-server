using Microsoft.EntityFrameworkCore;

namespace attendance_server.Model
{
    public class FacultyDbContext:DbContext
    {
        public FacultyDbContext()
        {

        }
        public FacultyDbContext(DbContextOptions<FacultyDbContext> options) : base(options)
        {

        }

        public DbSet<FacultyModel> Faculty { get; set; }
    }
}
