using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Models;

namespace TestApi.Data;

public class ShoppingListContext : DbContext
{
    public ShoppingListContext (DbContextOptions<ShoppingListContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingList>().Property(l => l.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Item>().Property(i => i.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Item>()
            .HasOne(item => item.ShoppingList)
            .WithMany(list => list.Items);
    }

    public DbSet<ShoppingList> ShoppingLists { get; set; } = null!;

    public DbSet<Item> Items { get; set; } = null!;
}
