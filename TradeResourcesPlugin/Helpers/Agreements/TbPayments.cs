using System;
using System.Collections.Generic;
using System.Text;
using Yoda.YodaReferences;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers.Agreements {
    public class TbPayments : QueryTable {
        public TbPayments() : base(nameof(TbPayments), "Оплата")
        {
            Fields = new Field[] {

                new IntField(nameof(flPaymentId), "Id оплаты").NotNull().Sequence(),
                new IntField(nameof(flAgreementId), "Id договора").NotNull(),
                new RefField<RefPaymentStatuses, PaymentStatus>(nameof(flPaymentStatus), "Статус оплаты").NotNull(),
                new MoneyField(nameof(flPayAmount), "Оплачиваемая сумма").NotNull(),
                new MoneyField(nameof(flPaidAmount), "Оплаченная сумма").NotNull(),
            };
            DbKey = "dbAgreements";
            PrimaryKey = new TablePrimaryKey($"PK_{Name}", new[] { new IndexCol(nameof(flPaymentId)), new IndexCol(nameof(flAgreementId)) });
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flPaymentId)}", new[] { new IndexCol(nameof(flPaymentId)) }, true),
                new TableIndex($"IDX_{Name}_{nameof(flAgreementId)}", new[] { new IndexCol(nameof(flAgreementId)) }, true)
            };
        }

        public IntField flPaymentId => (IntField)this[nameof(flPaymentId)];
        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public RefField<RefPaymentStatuses, PaymentStatus> flPaymentStatus => (RefField<RefPaymentStatuses, PaymentStatus>)this[nameof(flPaymentStatus)];
        public MoneyField flPayAmount => (MoneyField)this[nameof(flPayAmount)];
        public MoneyField flPaidAmount => (MoneyField)this[nameof(flPaidAmount)];

    }

    public class RefPaymentStatuses : ReferenceEnum<PaymentStatus> {
        public const string RefName = nameof(RefPaymentStatuses);
        public RefPaymentStatuses() : base(RefName, "Статусы оплаты", new Dictionary<PaymentStatus, string> {
            {PaymentStatus.Paying, "Оплачивается"},
            {PaymentStatus.Paid, "Оплачен"}
        })
        {
        }
    }

    public enum PaymentStatus {
        Paying,
        Paid
    }

    public class PaymentModel {
        public int flPaymentId { get; set; }
        public int flAgreementId { get; set; }
        public PaymentStatus flPaymentStatus { get; set; }
        public decimal flPayAmount { get; set; }
        public decimal flPaidAmount { get; set; }
        public PaymentMatchModel[] flPaymentMatches { get; set; }
    }

}
