using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.ImasCGImages.Models
{
    public sealed   class SearchCondition
    {
        public string Hash { get; set; }

        public string Headline { get; set; }

        public TextOperator HeadlineOperator { get; set; } = TextOperator.Contains;

        public string Name { get; set; }

        public TextOperator NameOperator { get; set; }

        public string Kana { get; set; }

        public TextOperator KanaOperator { get; set; }

        public IdolRarity Rarity { get; set; }

        public bool? IsPlus { get; set; }

        public IdolType IdolType { get; set; }

        public int Count { get; set; } = 32;
    }
}
