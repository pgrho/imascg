using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public sealed class ImasCDDbContext : DbContext
    {
        public ImasCDDbContext() { }
        public ImasCDDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        public DbSet<Idol> Idols { get; set; }
        public DbSet<IdolImage> IdolImages { get; set; }
        public DbSet<IdolImageData> IdolImageData { get; set; }

        public async Task<int> SaveIdolsAsync(IEnumerable<Idol> idols)
        {
            var idolDictionary = idols.ToDictionary(_ => _.Id);

            var dbIdols = await Idols.ToDictionaryAsync(_ => _.Id);
            var c = 0;

            foreach (var kv in idolDictionary)
            {
                Idol dbIdol;
                if (dbIdols.TryGetValue(kv.Key, out dbIdol))
                {
                    // 更新しない
                    //dbIdol.Name = kv.Value.Name;
                    //dbIdol.Kana = kv.Value.Kana;
                }
                else
                {
                    Idols.Add(kv.Value);
                    c++;
                }
            }

            await SaveChangesAsync();

            return c;
        }
    }
}