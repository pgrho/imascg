using HtmlAgilityPack;
using Shipwreck.ImasCGImages.Crawler.Properties;
using Shipwreck.ImasCGImages.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Crawler
{
    class Program
    {
        private static void Main(string[] args)
        {
            var cb = new SqlConnectionStringBuilder();
            cb.ConnectionString = Settings.Default.ConnectionString;

            Console.Write("Data Source (default:{0}):", cb.DataSource);
            cb.DataSource = ReadLine() ?? cb.DataSource;

            Console.Write("Initial Catalog (default:{0}):", cb.InitialCatalog);
            cb.InitialCatalog = ReadLine() ?? cb.InitialCatalog;

            Console.Write("Integrated Security (default:{0}):", cb.IntegratedSecurity);
            cb.IntegratedSecurity = Regex.IsMatch(ReadLine() ?? cb.IntegratedSecurity.ToString(), "^(t|true|y|yes)$", RegexOptions.IgnoreCase);

            Console.Write("User ID (default:{0}):", cb.UserID);
            cb.UserID = ReadLine() ?? cb.UserID;

            Console.Write("Password (default:{0}):", string.IsNullOrEmpty(cb.Password) ? "" : "****");
            cb.Password = ReadLine() ?? cb.Password;

            var cs = cb.ConnectionString;

            Settings.Default.ConnectionString = cs;
            Settings.Default.Save();

            Task.Run(() => new Program(cs).MainAsync()).Wait();
        }

        private static string ReadLine()
        {
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            return s.Trim();
        }

        private readonly string _ConnectionString;
        private readonly bool _SkipImageList = true;
        private readonly int Wait = 500;

        private Program(string connectionString)
        {
            this._ConnectionString = connectionString;
        }

        private async Task MainAsync()
        {
            Console.WriteLine("アイドルの一覧をダウンロードしています。");
            var idols = await Idol.DownloadIdolsAsync();

            Console.WriteLine("アイドルの一覧を保存しています。");
            await SaveIdolsAsync(idols);

            await Task.Delay(Wait);

            if (!_SkipImageList)
            {
                var d = new HashSet<Tuple<string, string>>();
                for (var i = 0; i < idols.Count; i++)
                {
                    var idol = idols[i];

                    Console.WriteLine("[{0}/{1}]{2}の画像一覧をダウンロードしています。", i + 1, idols.Count, idol.Name);
                    var imgs = await idol.GetIdolImagesAsync(errorHandler: (n, v) =>
                    {
                        var t = Tuple.Create(n, v);
                        if (d.Add(t))
                        {
                            Console.WriteLine("不明な{0}:{1}", n, v);
                        }
                    });

                    Console.WriteLine("[{0}/{1}]{2}の画像一覧を保存しています。", i + 1, idols.Count, idol.Name);
                    await SaveIdolImagesAsync(imgs);

                    await Task.Delay(Wait);
                }
            }
            // TODO:nullのデータを消してリトライ

            var types = new[] { IdolImageDataType.Framed, IdolImageDataType.Frameless, IdolImageDataType.Quest, IdolImageDataType.LS, IdolImageDataType.XS };

            var tc = await GetUnaquiredImageCountAsync(types);
            var dc = 0;
            for (var img = await GetUnaquiredImageAsync(types); img != null; img = await GetUnaquiredImageAsync(types))
            {
                foreach (var t in types)
                {
                    if (await AnyImageDataAsync(img.Hash, t))
                    {
                        continue;
                    }
                    dc++;

                    Console.WriteLine("[{0}/{1}]{2}の{3}画像をダウンロードしています。", dc, tc, img.Headline, t);
                    var d = await GetImageDataAsync(img.Hash, t);

                    Console.WriteLine("[{0}/{1}]{2}の{3}画像を保存しています。", dc, tc, img.Headline, t);
                    await InsertImageDataAsync(img.Hash, t, d);

                    await Task.Delay(Wait);
                }
            }
        }

        private async Task<int> SaveIdolsAsync(IEnumerable<Idol> idols)
        {
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
          return      await db.SaveIdolsAsync(idols);
            }
        }

        private async Task SaveIdolImagesAsync(IEnumerable<IdolImage> images)
        {
            var imgDic = images.ToDictionary(_ => _.Hash);
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
                var dbImages = await db.IdolImages.ToDictionaryAsync(_ => _.Hash);

                foreach (var kv in imgDic)
                {
                    IdolImage d;
                    if (dbImages.TryGetValue(kv.Key, out d))
                    {
                        // 更新しない

                        //var s = kv.Value;

                        //d.Headline = !string.IsNullOrEmpty(s.Headline) ? s.Headline : d.Headline;
                        //d.Rarity = s.Rarity > 0 ? s.Rarity : d.Rarity;
                        //d.Type = s.Type > 0 ? s.Type : d.Type;
                        //d.BloodType = s.BloodType > 0 ? s.BloodType : d.BloodType;
                        //d.Height = s.Height > 0 ? s.Height : d.Height;
                        //d.Weight = s.Weight > 0 ? s.Weight : d.Weight;
                        //d.Bust = s.Bust > 0 ? s.Bust : d.Bust;
                        //d.Waist = s.Waist > 0 ? s.Waist : d.Waist;
                        //d.Hip = s.Hip > 0 ? s.Hip : d.Hip;
                        //d.Age = s.Age > 0 ? s.Age : d.Age;
                        //d.BirthMonth = s.BirthMonth > 0 ? s.BirthMonth : d.BirthMonth;
                        //d.BirthDay = s.BirthDay > 0 ? s.BirthDay : d.BirthDay;
                        //d.SunSign = s.SunSign > 0 ? s.SunSign : d.SunSign;
                        //d.Birthplace = !string.IsNullOrEmpty(s.Birthplace) ? s.Birthplace : d.Birthplace;
                        //d.Hobby = !string.IsNullOrEmpty(s.Hobby) ? s.Hobby : d.Hobby;
                        //d.Handedness = s.Handedness > 0 ? s.Handedness : d.Handedness;
                    }
                    else
                    {
                        if (kv.Value.Idol != null)
                        {
                            db.Idols.Attach(kv.Value.Idol);
                        }
                        db.IdolImages.Add(kv.Value);
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        private async Task<int> GetUnaquiredImageCountAsync(IdolImageDataType[] types)
        {
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
                var tc = await db.IdolImages.CountAsync();
                var dc = await db.IdolImageData.CountAsync(_ => _.Data != null);
                return tc * types.Length - dc;
            }
        }
        private async Task<IdolImage> GetUnaquiredImageAsync(IdolImageDataType[] types)
        {
            var c = types.Length;
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
                return await db.IdolImages.Include(_ => _.Idol)
                                .FirstOrDefaultAsync(_ => db.IdolImageData.Count(h => h.Hash == _.Hash) < c);
            }
        }

        private async Task<bool> AnyImageDataAsync(string hash, IdolImageDataType type)
        {
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
                return await db.IdolImageData.AnyAsync(_ => _.Hash == hash && _.Type == type);
            }
        }

        private async Task<byte[]> GetImageDataAsync(string hash, IdolImageDataType type)
        {
            var u = IdolImageData.GetImageUrl(hash, type);

            using (var wc = new WebClient())
            {
                try
                {
                    return await wc.DownloadDataTaskAsync(u);
                }
                catch
                {
                    return null;
                }
            }
        }

        private async Task InsertImageDataAsync(string hash, IdolImageDataType type, byte[] data)
        {
            using (var db = new ImasCDDbContext(_ConnectionString))
            {
                var d = await db.IdolImageData.FirstOrDefaultAsync(_ => _.Hash == hash && _.Type == type);

                if (d == null)
                {
                    d = new IdolImageData();
                    d.Hash = hash;
                    d.Type = type;
                    db.IdolImageData.Add(d);
                }
                d.Data = data;

                await db.SaveChangesAsync();
            }
        }
    }
}