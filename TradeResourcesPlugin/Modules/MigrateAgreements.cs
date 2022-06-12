using LandSource.Helpers;
using LandSource.QueryTables.Trades;
using LandSource.References.LandObject;
using LandSource.References.Trades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TradeResourcesPlugin.Helpers;
using TradeResourcesPlugin.Modules.LandObjectsMenus.Agreements;
using Yoda.Interfaces;
using Yoda.Interfaces.Helpers;
using Yoda.Interfaces.Menu;
using YodaQuery;

namespace TradeResourcesPlugin.Modules
{
    public class MigrateAgreements : FrmMenu
    {

        public MigrateAgreements(string moduleName) : base(nameof(MigrateAgreements), "MigrateAgreements")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var TbConfiscateAgreements = new TbConfiscateAgreements()
                    .AddFilter(t => t.flCreateDate, ConditionOperator.GreateOrEqual, new DateTime(2021, 10, 25))
                    .AddFilter(t => t.flAgreementStatus, "Signed")
                    .AddFilter(t => t.flAgreementType, "Upgs")
                    ;
                var TbConfiscateAgreementTemplate = new TbConfiscateAgreementTemplate();
                var TbAuctions = new TbAuctions()
                    .AddFilter(t => t.flSource, "LandResources")
                    ;
                var TbAuctionObjects = new TbAuctionObjects()
                    .AddFilter(t => t.flObjectType, ConditionOperator.In, new[] { "LandAgricultural", "Land" })
                    ;
                var TbConfiscatedAgreementsContent = new TbConfiscatedAgreementsContent();
                var TbUpgsAgreementsContent = new TbUpgsAgreementsContent();
                var TbConfiscateAgreementTemplateModelHistory = new TbConfiscateAgreementTemplateModelHistory()
                    .AddFilter(t => t.flIsActive, true)
                    ;

                var join = TbConfiscateAgreements
                    .JoinT(nameof(TbConfiscateAgreements), TbConfiscateAgreementTemplate, nameof(TbConfiscateAgreementTemplate))
                    .On(new Join(TbConfiscateAgreementTemplate.flAgreementId, TbConfiscateAgreements.flAgreementId))
                    .JoinT(nameof(TbConfiscateAgreements), TbAuctions, nameof(TbAuctions))
                    .On(new Join(TbAuctions.flAuctionId, TbConfiscateAgreements.flAuctionId))
                    .JoinT(nameof(TbAuctions), TbAuctionObjects, nameof(TbAuctionObjects))
                    .On(new Join(TbAuctionObjects.flObjectId, TbAuctions.flObjectId))
                    .JoinT(nameof(TbConfiscateAgreements), TbConfiscatedAgreementsContent, nameof(TbConfiscatedAgreementsContent), JoinType.Left)
                    .On(new Join(TbConfiscatedAgreementsContent.flAgreementId, TbConfiscateAgreements.flAgreementId))
                    .JoinT(nameof(TbConfiscateAgreements), TbUpgsAgreementsContent, nameof(TbUpgsAgreementsContent), JoinType.Left)
                    .On(new Join(TbUpgsAgreementsContent.flAgreementId, TbConfiscateAgreements.flAgreementId))
                    .JoinT(nameof(TbConfiscateAgreements), TbConfiscateAgreementTemplateModelHistory, nameof(TbConfiscateAgreementTemplateModelHistory))
                    .On(new Join(TbConfiscateAgreementTemplateModelHistory.flAgreementId, TbConfiscateAgreements.flAgreementId));

                var select = join.Select(new FieldAlias[] {
                    TbConfiscateAgreementTemplate.flAgreementNumber,
                    TbAuctions.flLandOwnershipType,
                    TbConfiscateAgreements.flAgreementStatus,
                    TbConfiscateAgreements.flCreateDate,
                    TbConfiscateAgreements.flActivateDate,
                    new FieldAlias(TbAuctionObjects.flSourceObjectId, "flObjectId"),
                    TbAuctionObjects.flObjectType,
                    new FieldAlias(TbAuctions.flSourceObjectId, "flTradeId"),
                    TbAuctions.flAuctionType,
                    TbAuctionObjects.flSellerBin,
                    new FieldAlias(TbConfiscatedAgreementsContent.flAgreementContent, "flAgreementContentConf"),
                    new FieldAlias(TbConfiscatedAgreementsContent.flAgreementContentWithSigns, "flAgreementContentWithSignsConf"),
                    new FieldAlias(TbUpgsAgreementsContent.flAgreementContent, "flAgreementContentUpgs"),
                    new FieldAlias(TbUpgsAgreementsContent.flAgreementContentWithSigns, "flAgreementContentWithSignsUpgs"),
                    TbConfiscateAgreementTemplate.flAgreementTemplates,
                    TbConfiscateAgreementTemplateModelHistory.flAgreementModel,
                }, re.QueryExecuter);

