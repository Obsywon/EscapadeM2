﻿using Microsoft.EntityFrameworkCore;
using EscapadeApi.Models;

namespace EscapadeApi
{
    public class CosmosContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

        public CosmosContext(DbContextOptions<CosmosContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>().ToContainer(nameof(User))
                .HasNoDiscriminator()
                .HasPartitionKey(e => e.Id);

            modelBuilder
                .Entity<Post>().ToContainer(nameof(Post))
                .HasNoDiscriminator()
                .HasPartitionKey(e => e.Id);
        }
    }
}
