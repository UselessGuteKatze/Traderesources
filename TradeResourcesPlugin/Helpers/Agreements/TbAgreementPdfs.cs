using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class TbAgreementPdfs : QueryTable {
        public TbAgreementPdfs() : base(nameof(TbAgreementPdfs), "Модели договоров")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull(),
                new BinaryField(nameof(flPdf), "Pdf договора").NotNull(),
                new BinaryField(nameof(flPdfWithSigns), "Pdf договора с подписями"),

            };
            DbKey = "dbAgreements";
            PrimaryKey = new TablePrimaryKey($"PK_{Name}", new[] { new IndexCol(nameof(flAgreementId)) });
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flAgreementId)}", new[] { new IndexCol(nameof(flAgreementId)) }, true)
            };
        }

        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public BinaryField flPdf => (BinaryField)this[nameof(flPdf)];
        public BinaryField flPdfWithSigns => (BinaryField)this[nameof(flPdfWithSigns)];
    }
}
