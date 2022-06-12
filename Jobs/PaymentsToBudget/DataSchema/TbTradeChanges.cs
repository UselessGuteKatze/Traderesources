using System;
using System.Linq;
using YodaQuery;

namespace PaymentsToBudget.DataSchema {
    public class TbTradeChanges: QueryTable {

        public TbTradeChanges(Sources source) : base(new[] { Sources.dbLands }.Contains(source) ? "tblandobjecttradechanges" : "tbtradechanges", "") {
            Fields = new Field[] {
                new IntField(nameof(flTradeId), "Trade Id"),
                new IntField(nameof(flAuctionId), "Auction Id")
            };
            DbKey = source switch
            {
                Sources.dbTrades => "dbTradeResources",
                Sources.dbLands => "dbTradeResources",
                Sources.dbHunting => "dbHunting",
                Sources.dbFishing => "dbFishing",
                _ => throw new NotImplementedException($"Unknown source {source}")
            };
        }
        public IntField flTradeId => (IntField)this[nameof(flTradeId)];
        public IntField flAuctionId => (IntField)this[nameof(flAuctionId)];

        public enum Sources
        {
            dbHunting,
            dbFishing,
            dbTrades,
            dbLands,
        }

    }
}