                Console.WriteLine(select.Rows.Count);

                select.AsEnumerable().Each(r =>
                {

                    var flAgreementNumber = TbConfiscateAgreementTemplate.flAgreementNumber.GetRowVal(r);
                    var flAgreementType = TbAuctions.flLandOwnershipType.GetRowVal(r) == "LANDPRIV" ? nameof(ДоговорКпЗемельногоУчастка) : nameof(ДоговорКпПраваАрендыЗемельногоУчастка);
                    var flAgreementStatus = TbConfiscateAgreements.flAgreementStatus.GetRowVal(r);
                    var flCreateDate = TbConfiscateAgreements.flCreateDate.GetRowVal(r);
                    var flSignDate = TbConfiscateAgreements.flActivateDate.GetRowVal(r);
                    var flObjectId = TbAuctionObjects.flSourceObjectId.GetRowVal(r, "flObjectId");
                    var flObjectType = TbAuctionObjects.flObjectType.GetRowVal(r);
                    var flTradeId = TbAuctions.flSourceObjectId.GetRowVal(r, "flTradeId");
                    var flTradeType = TbAuctions.flAuctionType.GetRowVal(r);
                    if (flTradeType.Contains("Up"))
                    {
                        flTradeType = "PriceUpLand";
                    }
                    if (flTradeType.Contains("Down"))
                    {
                        flTradeType = "PriceDownLand";
                    }
                    if (flTradeType.Contains("Competition"))
                    {
                        flTradeType = "CompetitionAgricultureLand";
                    }
                    var flSellerBin = TbAuctionObjects.flSellerBin.GetRowVal(r);
                    var flAgreementCreatorBin = flSellerBin;
                    var flWinnerXin = new TbLandObjectsTrades()
                        .AddFilter(t => t.flId, flTradeId)
                        .SelectScalar(t => t.flWinnerBin, re.QueryExecuter)
                        ;

                    var flPdf = TbConfiscatedAgreementsContent.flAgreementContent.GetRowVal<byte[]>(r, "flAgreementContentConf");
                    var flPdfWithSigns = TbConfiscatedAgreementsContent.flAgreementContentWithSigns.GetRowVal<byte[]>(r, "flAgreementContentWithSignsConf");
                    if (flPdf == null)
                    {
                        flPdf = TbUpgsAgreementsContent.flAgreementContent.GetRowVal<byte[]>(r, "flAgreementContentUpgs");
                        flPdfWithSigns = TbUpgsAgreementsContent.flAgreementContentWithSigns.GetRowVal<byte[]>(r, "flAgreementContentWithSignsUpgs");
                    }

                    var flModels = JsonConvert.DeserializeObject<AgreementModel>(TbConfiscateAgreementTemplateModelHistory.flAgreementModel.GetRowVal(r));
                    var flTemplates = JsonConvert.DeserializeObject<AgreementTemplate[]>(TbConfiscateAgreementTemplate.flAgreementTemplates.GetRowVal(r));

                    var flKzTemplateText = flTemplates.First(x => x.Lang.Name.EqualsIgnoreCase("kz")).Text;
                    var flRuTemplateText = flTemplates.First(x => x.Lang.Name.EqualsIgnoreCase("ru")).Text;

                    foreach (var modelItem in flModels)
                    {
                        flKzTemplateText = flKzTemplateText.Replace(modelItem.Key, modelItem.Kz.Text);
                        flRuTemplateText = flRuTemplateText.Replace(modelItem.Key, modelItem.Ru.Text);
                    }

                    var flContent = $@"
<div>
    <div class=""row"">
        <div class=""col"">
            <div class=""pre-render p-10-mm"">
                <div class=""app-content "">
                    {flKzTemplateText}
                </div>
            </div>
        </div>
        <div class=""col"">
            <div class=""pre-render p-10-mm"">
                <div class=""app-content "">
                    {flRuTemplateText}
                </div>
            </div>
        </div>
    </div>
</div>
";
                    if (new TbAgreements().AddFilter(t => t.flAgreementNumber, flAgreementNumber).Count(re.QueryExecuter) == 0)
                    {
                        var flAgreementId = new TbAgreements().flAgreementId.GetNextId(re.QueryExecuter);
                        var flAgreementRevisionId = flAgreementId;
                        var TbAgreementsInsert = new TbAgreements().Insert()
                            .SetT(t => t.flAgreementId, flAgreementId)
                            .SetT(t => t.flAgreementNumber, flAgreementNumber)
                            .SetT(t => t.flAgreementRevisionId, flAgreementRevisionId)
                            .Set(t => t.flAgreementType, flAgreementType)
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Signed)
                            .SetT(t => t.flAgreementCreateDate, flCreateDate)
                            .SetT(t => t.flAgreementSignDate, flSignDate)
                            .SetT(t => t.flObjectId, flObjectId)
                            .Set(t => t.flObjectType, flObjectType)
                            .SetT(t => t.flTradeId, flTradeId)
                            .Set(t => t.flTradeType, flTradeType)
                            .SetT(t => t.flSellerBin, flSellerBin)
                            .SetT(t => t.flWinnerXin, flWinnerXin)
                            .SetT(t => t.flAgreementCreatorBin, flAgreementCreatorBin)
                        ;

                        var TbAgreementPdfsInsert = new TbAgreementPdfs().Insert()
                            .SetT(t => t.flAgreementId, flAgreementId)
                            .Set(t => t.flPdf, flPdf)
                            .Set(t => t.flPdfWithSigns, flPdfWithSigns)
                        ;

                        var TbAgreementModels = new TbAgreementModels().Insert()
                            .SetT(t => t.flAgreementId, flAgreementId)
                            .SetT(t => t.flAgreementRevisionId, flAgreementRevisionId)
                            .SetT(t => t.flAgreementStatus, AgreementStatuses.Signed)
                            .SetT(t => t.flDateTime, flSignDate)
                            .SetT(t => t.flComment, "Перенесён из ЭТП")
                            .SetT(t => t.flCommentDateTime, DateTime.Now)
                            .SetT(t => t.flModels, new DefaultAgrTemplate[] { })
                            .SetT(t => t.flContent, flContent)
                        ;

                        using (var transaction = re.QueryExecuter.BeginTransaction("dbAgreements"))
                        {
                            TbAgreementsInsert.Execute(re.QueryExecuter, transaction);
                            TbAgreementPdfsInsert.Execute(re.QueryExecuter, transaction);
                            TbAgreementModels.Execute(re.QueryExecuter, transaction);

                            LandObjectModelHelper.SetLandObjectBlock(flObjectId, LandObjectBlocks.SaledAgr.ToString(), re.QueryExecuter, transaction);
                            LandObjectTradeModelHelper.SetTradeStatus(flTradeId, RefLandObjectTradeStatuses.Held.ToString(), re.QueryExecuter, transaction);

                            transaction.Commit();
                        }
                    }

                        
                });

            });
        }
    }

    public abstract class TbAgreementsContentBase : QueryTable
    {
        protected TbAgreementsContentBase(string tableName, string text, string dbKey, Field[] additionalFields)
            : base(tableName, text)
        {
            var fields = new List<Field> {
                new IntField("flAgreementId", "Id договора"),
                new BinaryField("flAgreementContent", "Договор (pdf)"),
                new BinaryField("flAgreementContentWithSigns", "Договор с подписями (pdf)"),
            };
            fields.AddRange(additionalFields);
            Fields = fields.ToArray();
            DbKey = dbKey;
        }

        public IntField flAgreementId { get { return (IntField)this["flAgreementId"]; } }
        public BinaryField flAgreementContent { get { return (BinaryField)this["flAgreementContent"]; } }
        public BinaryField flAgreementContentWithSigns { get { return (BinaryField)this["flAgreementContentWithSigns"]; } }
    }

    public class TbUpgsAgreementsContent : TbAgreementsContentBase
    {
        public TbUpgsAgreementsContent() : base("TbUpgsAgreementsContent", "Договоры УПГС (pdf)", "dbAuction", new Field[] { }) { }
    }

    public class TbConfiscatedAgreementsContent : TbAgreementsContentBase
    {
        public TbConfiscatedAgreementsContent() : base("TbConfiscatedAgreementsContent", "Договоры Конфиската (pdf)", "dbAuction", new Field[] { }) { }
    }

    public class TbAuctionObjects : QueryTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TbAuctionObjects"/> class.
        /// </summary>
        public TbAuctionObjects()
            : base("TbAuctionObjects", "Объекты для продажи на аукционе")
        {
            Fields = new[] {
                new IntField("flObjectId", "Номер объекта"),
                new TextField("flSource", "Источник информации об объекте", 100),
                new IntField("flSourceObjectId", "Id Объекта источника"),
                new TextField("flObjectType", "Тип объекта"),
                new TextField("flSellerBin", "ИИН/БИН продавца/наймодателя"),

                new TextField("flSellerInfoRu", "Продавец/Наймодатель (рус.)"),
                new TextField("flSellerInfoKz", "Продавец/Наймодатель (каз.)"),

                new TextField("flSellerFirstPerson", "Первый руководитель"),
                new TextField("flSellerFirstPersonPosition", "Должность первого руководителя"),
                new TextField("flSellerPhoneRu", "Телефоны для справок"),
                new TextField("flSellerPhoneKz", "Телефоны для справок"),

                new TextField("flBalansHolderInfoRu", "Балансодержатель (рус.)"),
                new TextField("flBalansHolderInfoKz", "Балансодержатель (каз.)"),

                new TextField("flNameRu", "Наименование объекта (рус.)", 300),
                new TextField("flNameKz", "Наименование объекта (каз.)", 300),

                new TextField("flDescriptionRu", "Описание объекта (рус.)"),
                new TextField("flDescriptionKz", "Описание объекта (каз.)"),

                new TextField("flSellerAdrIndex", "Продавец/Наймодатель. Индекс", 6).Required(),
                new TextField("flSellerAdrAdr", "Продавец/Наймодатель. Нас. пункт, дом, кв.", 256).Required(),


                new TextField("flObjectAdrAdr", "Объект. Нас. пункт, дом, кв.", 256).Required(),

                new TextField("flNote", "Примечание"),

                new TextField("flSellerGeoData", "Информация о местонахождении продавца на карте"),
                new TextField("flObjectGeoData", "Информация о местонахождении объекта на карте"),
                new TextField("flMetaData", "Метаданные"),
                new TextField("flMetaDataType", "Тип метаданных"),
            };
            DbKey = "dbAuction";
        }

        /// <summary>
        /// Gets the object identifier field 
        /// </summary>
        public IntField flObjectId { get { return (IntField)this["flObjectId"]; } }
        /// <summary>
        /// Gets the source field 
        /// </summary>
        public TextField flSource { get { return (TextField)this["flSource"]; } }
        /// <summary>
        /// Gets the source object identifier field 
        /// </summary>
        public IntField flSourceObjectId { get { return (IntField)this["flSourceObjectId"]; } }
        /// <summary>
        /// Gets the object type field 
        /// </summary>
        public TextField flObjectType { get { return (TextField)this["flObjectType"]; } }
        /// <summary>
        /// Gets the seller bin field 
        /// </summary>
        public TextField flSellerBin { get { return (TextField)this["flSellerBin"]; } }

        /// <summary>
        /// Gets the seller information ru field 
        /// </summary>
        public TextField flSellerInfoRu { get { return (TextField)this["flSellerInfoRu"]; } }
        /// <summary>
        /// Gets the seller information kz field 
        /// </summary>
        public TextField flSellerInfoKz { get { return (TextField)this["flSellerInfoKz"]; } }

        /// <summary>
        /// Gets the seller first person field 
        /// </summary>
        public TextField flSellerFirstPerson { get { return (TextField)this["flSellerFirstPerson"]; } }
        /// <summary>
        /// Gets the seller first person position field 
        /// </summary>
        public TextField flSellerFirstPersonPosition { get { return (TextField)this["flSellerFirstPersonPosition"]; } }
        /// <summary>
        /// Gets the seller phone ru field 
        /// </summary>
        public TextField flSellerPhoneRu { get { return (TextField)this["flSellerPhoneRu"]; } }
        /// <summary>
        /// Gets the seller phone kz field 
        /// </summary>
        public TextField flSellerPhoneKz { get { return (TextField)this["flSellerPhoneKz"]; } }

        /// <summary>
        /// Gets the name ru field 
        /// </summary>
        public TextField flNameRu { get { return (TextField)this["flNameRu"]; } }
        /// <summary>
        /// Gets the name kz field 
        /// </summary>
        public TextField flNameKz { get { return (TextField)this["flNameKz"]; } }

        /// <summary>
        /// Gets the description ru field 
        /// </summary>
        public TextField flDescriptionRu { get { return (TextField)this["flDescriptionRu"]; } }
        /// <summary>
        /// Gets the description kz field 
        /// </summary>
        public TextField flDescriptionKz { get { return (TextField)this["flDescriptionKz"]; } }

        /// <summary>
        /// Gets the balans holder information ru field 
        /// </summary>
        public TextField flBalansHolderInfoRu { get { return (TextField)this["flBalansHolderInfoRu"]; } }
        /// <summary>
        /// Gets the balans holder information kz field 
        /// </summary>
        public TextField flBalansHolderInfoKz { get { return (TextField)this["flBalansHolderInfoKz"]; } }

        /// <summary>
        /// Gets the seller adr country field 
        /// </summary>
        public ReferenceTextField flSellerAdrCountry { get { return (ReferenceTextField)this["flSellerAdrCountry"]; } }
        /// <summary>
        /// Gets the seller adr index field 
        /// </summary>
        public TextField flSellerAdrIndex { get { return (TextField)this["flSellerAdrIndex"]; } }
        /// <summary>
        /// Gets the seller adr obl field 
        /// </summary>
        public ReferenceTextField flSellerAdrObl { get { return (ReferenceTextField)this["flSellerAdrObl"]; } }
        /// <summary>
        /// Gets the seller adr reg field 
        /// </summary>
        public ReferenceTextField flSellerAdrReg { get { return (ReferenceTextField)this["flSellerAdrReg"]; } }
        /// <summary>
        /// Gets the seller adr adr field 
        /// </summary>
        public TextField flSellerAdrAdr { get { return (TextField)this["flSellerAdrAdr"]; } }

        /// <summary>
        /// Gets the object adr country field 
        /// </summary>
        public ReferenceTextField flObjectAdrCountry { get { return (ReferenceTextField)this["flObjectAdrCountry"]; } }
        /// <summary>
        /// Gets the object adr obl field 
        /// </summary>
        public ReferenceTextField flObjectAdrObl { get { return (ReferenceTextField)this["flObjectAdrObl"]; } }
        /// <summary>
        /// Gets the object adr reg field 
        /// </summary>
        public ReferenceTextField flObjectAdrReg { get { return (ReferenceTextField)this["flObjectAdrReg"]; } }
        /// <summary>
        /// Gets the object adr adr field 
        /// </summary>
        public TextField flObjectAdrAdr { get { return (TextField)this["flObjectAdrAdr"]; } }

        /// <summary>
        /// Gets the note field 
        /// </summary>
        public TextField flNote { get { return (TextField)this["flNote"]; } }

        public TextField flSellerGeoData { get { return (TextField)this["flSellerGeoData"]; } }
        public TextField flObjectGeoData { get { return (TextField)this["flObjectGeoData"]; } }
        public TextField flMetaData { get { return (TextField)this["flMetaData"]; } }
        public TextField flMetaDataType { get { return (TextField)this["flMetaDataType"]; } }
    }

    public class TbAuctions : QueryTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TbAuctions"/> class.
        /// </summary>
        public TbAuctions() : base("TbAuctions", "Аукционы")
        {
            Fields = new Field[] {
                new IntField("flAuctionId", "Номер торгов"),
                new IntField("flObjectId", "Id объекта"),
                new TextField("flSource", "Источник информации об объекте"),
                new IntField("flSourceObjectId", "Id Объекта источника"),
                new TextField("flAuctionType", "Тип торгов"),
                new DateTimeField("flStartDate", "Дата и время начала торгов"),
                new MoneyField("flStartCost", "Стартовая цена, тг."),
                new MoneyField("flMinCost", "Минимальная цена, тг."),
                new MoneyField("flGuranteePaymentCost", "Гарантийный взнос, тг."),
                new IntField("flMinParticipantsCount", "Минимальное количество участников"),
                new DateTimeField("flPublishDate", "Дата публикации"),

                new TextField("flSellerIik", "Расчетный счет"),
                new TextField("flSellerBik", "БИК"),
                new TextField("flSellerKnp", "КНП"),
                new TextField("flSellerKbe", "Кбе"),
                new TextField("flSellerCode", "Код учреждения"),
                new TextField("flAuctionNoteRu", "Примечание (рус.)"),
                new TextField("flAuctionNoteKz", "Примечание (каз.)"), 
                //Получатель платежа - тот на кого реально оформлен счет. Необходим так как не у всех продавцов есть счет, куда можно перечислять гар.взносы, поэтому они пользуется "чужими" счетами.
                //Если продавец сам является держателем счета, то эти поля будут NULL
                new TextField("flPaymentsRecipientInfoRu", "Получатель платежа (рус.)"),
                new TextField("flPaymentsRecipientInfoKz", "Получатель платежа (каз.)"),
                new TextField("flNote", "Примечание (рус.)"),
                new TextField("flNoteKz", "Примечание (каз.)"),
                new TextField("flMetaData", "Доп. данные"),
                new TextField("flLandOwnershipType", "flLandOwnershipType"),
                new TextField("flMetaDataType", "Тип метаданных"),
                new DateTimeField("flAcceptAppsEndDate", "Дата окончания приема заявок"),
            };
            DbKey = "dbAuction";
        }

        /// <summary>
        /// Gets the auction identifier field 
        /// </summary>
        public IntField flAuctionId { get { return (IntField)this["flAuctionId"]; } }
        /// <summary>
        /// Gets the object identifier field 
        /// </summary>
        public IntField flObjectId { get { return (IntField)this["flObjectId"]; } }

        /// <summary>
        /// Gets the source field 
        /// </summary>
        public TextField flSource { get { return (TextField)this["flSource"]; } }
        /// <summary>
        /// Gets the source object identifier field 
        /// </summary>
        public IntField flSourceObjectId { get { return (IntField)this["flSourceObjectId"]; } }

        /// <summary>
        /// Gets the auction type field 
        /// </summary>
        public TextField flAuctionType { get { return (TextField)this["flAuctionType"]; } }
        /// <summary>
        /// Gets the start date field 
        /// </summary>
        public DateTimeField flStartDate { get { return (DateTimeField)this["flStartDate"]; } }
        /// <summary>
        /// Gets the start cost field 
        /// </summary>
        public MoneyField flStartCost { get { return (MoneyField)this["flStartCost"]; } }
        /// <summary>
        /// Gets the minimum cost field 
        /// </summary>
        public MoneyField flMinCost { get { return (MoneyField)this["flMinCost"]; } }

        /// <summary>
        /// Gets the minimum participants count field 
        /// </summary>
        public IntField flMinParticipantsCount { get { return (IntField)this["flMinParticipantsCount"]; } }

        /// <summary>
        /// Gets the gurantee payment cost field 
        /// </summary>
        public MoneyField flGuranteePaymentCost { get { return (MoneyField)this["flGuranteePaymentCost"]; } }

        public ReferenceTextField flPayPeriod { get { return (ReferenceTextField)this["flPayPeriod"]; } }

        /// <summary>
        /// Gets the publish date field 
        /// </summary>
        public DateTimeField flPublishDate { get { return (DateTimeField)this["flPublishDate"]; } }

        /// <summary>
        /// Gets the seller iik field 
        /// </summary>
        public TextField flSellerIik { get { return (TextField)this["flSellerIik"]; } }
        /// <summary>
        /// Gets the seller bik field 
        /// </summary>
        public TextField flSellerBik { get { return (TextField)this["flSellerBik"]; } }
        /// <summary>
        /// Gets the seller KNP field 
        /// </summary>
        public TextField flSellerKnp { get { return (TextField)this["flSellerKnp"]; } }
        /// <summary>
        /// Gets the seller kbe field 
        /// </summary>
        public TextField flSellerKbe { get { return (TextField)this["flSellerKbe"]; } }
        /// <summary>
        /// Gets the seller code field 
        /// </summary>
        public TextField flSellerCode { get { return (TextField)this["flSellerCode"]; } }
        /// <summary>
        /// Gets the auction note ru field 
        /// </summary>
        public TextField flAuctionNoteRu { get { return (TextField)this["flAuctionNoteRu"]; } }
        /// <summary>
        /// Gets the auction note kz field 
        /// </summary>
        public TextField flAuctionNoteKz { get { return (TextField)this["flAuctionNoteKz"]; } }
        /// <summary>
        /// Gets the payments recipient information ru field 
        /// </summary>
        public TextField flPaymentsRecipientInfoRu { get { return (TextField)this["flPaymentsRecipientInfoRu"]; } }
        /// <summary>
        /// Gets the payments recipient information kz field 
        /// </summary>
        public TextField flPaymentsRecipientInfoKz { get { return (TextField)this["flPaymentsRecipientInfoKz"]; } }

        public TextField flNote { get { return (TextField)this["flNote"]; } }
        public TextField flNoteKz { get { return (TextField)this["flNoteKz"]; } }
        public TextField flMetaData { get { return (TextField)this["flMetaData"]; } }
        public TextField flMetaDataType { get { return (TextField)this["flMetaDataType"]; } }
        public TextField flLandOwnershipType { get { return (TextField)this["flLandOwnershipType"]; } }
        public DateTimeField flAcceptAppsEndDate { get { return (DateTimeField)this["flAcceptAppsEndDate"]; } }
    }

    public abstract class TbAgreementTemplateBase : QueryTable
    {
        protected TbAgreementTemplateBase(string tableName, string text, string dbKey)
            : base(tableName, text)
        {
            Fields = new Field[] {
                new IntField("flAgreementId", "Id договора"),
                new TextField("flAgreementNumber", "Номер договора"),
                new DateField("flAgreementDate", "Дата договора"),
                new TextField("flDocNumber", "Номер документа"),
                new JsonField<AgreementTemplate[]>("flAgreementTemplates", "Шаблоны договора"),
                new IntField("flUserId", "Id пользователя")
            };
            DbKey = dbKey;
        }

        public IntField flAgreementId { get { return (IntField)this["flAgreementId"]; } }
        public TextField flAgreementNumber { get { return (TextField)this["flAgreementNumber"]; } }
        public DateField flAgreementDate { get { return (DateField)this["flAgreementDate"]; } }
        public TextField flDocNumber { get { return (TextField)this["flDocNumber"]; } }
        public JsonField<AgreementTemplate[]> flAgreementTemplates { get { return (JsonField<AgreementTemplate[]>)this["flAgreementTemplates"]; } }
        public IntField flUserId { get { return (IntField)this["flUserId"]; } }
    }

    public class AgreementTemplate
    {
        public string Text { get; set; }
        public Language Lang { get; set; }
        public AgreementTemplate() { }

        public AgreementTemplate(string text, Language lang)
        {
            Text = text;
            Lang = lang;
        }
    }

    public class Language
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Language(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class TbConfiscateAgreementTemplate : TbAgreementTemplateBase
    {
        public TbConfiscateAgreementTemplate() : base("TbConfiscateAgreementTemplate", "Шаблоны договоров по конфискованному имуществу", "dbAuction")
        {
        }
    }

    public abstract class TbAgreementsBase : QueryTable
    {
        protected TbAgreementsBase(string tableName, string text, string dbKey, Field[] additionalFields)
            : base(tableName, text)
        {
            var fields = new List<Field> {
                new IntField("flAuctionId", "Номер аукциона"),
                new IntField("flAgreementId", "Id договора"),
                new TextField("flAgreementType", "Тип договора"),
                new TextField("flAgreementStatus", "Статус"),
                new DateTimeField("flCreateDate", "Дата создания"),
                new DateTimeField("flActivateDate", "Дата исполнения"),
            };
            fields.AddRange(additionalFields);
            Fields = fields.ToArray();
            DbKey = dbKey;
        }
        public IntField flAuctionId { get { return (IntField)this["flAuctionId"]; } }
        public IntField flAgreementId { get { return (IntField)this["flAgreementId"]; } }
        public TextField flAgreementType { get { return (TextField)this["flAgreementType"]; } }
        public TextField flAgreementStatus { get { return (TextField)this["flAgreementStatus"]; } }
        public DateTimeField flCreateDate { get { return (DateTimeField)this["flCreateDate"]; } }
        public DateTimeField flActivateDate { get { return (DateTimeField)this["flActivateDate"]; } }
    }

    public class TbConfiscateAgreements : TbAgreementsBase
    {
        public TbConfiscateAgreements()
            : base("TbConfiscateAgreements", "Договоры конфиската", "dbAuction", new Field[] { })
        {
        }
    }

    public abstract class TbAgreementTemplateModelHistoryBase : QueryTable
    {
        protected TbAgreementTemplateModelHistoryBase(string tableName, string text, string dbKey)
            : base(tableName, text)
        {
            Fields = new Field[] {
                new IntField("flAgreementId", "Id договора"),
                new IntField("flUserId", "Id пользователя"),
                new JsonField<AgreementModel>("flAgreementModel", "Модель договора"),
                new DateTimeField("flDate", "Дата"),
                new BooleanField("flIsActive", "Активная")
            };
            DbKey = dbKey;
        }

        public IntField flAgreementId { get { return (IntField)this["flAgreementId"]; } }
        public IntField flUserId { get { return (IntField)this["flUserId"]; } }
        public JsonField<AgreementModel> flAgreementModel { get { return (JsonField<AgreementModel>)this["flAgreementModel"]; } }
        public DateTimeField flDate { get { return (DateTimeField)this["flDate"]; } }
        public BooleanField flIsActive { get { return (BooleanField)this["flIsActive"]; } }
    }

    public class AgreementModel : List<AgreementModelItem>
    {
        public void AppendAbsentModelItems(AgreementModel value)
        {
            foreach (var item in value)
            {
                if (this.Any(x => x.Key.Equals(item.Key)))
                {
                    continue;
                }
                Add(item);
            }
        }
    }

    public class AgreementModelItem
    {
        public AgreementModelItem(string key, ModelValueRu ru, ModelValueKz kz)
        {
            Key = key;
            Ru = ru;
            Kz = kz;
        }
        public string Key { get; set; }
        public ModelValueRu Ru { get; set; }
        public ModelValueKz Kz { get; set; }
    }

    public class ModelValue
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Language Lang { get; set; }
        public ModelValueType Type { get; set; }
        public bool Required { get; set; }
        public ModelValue() { }
        public ModelValue(string title, string text, Language lang, ModelValueType type, bool required)
        {
            Text = text;
            Lang = lang;
            Title = title;
            Type = type;
            Required = required;
        }
    }

    public class ModelValueRu : ModelValue
    {
        public ModelValueRu()
        {
            Lang = new Ru();
        }

        public ModelValueRu(string title, string text)
        {
            Text = text;
            Title = title;
            Lang = new Ru();
            Type = ModelValueType.String;
            Required = false;
        }

        public ModelValueRu(string title, string text, ModelValueType type, bool required)
        {
            Text = text;
            Title = title;
            Lang = new Ru();
            Type = type;
            Required = required;
        }
    }
    public class ModelValueKz : ModelValue
    {
        public ModelValueKz()
        {
            Lang = new Kz();
        }

        public ModelValueKz(string title, string text)
        {
            Text = text;
            Title = title;
            Lang = new Kz();
            Type = ModelValueType.String;
            Required = false;
        }

        public ModelValueKz(string title, string text, ModelValueType type, bool required)
        {
            Text = text;
            Title = title;
            Lang = new Kz();
            Type = type;
            Required = required;
        }
    }

    public class Kz : Language
    {
        public Kz() : base("Kz", "Версия на государственном языке") { }
    }

    public class Ru : Language
    {
        public Ru() : base("Ru", "Версия на русском языке") { }
    }

    public enum ModelValueType
    {
        String,
        Int,
        DateTime,
        Decimal,
        Iban,
        Bik,
        Kbe,
        Knp,
        Kbk,
        Xin
    }

    public class TbConfiscateAgreementTemplateModelHistory : TbAgreementTemplateModelHistoryBase
    {
        public TbConfiscateAgreementTemplateModelHistory()
            : base("TbConfiscateAgreementTemplateModelHistory", "История изменении договоров", "dbAuction")
        {
        }
    }


}
