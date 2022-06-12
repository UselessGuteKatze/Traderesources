using System;
using System.Collections.Generic;
using Yoda.YodaReferences;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class TbAgreements : QueryTable
    {
        public TbAgreements() : base(nameof(TbAgreements), "Договоры")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull().Sequence(),
                new TextField(nameof(flAgreementNumber), "Номер договора", 20).NotNull(),
                new IntField(nameof(flAgreementRevisionId), "Id ревизии договора").NotNull(),
                new ReferenceTextField(nameof(flAgreementType), "Тип договора", RefAgreementTypes.RefName, 1, 100).NotNull(),
                new RefField<RefAgreementStatuses, AgreementStatuses>(nameof(flAgreementStatus), "Статус договора").NotNull(),
                new DateTimeField(nameof(flAgreementCreateDate), "Дата создания договора").NotNull(),
                new DateTimeField(nameof(flAgreementSignDate), "Дата подписания договора"),
                new IntField(nameof(flObjectId), "Id объекта").NotNull(),
                new ReferenceTextField(nameof(flObjectType), "Тип объекта", RefAgreementObjectTypes.RefName, 1, 100).NotNull(),
                new IntField(nameof(flTradeId), "Id торга"),
                new ReferenceTextField(nameof(flTradeType), "Тип торга", RefAgreementTradeTypes.RefName, 1, 100),
                new IntField(nameof(flAuctionId), "Id аукциона (ЭТП)"),
                new TextField(nameof(flAgreementCreatorBin), "Создатель договора", length: 12),
                new TextField(nameof(flSellerBin), "Продавец", length: 12),
                new TextField(nameof(flWinnerXin), "Победитель", length: 12),
            };
            DbKey = "dbAgreements";
            PrimaryKey = new TablePrimaryKey($"PK_{Name}", new[] { new IndexCol(nameof(flAgreementId)), new IndexCol(nameof(flAgreementRevisionId)) });
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flAgreementId)}", new[] { new IndexCol(nameof(flAgreementId)), new IndexCol(nameof(flAgreementNumber)) }, true)
            };
        }

        public DateTimeField flRequestDate => (DateTimeField)this[nameof(flRequestDate)];
        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public TextField flAgreementNumber => (TextField)this[nameof(flAgreementNumber)];
        public IntField flAgreementRevisionId => (IntField)this[nameof(flAgreementRevisionId)];
        public ReferenceTextField flAgreementType => (ReferenceTextField)this[nameof(flAgreementType)];
        public RefField<RefAgreementStatuses, AgreementStatuses> flAgreementStatus => (RefField<RefAgreementStatuses, AgreementStatuses>)this[nameof(flAgreementStatus)];
        public DateTimeField flAgreementCreateDate => (DateTimeField)this[nameof(flAgreementCreateDate)];
        public DateTimeField flAgreementSignDate => (DateTimeField)this[nameof(flAgreementSignDate)];
        public IntField flObjectId => (IntField)this[nameof(flObjectId)];
        public ReferenceTextField flObjectType => (ReferenceTextField)this[nameof(flObjectType)];
        public IntField flTradeId => (IntField)this[nameof(flTradeId)];
        public ReferenceTextField flTradeType => (ReferenceTextField)this[nameof(flTradeType)];
        public IntField flAuctionId => (IntField)this[nameof(flAuctionId)];
        public TextField flAgreementCreatorBin => (TextField)this[nameof(flAgreementCreatorBin)];
        public TextField flSellerBin => (TextField)this[nameof(flSellerBin)];
        public TextField flWinnerXin => (TextField)this[nameof(flWinnerXin)];

    }

    public class RefAgreementStatuses : ReferenceEnum<AgreementStatuses>
    {
        public const string RefName = nameof(RefAgreementStatuses);
        public RefAgreementStatuses() : base(RefName, "Статусы договора", new Dictionary<AgreementStatuses, string> {
            {AgreementStatuses.Saved, "Сохранён"},
            {AgreementStatuses.OnApproval, "На согласовании"},
            {AgreementStatuses.OnCorrection, "Отправлен на доработку"},
            {AgreementStatuses.Agreed, "Согласован"},
            {AgreementStatuses.SignedSeller, "Подписан продавцом"},
            {AgreementStatuses.SignedWinner, "Подписан победителем"},
            {AgreementStatuses.Signed, "Подписан сторонами"},
            {AgreementStatuses.Deleted, "Удалён"},
            {AgreementStatuses.Canceled, "Подписан акт об отмене результатов торгов"},
            {AgreementStatuses.Extended, "Период подписи продлён / Договор восстановлен"},
        })
        {
        }
    }

    public enum AgreementStatuses
    {
        Saved,
        OnApproval,
        OnCorrection,
        Agreed,
        SignedSeller,
        SignedWinner,
        Signed,
        Deleted,
        Canceled,
        Extended
    }

    public class RefAgreementTypes : Reference
    {
        public const string RefName = nameof(RefAgreementTypes);

        public RefAgreementTypes() : base(RefName, "Типы договоров")
        {
            Items = new ReferenceItemCollection {
                {Values.Lands.ToString(), "Земельные ресурсы", new ReferenceItemCollection
                    {
                        {"ДоговорКпЗемельногоУчастка", "Договор купли-продажи земельного участка"},
                        {"ДоговорКпПраваАрендыЗемельногоУчастка", "Договор купли-продажи права аренды земельного участка"},
                    }
                },
                {Values.Hunt.ToString(), "Охотничьи угодья", new ReferenceItemCollection
                    {
                        {"ДоговорНаВедениеОхотничьегоХозяйства", "Договор на ведение охотничьего хозяйства"},
                    }
                },
                {Values.Fish.ToString(), "Рыбохозяйственные водоёмы", new ReferenceItemCollection
                    {
                        {"ДоговорНаВедениеРыбногоХозяйстваОтрхИСрх", "Договор на ведение рыбного хозяйства (при ведении озерно-товарного рыбоводного хозяйства или садкового рыбоводного хозяйства)"},
                        {"ДоговорНаВедениеРыбногоХозяйстваПрхИЛрх", "Договор на ведение рыбного хозяйства (при ведении промыслового или любительского (спортивного) рыболовства)"},
                    }
                }
            };
        }

        public enum Values
        {
            Lands,
            Hunt,
            Fish
        }
    }

    public class RefAgreementTradeTypes : Reference
    {
        public const string RefName = nameof(RefAgreementTradeTypes);

        public RefAgreementTradeTypes() : base(RefName, "Типы торгов")
        {
            Items = new ReferenceItemCollection {
                {Values.Lands.ToString(), "Земельные ресурсы", new ReferenceItemCollection
                    {
                        {"PriceUpLand", "Земельные торги на повышение цены"},
                        {"PriceDownLand", "Земельные торги на понижение цены"},
                        {"CompetitionAgricultureLand", "Конкурс по предоставлению права аренды земельного участка сельскохозяйственного назначения"},
                    }
                },
                {Values.Hunt.ToString(), "Охотничьи угодья и рыбохозяйственные водоёмы", new ReferenceItemCollection
                    {
                        {"CompetitionHunt", "Конкурс по закреплению охотничьих угодий и рыбохозяйственных водоемов"},
                    }
                }
            };
        }

        public enum Values
        {
            Lands,
            Hunt,
            Fish
        }
    }

    public class RefAgreementObjectTypes : Reference
    {
        public const string RefName = nameof(RefAgreementObjectTypes);

        public RefAgreementObjectTypes() : base(RefName, "Типы объектов")
        {
            Items = new ReferenceItemCollection {
                {Values.Lands.ToString(), "Земельные ресурсы", new ReferenceItemCollection
                    {
                        {"Land", "Земельные участки коммерческого назначения в населённых пунктах"},
                        {"LandAgricultural", "Земельные участки сель хоз назначения"},
                    }
                },
                {Values.Hunt.ToString(), "Охотничьи угодья", new ReferenceItemCollection
                    {
                        {"HuntingLand", "Охотничьи угодья"},
                    }
                },
                {Values.Fish.ToString(), "Рыбохозяйственные водоёмы", new ReferenceItemCollection
                    {
                        {"FishingReservoir", "Рыбохозяйственные водоёмы"},
                    }
                }
            };
        }

        public enum Values
        {
            Lands,
            Hunt,
            Fish
        }
    }

    public class AgreementsModel
    {
        public int flAgreementId { get; set; }
        public string flAgreementNumber { get; set; }
        public int flAgreementRevisionId { get; set; }
        public string flAgreementType { get; set; }
        public AgreementStatuses flAgreementStatus { get; set; }
        public DateTime flAgreementCreateDate { get; set; }
        public DateTime? flAgreementSignDate { get; set; }
        public int flObjectId { get; set; }
        public string flObjectType { get; set; }
        public int? flTradeId { get; set; }
        public string flTradeType { get; set; }
        public int? flAuctionId { get; set; }
        public string flAgreementCreatorBin { get; set; }
        public string flSellerBin { get; set; }
        public string flWinnerXin { get; set; }
    }
}
