using HuntingSource.References.Trade;
using Yoda.Interfaces.Helpers;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers.Agreements
{
    public class TbRenderPaymentMatches : QueryTable
    {
        public TbRenderPaymentMatches() : base(nameof(TbRenderPaymentMatches), "Платежи")
        {
            Fields = new Field[] {
                new TextField(nameof(flName), "Наименование", false),
                new TextField(nameof(flXin), "БИН", false, 12),
                new TextField(nameof(flBik), "БИК", false, 8),
                new TextField(nameof(flIban), "IBAN", false, 32){MetaData = new FieldMetaData {{ "validation-regex", "^[A-Z0-9]{9,32}$" } }},
                new IntField(nameof(flKbe), "КБЕ"),
                new IntField(nameof(flKnp), "КНП"),
                new ReferenceTextField(nameof(flKbk), "КБК", RefKbk.RefName, 0, 6),

                new TextField(nameof(flOverpaymentName), "Наименование/ФИО", false).Required(),
                new TextField(nameof(flOverpaymentXin), "ИИН/БИН", false, 12).Required(),
                new TextField(nameof(flOverpaymentBik), "БИК", false, 8).Required(),
                new TextField(nameof(flOverpaymentIban), "IBAN", false, 32){MetaData = new FieldMetaData {{ "validation-regex", "^[A-Z0-9]{9,32}$" } }}.Required(),
                new IntField(nameof(flOverpaymentKbe), "КБЕ").Required(),
                new IntField(nameof(flOverpaymentKnp), "КНП").Required(),
                new ReferenceTextField(nameof(flOverpaymentKbk), "КБК", RefKbk.RefName, 0, 6).Required(),
            };
            DbKey = null;
        }

        public TextField flName => (TextField)this[nameof(flName)];
        public TextField flXin => (TextField)this[nameof(flXin)];
        public TextField flBik => (TextField)this[nameof(flBik)];
        public TextField flIban => (TextField)this[nameof(flIban)];
        public IntField flKbe => (IntField)this[nameof(flKbe)];
        public IntField flKnp => (IntField)this[nameof(flKnp)];
        public ReferenceTextField flKbk => (ReferenceTextField)this[nameof(flKbk)];

        public TextField flOverpaymentName => (TextField)this[nameof(flOverpaymentName)];
        public TextField flOverpaymentXin => (TextField)this[nameof(flOverpaymentXin)];
        public TextField flOverpaymentBik => (TextField)this[nameof(flOverpaymentBik)];
        public TextField flOverpaymentIban => (TextField)this[nameof(flOverpaymentIban)];
        public IntField flOverpaymentKbe => (IntField)this[nameof(flOverpaymentKbe)];
        public IntField flOverpaymentKnp => (IntField)this[nameof(flOverpaymentKnp)];
        public ReferenceTextField flOverpaymentKbk => (ReferenceTextField)this[nameof(flOverpaymentKbk)];

    }
}
