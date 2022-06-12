using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class TbAgreementModels : QueryTable {
        public TbAgreementModels() : base(nameof(TbAgreementModels), "Модели договоров")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull(),
                new IntField(nameof(flAgreementRevisionId), "Id ревизии договора").NotNull(),
                new RefField<RefAgreementStatuses, AgreementStatuses>(nameof(flAgreementStatus), "Статус договора").NotNull(),
                new DateTimeField(nameof(flDateTime), "Дата").NotNull(),
                new TextField(nameof(flComment), "Примечание", true),
                new DateTimeField(nameof(flCommentDateTime), "Дата комментария"),
                new JsonField<DefaultAgrTemplate[]>(nameof(flModels), "Модели договора").NotNull(),
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
        public JsonField<DefaultAgrTemplate[]> flModels => (JsonField<DefaultAgrTemplate[]>)this[nameof(flModels)];
        public TextField flContent => (TextField)this[nameof(flContent)];
    }
}
