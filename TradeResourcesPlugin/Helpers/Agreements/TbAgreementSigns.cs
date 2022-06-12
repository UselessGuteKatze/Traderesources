using System.Collections.Generic;
using Yoda.YodaReferences;
using YodaHelpers.ActionMenus;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class TbAgreementSigns : TbSignStoreBase {
        public TbAgreementSigns() : base(nameof(TbAgreementSigns), "Подписи договоров")
        {
            Fields = new Field[] {

                new IntField(nameof(flAgreementId), "Id договора").NotNull(),
                new TextField(nameof(flDataToSign), "Данные для подписи").NotNull(),
                new TextField(nameof(flCertInfo), "Информация о подписи").NotNull(),
                new TextField(nameof(flSignedData), "Подписанные данные").NotNull(),
                new DateTimeField(nameof(flSignDate), "Дата подписи").NotNull(),
                new IntField(nameof(flUserId), "Id пользователя").NotNull(),
                new RefField<RefAgreementSignerRoles, AgreementSignerRoles>(nameof(flSignerRole), "Роль подписанта").NotNull()

            };
            PrimaryKey = new TablePrimaryKey($"PK_{Name}", new[] { new IndexCol(nameof(flAgreementId)), new IndexCol(nameof(flSignerRole)) });
            DbKey = "dbAgreements";
            Indexes = new[] {
                new TableIndex($"IDX_{Name}_{nameof(flAgreementId)}", new[] { new IndexCol(nameof(flAgreementId)), new IndexCol(nameof(flSignerRole)) }, true)
            };
        }

        public IntField flAgreementId => (IntField)this[nameof(flAgreementId)];
        public TextField flDataToSign => (TextField)this[nameof(flDataToSign)];
        public TextField flCertInfo => (TextField)this[nameof(flCertInfo)];
        public TextField flSignedData => (TextField)this[nameof(flSignedData)];
        public DateTimeField flSignDate => (DateTimeField)this[nameof(flSignDate)];
        public IntField flUserId => (IntField)this[nameof(flUserId)];
        public RefField<RefAgreementSignerRoles, AgreementSignerRoles> flSignerRole => (RefField<RefAgreementSignerRoles, AgreementSignerRoles>)this[nameof(flSignerRole)];

        public override IntField GetObjIdField()
        {
            return flAgreementId;
        }

        public override TextField GetDataToSignField()
        {
            return flDataToSign;
        }

        public override TextField GetCertInfoField()
        {
            return flCertInfo;
        }

        public override TextField GetSignedDataField()
        {
            return flSignedData;
        }

        public override DateTimeField GetSignDateField()
        {
            return flSignDate;
        }

        public override IntField GetUserIdField()
        {
            return flUserId;
        }
    }

    public class RefAgreementSignerRoles : ReferenceEnum<AgreementSignerRoles> {
        public const string RefName = nameof(RefAgreementSignerRoles);
        public RefAgreementSignerRoles() : base(RefName, "Подписи договора", new Dictionary<AgreementSignerRoles, string> {
            {AgreementSignerRoles.Seller, "Продавец"},
            {AgreementSignerRoles.Winner, "Победитель"},
        })
        {
        }
    }

    public enum AgreementSignerRoles {
        Seller,
        Winner
    }
}

