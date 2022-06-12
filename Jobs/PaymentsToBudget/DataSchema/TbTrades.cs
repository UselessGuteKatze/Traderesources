using System;
using System.Linq;
using YodaQuery;

namespace PaymentsToBudget.DataSchema {
    public class TbTrades : QueryTable {
        public TbTrades(Sources source) : base(new[] { Sources.dbLands }.Contains(source) ? "tblandobjectstrades" : "tbtrades", "") {
            Fields = new Field[] {
                new IntField(nameof(flId), "Id"),
                new JsonField<WinnerData>(nameof(flWinnerData), "Данные покупателя")
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

        public IntField flId => (IntField)this[nameof(flId)];
        public JsonField<WinnerData> flWinnerData => (JsonField<WinnerData>)this[nameof(flWinnerData)];

        public enum Sources
        {
            dbHunting,
            dbFishing,
            dbTrades,
            dbLands,
        }
    }

    public class WinnerData {
        public string Xin { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CorpName { get; set; }
        public bool IsCorpUser { get; set; }
        public string FirstPersonName { get; set; }
        public string AddressInfo { get; set; }
        public string ContactInfo { get; set; }
        public string FullOrgXinName {
            get {
                return UserType switch {
                    "Individual" => $"{LastName} {FirstName} {MiddleName}, ИИН {Xin}",
                    "IndividualCorp" => $"{CorpName} {FirstPersonName}, ИИН {Xin}",
                    "Corporate" => $"\"{CorpName}\" {FirstPersonName}, БИН {Xin}",
                    _ => $"{LastName} {FirstName} {MiddleName} \"{CorpName}\" {FirstPersonName}, ИИН/БИН {Xin}"
                };
            }
        }
        public string FullOrgName {
            get {
                return UserType switch {
                    "Individual" => $"{LastName} {FirstName} {MiddleName}",
                    "IndividualCorp" => $"\"{CorpName}\" {FirstPersonName}",
                    "Corporate" => $"\"{CorpName}\" {FirstPersonName}",
                    _ => $"{LastName} {FirstName} {MiddleName} \"{CorpName}\" {FirstPersonName}"
                };
            }
        }
        public string FullName {
            get {
                return $"{LastName} {FirstName} {MiddleName}";
            }
        }
        public WinnerBankDetails ParticipiantBankDetails { get; set; }
    }

    public class WinnerBankDetails {
        public string KBE { get; set; }
        public string IIK { get; set; }
        public string BIK { get; set; }
        public string BankName { get; set; }
    }
}
