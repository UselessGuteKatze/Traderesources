using YodaQuery;

namespace TradeResourcesPlugin.Helpers.Agreements
{
    public class TbRenderPaymentItems : QueryTable
    {
        public TbRenderPaymentItems() : base(nameof(TbRenderPaymentItems), "Платежи")
        {
            Fields = new Field[] {
                new IntField(nameof(flPaymentItemId), "Id оплаты"),
                new RefField<RefPaymentMatchStatuses, PaymentMatchStatus>(nameof(flPaymentItemStatus), "Статус платежа"),
                new MoneyField(nameof(flAmount), "Сумма платежа"),
                new DateTimeField(nameof(flDateTime), "Дата платежа"),
                new TextField(nameof(flPurpose), "Назначение платежа")
            };
            DbKey = null;
        }

        public IntField flPaymentItemId => (IntField)this[nameof(flPaymentItemId)];
        public RefField<RefPaymentMatchStatuses, PaymentMatchStatus> flPaymentItemStatus => (RefField<RefPaymentMatchStatuses, PaymentMatchStatus>)this[nameof(flPaymentItemStatus)];
        public MoneyField flAmount => (MoneyField)this[nameof(flAmount)];
        public DateTimeField flDateTime => (DateTimeField)this[nameof(flDateTime)];
        public TextField flPurpose => (TextField)this[nameof(flPurpose)];

    }
}
