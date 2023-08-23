using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.Entities.Site;

namespace TopNews.Infrastructure.Initializers
{
    internal static class CategoriesAnsPostsInitializer
    {
        public static void SeedCategoryAnsPosts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(new Category[]
            {
                new Category { Id = 1, Name = "Sport"},
                new Category { Id = 2, Name = "IT"}
            });
        }
    }
}
