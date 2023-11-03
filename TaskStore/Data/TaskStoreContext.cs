using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskStore.Models;

namespace TaskStore.Data
{
    public class TaskStoreContext : DbContext
    {
        public TaskStoreContext (DbContextOptions<TaskStoreContext> options)
            : base(options)
        {
        }

        public DbSet<TaskStore.Models.Store> Store { get; set; } = default!;

        public DbSet<TaskStore.Models.Product>? Product { get; set; }
    }
}
