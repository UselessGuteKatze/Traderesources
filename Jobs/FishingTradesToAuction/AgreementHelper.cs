using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Yoda.YodaReferences;
using YodaQuery;

namespace FishingTradesToAuction
{
    public class TbAgreements : QueryTable
    {
        public TbAgreements() : base(nameof(TbAgreements), "Договоры")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull().Sequence(),
                new TextField(nameof(flAgreementNumber), "Номер договора", 20).NotNull(),
                new IntField(nameof(flAgreementRevisionId), "Id ревизии договора").NotNull(),
                new TextField(nameof(flAgreementType), "Тип договора").NotNull(),
                new RefField<RefAgreementStatuses, AgreementStatuses>(nameof(flAgreementStatus), "Статус договора").NotNull(),
                new DateTimeField(nameof(flAgreementCreateDate), "Дата создания договора").NotNull(),
                new DateTimeField(nameof(flAgreementSignDate), "Дата подписания договора"),
                new IntField(nameof(flObjectId), "Id объекта").NotNull(),
                new TextField(nameof(flObjectType), "Тип объекта").NotNull(),
                new IntField(nameof(flTradeId), "Id торга"),
                new TextField(nameof(flTradeType), "Тип торга"),
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

        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public TextField flAgreementNumber => (TextField)this[nameof(flAgreementNumber)];
        public IntField flAgreementRevisionId => (IntField)this[nameof(flAgreementRevisionId)];
        public TextField flAgreementType => (TextField)this[nameof(flAgreementType)];
        public RefField<RefAgreementStatuses, AgreementStatuses> flAgreementStatus => (RefField<RefAgreementStatuses, AgreementStatuses>)this[nameof(flAgreementStatus)];
        public DateTimeField flAgreementCreateDate => (DateTimeField)this[nameof(flAgreementCreateDate)];
        public DateTimeField flAgreementSignDate => (DateTimeField)this[nameof(flAgreementSignDate)];
        public IntField flObjectId => (IntField)this[nameof(flObjectId)];
        public TextField flObjectType => (TextField)this[nameof(flObjectType)];
        public IntField flTradeId => (IntField)this[nameof(flTradeId)];
        public TextField flTradeType => (TextField)this[nameof(flTradeType)];
        public TextField flAgreementCreatorBin => (TextField)this[nameof(flAgreementCreatorBin)];
        public TextField flSellerBin => (TextField)this[nameof(flSellerBin)];
        public TextField flWinnerXin => (TextField)this[nameof(flWinnerXin)];

    }
    public class TbAgreementModels : QueryTable
    {
        public TbAgreementModels() : base(nameof(TbAgreementModels), "Модели договоров")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull(),
                new IntField(nameof(flAgreementRevisionId), "Id ревизии договора").NotNull(),
                new RefField<RefAgreementStatuses, AgreementStatuses>(nameof(flAgreementStatus), "Статус договора").NotNull(),
                new DateTimeField(nameof(flDateTime), "Дата").NotNull(),
                new TextField(nameof(flComment), "Примечание", true),
                new DateTimeField(nameof(flCommentDateTime), "Дата комментария"),
                new TextField(nameof(flModels), "Модели договора").NotNull(),
                new TextField(nameof(flContent), "Контент договора").NotNull(),

            };
            DbKey = "dbAgreements";
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flAgreementId)}_{nameof(flAgreementRevisionId)}", new[] { new IndexCol(nameof(flAgreementId)), new IndexCol(nameof(flAgreementRevisionId)) }, true)
            };
        }

        public DateTimeField flRequestDate => (DateTimeField)this[nameof(flRequestDate)];
        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public IntField flAgreementRevisionId => (IntField)this[nameof(flAgreementRevisionId)];
        public RefField<RefAgreementStatuses, AgreementStatuses> flAgreementStatus => (RefField<RefAgreementStatuses, AgreementStatuses>)this[nameof(flAgreementStatus)];
        public DateTimeField flDateTime => (DateTimeField)this[nameof(flDateTime)];
        public TextField flComment => (TextField)this[nameof(flComment)];
        public DateTimeField flCommentDateTime => (DateTimeField)this[nameof(flCommentDateTime)];
        public TextField flModels => (TextField)this[nameof(flModels)];
        public TextField flContent => (TextField)this[nameof(flContent)];
    }

    public class RefAgreementStatuses : ReferenceEnum<AgreementStatuses> {
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
        }) {
        }
    }

    public enum AgreementStatuses {
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
        public string flAgreementCreatorBin { get; set; }
        public string flSellerBin { get; set; }
        public string flWinnerXin { get; set; }
    }

    public class TbConfiscateAgreements : QueryTable
    {
        public TbConfiscateAgreements(): base(nameof(TbConfiscateAgreements), "Договоры ЭТП")
        {
            var fields = new List<Field> {
                new IntField(nameof(flAuctionId), "Номер аукциона"),
                new IntField(nameof(flAgreementId), "Id договора"),
                new RefField<RefAgreementType, AgreementTypes>(nameof(flAgreementType), "Тип договора"),
                new RefField<RefConfiscateAgreementStatus, ConfiscateAgreementStatuses>(nameof(flAgreementStatus), "Статус"),
                new DateTimeField(nameof(flCreateDate), "Дата создания"),
                new DateTimeField(nameof(flActivateDate), "Дата исполнения"),
            };
            Fields = fields.ToArray();
            DbKey = "dbAuction";
        }
        public IntField flAuctionId { get { return (IntField)this[nameof(flAuctionId)]; } }
        public IntField flAgreementId { get { return (IntField)this[nameof(flAgreementId)]; } }
        public RefField<RefAgreementType, AgreementTypes> flAgreementType { get { return (RefField<RefAgreementType, AgreementTypes>)this[nameof(flAgreementType)]; } }
        public RefField<RefConfiscateAgreementStatus, ConfiscateAgreementStatuses> flAgreementStatus { get { return (RefField<RefConfiscateAgreementStatus, ConfiscateAgreementStatuses>)this[nameof(flAgreementStatus)]; } }
        public DateTimeField flCreateDate { get { return (DateTimeField)this[nameof(flCreateDate)]; } }
        public DateTimeField flActivateDate { get { return (DateTimeField)this[nameof(flActivateDate)]; } }
    }

    public class RefAgreementType : ReferenceEnum<AgreementTypes>
    {
        public const string RefName = nameof(RefAgreementType);
        public RefAgreementType() : base(RefName, "Статусы договора", new Dictionary<AgreementTypes, string> {
            {AgreementTypes.Fish, "Договор на ведение рыбного хозяйства"}
        })
        {
        }
    }
    public enum AgreementTypes
    {
        Fish
    }
    public class RefConfiscateAgreementStatus : ReferenceEnum<ConfiscateAgreementStatuses>
    {
        public const string RefName = nameof(RefConfiscateAgreementStatus);
        public RefConfiscateAgreementStatus() : base(RefName, "Статусы договора", new Dictionary<ConfiscateAgreementStatuses, string> {
            {ConfiscateAgreementStatuses.Saved, "Сохранен"},
            {ConfiscateAgreementStatuses.Signed, "Подписан"},
            {ConfiscateAgreementStatuses.Deleted, "Удален"},
            {ConfiscateAgreementStatuses.Canceled, "Расторгнут"}
        })
        {
        }
    }
    public enum ConfiscateAgreementStatuses
    {
        Saved,
        Signed,
        Deleted,
        Canceled
    }
    public class ConfiscateAgreementsModel
    {
        public ConfiscateAgreementsModel(int flAuctionId, int flAgreementId, ConfiscateAgreementStatuses flAgreementStatus, DateTime flCreateDate, DateTime? flActivateDate)
        {
            this.flAuctionId = flAuctionId;
            this.flAgreementId = flAgreementId;
            this.flAgreementType = AgreementTypes.Fish;
            this.flAgreementStatus = flAgreementStatus;
            this.flCreateDate = flCreateDate;
            this.flActivateDate = flActivateDate;
        }
        public int flAuctionId { get; set; }
        public int flAgreementId { get; set; }

        public AgreementTypes flAgreementType = AgreementTypes.Fish;
        public ConfiscateAgreementStatuses flAgreementStatus { get; set; }
        public DateTime flCreateDate { get; set; }
        public DateTime? flActivateDate { get; set; }
    }

    public static class AgreementHelper
    {
        public static AgreementsModel GetAgreementsModelFirstOrDefault(this TbAgreements tbAgreements, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            var r = tbAgreements.SelectFirstOrDefault(t => t.Fields.ToFieldsAliases(), queryExecuter, transaction);

            if (!r.IsFirstRowExists)
            {
                return null;
            }

            return new AgreementsModel
            {
                flAgreementId = r.GetVal(t => t.flAgreementId),
                flAgreementNumber = r.GetVal(t => t.flAgreementNumber),
                flAgreementRevisionId = r.GetVal(t => t.flAgreementRevisionId),
                flAgreementType = r.GetVal(t => t.flAgreementType),
                flAgreementStatus = r.GetVal(t => t.flAgreementStatus),
                flAgreementCreateDate = r.GetVal(t => t.flAgreementCreateDate),
                flAgreementSignDate = r.GetValOrNull(t => t.flAgreementSignDate),
                flObjectId = r.GetVal(t => t.flObjectId),
                flObjectType = r.GetVal(t => t.flObjectType),
                flTradeId = r.GetValOrNull(t => t.flTradeId),
                flTradeType = r.GetVal(t => t.flTradeType),
                flAgreementCreatorBin = r.GetVal(t => t.flAgreementCreatorBin),
                flSellerBin = r.GetVal(t => t.flSellerBin),
                flWinnerXin = r.GetVal(t => t.flWinnerXin)
            };
        }
        public static AgreementsModel GetAgreementModel(int objectId, string objectType, int tradeId, string tradeType, IQueryExecuter queryExecuter)
        {
            return new TbAgreements()
                    .AddFilter(t => t.flObjectId, objectId)
                    .AddFilter(t => t.flTradeId, tradeId)
                    .AddFilter(t => t.flObjectType, objectType)
                    .AddFilter(t => t.flTradeType, tradeType)
                    .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Deleted)
                    .AddFilterNot(t => t.flAgreementStatus, AgreementStatuses.Canceled)
                    .GetAgreementsModelFirstOrDefault(queryExecuter);
        }
        public static void SetAgreementStatus(int agreementId, AgreementStatuses status, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            new TbAgreements().AddFilter(t => t.flAgreementId, agreementId).Update().SetT(t => t.flAgreementStatus, status).Execute(queryExecuter, transaction);
        }
        public static void SetAgreementModelStatus(int agreementRevisionId, AgreementStatuses status, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            new TbAgreementModels().AddFilter(t => t.flAgreementRevisionId, agreementRevisionId).Update().SetT(t => t.flAgreementStatus, status).Execute(queryExecuter, transaction);
        }
        public static bool ExistsTbConfiscateAgreements(int auctionId, int agreementId, IQueryExecuter queryExecuter)
        {
            return new TbConfiscateAgreements()
                .AddFilter(t => t.flAuctionId, auctionId)
                .AddFilter(t => t.flAgreementId, agreementId)
                .Count(queryExecuter) > 0;
        }
        public static void InsertTbConfiscateAgreements(ConfiscateAgreementsModel agreement, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            new TbConfiscateAgreements()
                .Insert()
                .SetT(t => t.flAuctionId, agreement.flAuctionId)
                .SetT(t => t.flAgreementId, agreement.flAgreementId)
                .SetT(t => t.flAgreementType, AgreementTypes.Fish)
                .SetT(t => t.flAgreementStatus, agreement.flAgreementStatus)
                .SetT(t => t.flCreateDate, agreement.flCreateDate)
                .SetT(t => t.flActivateDate, agreement.flActivateDate)
                .Execute(queryExecuter, transaction);
        }
        public static void UpdateTbConfiscateAgreementsStatus(int auctionId, int agreementId, ConfiscateAgreementStatuses status, DateTime? activateDate, IQueryExecuter queryExecuter, ITransaction transaction = null)
        {
            new TbConfiscateAgreements()
                .AddFilter(t => t.flAuctionId, auctionId)
                .AddFilter(t => t.flAgreementId, agreementId)
                .AddFilter(t => t.flAgreementType, AgreementTypes.Fish)
                .Update()
                .SetT(t => t.flAgreementStatus, status)
                .SetT(t => t.flActivateDate, activateDate)
                .Execute(queryExecuter, transaction);
        }
    } 
}