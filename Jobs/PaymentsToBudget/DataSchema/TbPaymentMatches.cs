using System;
using System.Collections.Generic;
using Yoda.YodaReferences;
using YodaHelpers.Payments;
using YodaQuery;

namespace PaymentsToBudget.DataSchema
{
    public class TbPaymentMatches : QueryTable
    {
        public TbPaymentMatches() : base(nameof(TbPaymentMatches), "Привязки платежей")
        {
            Fields = new Field[] {

                new IntField(nameof(flId), "Id записи").NotNull().Sequence(),
                new IntField(nameof(flPaymentId), "Id оплаты").NotNull(),
                new DateTimeField(nameof(flDateTime), "Дата привязки").NotNull(),
                new JsonField<PaymentItemModel[]>(nameof(flPaymentItems), "Платежи привязки").NotNull(),
                new RefField<RefPaymentMatchStatuses, PaymentMatchStatus>(nameof(flStatus), "Статус привязки").NotNull(),
                new JsonField<MatchResult>(nameof(flMatchResult), "MatchResult").NotNull(),
                new MoneyField(nameof(flAmount), "Сумма привязки").NotNull(),
                new MoneyField(nameof(flGuaranteeAmount), "Сумма привязки гар. взноса").NotNull(),
                new MoneyField(nameof(flRealAmount), "Сумма привязки без гар. взноса (т.к. он отправляется самой этп)").NotNull(),
                new BooleanField(nameof(flHasSendAmount), "Содержит сумму отправки в бюджет").NotNull(),
                new MoneyField(nameof(flSendAmount), "Сумма отправки в бюджет (без гар. взноса, т.к. он отправляется самой этп, и без переплаты, т.к. она отправляется отдельно)").NotNull(),
                new JsonField<MatchBlockResult>(nameof(flMatchBlockResult), "MatchBlockResult"),
                new JsonField<RequisitesModel>(nameof(flRequisites), "Requisites"),

                new BooleanField(nameof(flOverpayment), "Содержит переплату").NotNull(),
                new MoneyField(nameof(flOverpaymentAmount), "Сумма переплаты"),
                new BooleanField(nameof(flSendOverpayment), "Содержит переплату (возвращаемую победителю)").NotNull(),
                new MoneyField(nameof(flOverpaymentSendAmount), "Сумма отправки переплаты (не содержит переплату из гарантийного взноса, если такая есть, т.к. она отправляется самой этп)"),
                new JsonField<MatchBlockResult>(nameof(flOverpaymentMatchBlockResult), "OverpaymentMatchBlockResult"),
                new JsonField<RequisitesModel>(nameof(flOverpaymentRequisites), "OverpaymentRequisites"),
            };
            DbKey = "dbAgreements";
            PrimaryKey = new TablePrimaryKey($"PK_{Name}", new[] { new IndexCol(nameof(flId)), new IndexCol(nameof(flPaymentId)) });
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flPaymentId)}", new[] { new IndexCol(nameof(flPaymentId)) }, false),
                new TableIndex($"IDX_{Name}_{nameof(flStatus)}", new[] { new IndexCol(nameof(flStatus)) }, false)
            };
        }

        public IntField flId => (IntField)this[nameof(flId)];
        public IntField flPaymentId => (IntField)this[nameof(flPaymentId)];
        public DateTimeField flDateTime => (DateTimeField)this[nameof(flDateTime)];
        public JsonField<PaymentItemModel[]> flPaymentItems => (JsonField<PaymentItemModel[]>)this[nameof(flPaymentItems)];
        public RefField<RefPaymentMatchStatuses, PaymentMatchStatus> flStatus => (RefField<RefPaymentMatchStatuses, PaymentMatchStatus>)this[nameof(flStatus)];
        public MoneyField flAmount => (MoneyField)this[nameof(flAmount)];
        public MoneyField flGuaranteeAmount => (MoneyField)this[nameof(flGuaranteeAmount)];
        public MoneyField flRealAmount => (MoneyField)this[nameof(flRealAmount)];
        public BooleanField flHasSendAmount => (BooleanField)this[nameof(flHasSendAmount)];
        public MoneyField flSendAmount => (MoneyField)this[nameof(flSendAmount)];
        public JsonField<MatchResult> flMatchResult => (JsonField<MatchResult>)this[nameof(flMatchResult)];
        public JsonField<MatchBlockResult> flMatchBlockResult => (JsonField<MatchBlockResult>)this[nameof(flMatchBlockResult)];
        public JsonField<RequisitesModel> flRequisites => (JsonField<RequisitesModel>)this[nameof(flRequisites)];

        public BooleanField flOverpayment => (BooleanField)this[nameof(flOverpayment)];
        public BooleanField flSendOverpayment => (BooleanField)this[nameof(flSendOverpayment)];
        public MoneyField flOverpaymentAmount => (MoneyField)this[nameof(flOverpaymentAmount)];
        public MoneyField flOverpaymentSendAmount => (MoneyField)this[nameof(flOverpaymentSendAmount)];
        public JsonField<MatchBlockResult> flOverpaymentMatchBlockResult => (JsonField<MatchBlockResult>)this[nameof(flOverpaymentMatchBlockResult)];
        public JsonField<RequisitesModel> flOverpaymentRequisites => (JsonField<RequisitesModel>)this[nameof(flOverpaymentRequisites)];
    }

    public class RefPaymentMatchStatuses : ReferenceEnum<PaymentMatchStatus>
    {
        public const string RefName = nameof(RefPaymentMatchStatuses);
        public RefPaymentMatchStatuses() : base(RefName, "Статусы платежей", new Dictionary<PaymentMatchStatus, string> {
            {PaymentMatchStatus.Linked, "Привязан"},
            {PaymentMatchStatus.LinkedAndSent, "Привязан и отправлен в бюджет"}
        })
        {
        }
    }

    public enum PaymentMatchStatus
    {
        Linked,
        LinkedAndSent
    }

    public class PaymentItemModel
    {
        public PaymentId flId { get; set; }
        public decimal flAmount { get; set; }
        public DateTime flDateTime { get; set; }
        public string flPurpose { get; set; }
        public bool flIsGuarantee { get; set; }
    }

    public class PaymentMatchModel
    {
        public int flId { get; set; }
        public int flPaymentId { get; set; }
        public DateTime flDateTime { get; set; }
        public PaymentItemModel[] flPaymentItems { get; set; }
        public PaymentMatchStatus flStatus { get; set; }
        public MatchResult flMatchResult { get; set; }
        public decimal flAmount { get; set; }
        public decimal flGuaranteeAmount { get; set; }
        public decimal flRealAmount { get; set; }
        public bool flHasSendAmount { get; set; }
        public decimal flSendAmount { get; set; }
        public MatchBlockResult flMatchBlockResult { get; set; }
        public RequisitesModel flRequisites { get; set; }

        public bool flOverpayment { get; set; }
        public bool flSendOverpayment { get; set; }
        public decimal? flOverpaymentAmount { get; set; }
        public decimal? flOverpaymentSendAmount { get; set; }
        public MatchBlockResult flOverpaymentMatchBlockResult { get; set; }
        public RequisitesModel flOverpaymentRequisites { get; set; }
    }

    public class RequisitesModel
    {
        public string flName { get; set; }
        public string flXin { get; set; }
        public string flBik { get; set; }
        public string flIban { get; set; }
        public int? flKbe { get; set; }
        public int? flKnp { get; set; }
        public string flKbk { get; set; }
        public string flContacts { get; set; }
    }
}
