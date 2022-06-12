using System;
using System.Collections.Generic;
using Source.SearchCollections;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Menu;
using YodaHelpers.ActionMenus;
using YodaHelpers.OrderHelpers;
using YodaHelpers.SearchCollections;
using YodaHelpers.Fields;
using YodaQuery;
using CommonSource.QueryTables;

namespace TradeResourcesPlugin.Modules.Administration.OfficialOrgs {
    public class MnuUploadOfficialOrgs : FrmMenu {
        public const string MnuName = nameof(MnuUploadOfficialOrgs);
        public MnuUploadOfficialOrgs() : base(MnuName, "Загрузить рабочие органы") {
            Path("upload-official-orgs");
            Access("admin-only");
            OnRendering(re => {
                var tbOrderResult = new TbOfficialOrgOrderResult();
                tbOrderResult.flOrderNote.RenderCustomT(re.Form, re.AsFormEnv(), null);
                re.Form.AddSubmitButton("btnUpload", "upload");
            });
            OnProcessing(env => {
                var bins = new[] {
                    "980540000882","060240008934","050140006635","060140003781","050240007111","060140013431","060140004600","060240011240","130640003676","130540016189","050140008027","130440028327","050140009401","060240002668",
"060240010301","060140013154","060140011346","060140006240","060240002717","041240003249","060340007563","060140012740","000540001322","060140007803","060240011131","050140001971","060140012760","060240005296",
"060240009497","980740001570","070740007553","060140015130","060140009790","020940003014","060240008449","060240009645","060240009000","060140014221","050140006060","060240005305","060340007028","060240008349",
"060240009833","060240008230","060140009314","060240002628","060140012780","060240007361","060140007060","130440014238","060240010153","060140007982","060140013194","130940003647","060140008970","060140008336",
"150440012611","060140009265","030840001606","110140010384","961140001342","060140002674","060140008188","060240010282","060240010986","180140011350","060240007420","060140005847","060140006469","060240007034",
"060240008944","060240009576","060240007936","060140003800","060140005649","041240007579","120340015588","130940013585","151040001684","150440001616","160540008867","150840019871","160740012763","170240004111",
"170240002501","170940032142","180140011816","190240001241","190540004205","200140011585","200740017338"
                };
                foreach(var bin in bins) {
                    var revId = create(bin, env.AsFormEnv());
                    sendToExec(revId, env.AsFormEnv());
                    execute(revId, env.AsFormEnv());
                }
            });
        }

        private int create(string bin, FormEnvironment env) {
            var orderManager = new OrderManagerUi(() => new TbOfficialOrgOrderResult(), () => new TbOfficialOrgOrderNegotiations());
            var tbOrderResult = new TbOfficialOrgOrderResult();
            var revId = tbOrderResult.flSubjectId.GetNextId(env.QueryExecuter);

            using (var transaction = env.QueryExecuter.BeginTransaction(tbOrderResult.DbKey)) {
                orderManager.Create(RefDefaultOrderTypes.Values.Create).Process(revId, transaction, env);
                processContentCreate(bin, revId, env, transaction);
                transaction.Commit();
            }
            return revId;
        }

        private void sendToExec(int revId, FormEnvironment env) {
            var orderManager = new OrderManagerUi(() => new TbOfficialOrgOrderResult(), () => new TbOfficialOrgOrderNegotiations());
            var tbOrderResult = new TbOfficialOrgOrderResult();
            using (var transaction = env.QueryExecuter.BeginTransaction(tbOrderResult.DbKey)) {
                orderManager.Run(revId).Process(transaction, env);
                transaction.Commit();
            }
        }

        private void execute(int revId, FormEnvironment env) {
            var orderManager = new OrderManagerUi(() => new TbOfficialOrgOrderResult(), () => new TbOfficialOrgOrderNegotiations());
            var tbOrderResult = new TbOfficialOrgOrderResult();
            using (var transaction = env.QueryExecuter.BeginTransaction(tbOrderResult.DbKey)) {
                orderManager.Execute(revId).Process(transaction, env);
                processContentExecute(revId, env, transaction);
                transaction.Commit();
            }
        }

        private void processContentCreate(string bin, int revId, FormEnvironment contextEnv, ITransaction transaction) {
            Action<DataModifingQueryProxy<TbOfficialOrgRevisions>> populate = (q) => {
                new Field[] { q.Table.flActivityTypes }.Each(f => q.DataModifingQuery.Set(f, (object)"[{\"Value\":\"Lands\",\"From\":\"2020-10-01T00:00:00\",\"To\":null}]"));
            };
            var valsBag = getValuesBag(bin, contextEnv.RequestContext);
            var updateOrInsert = new TbOfficialOrgRevisions()
                    .Self(out var tbOrg)
                    .Insert();

            updateOrInsert.Table.Fields.Each(f => updateOrInsert.DataModifingQuery.Set(f, valsBag.GetValueOrDefault(f, DBNull.Value)));
            updateOrInsert
                .Set(t => t.flRevisionId, revId)
                .Set(t => t.flOrgId, new TbOfficialOrg().flOrgId.GetNextId(contextEnv.QueryExecuter))
                .Set(t => t.flNote, tbOrg.flNote.GetVal(contextEnv));

            populate(updateOrInsert);
            updateOrInsert.Execute(contextEnv.QueryExecuter, transaction);
        }

        private void processContentExecute(int revId, FormEnvironment contextEnv, ITransaction transaction) {
            var r = new TbOfficialOrgRevisions()
                    .AddFilter(t => t.flRevisionId, revId)
                    .SelectFirst(t => t.Fields.ToFieldsAliases(), contextEnv.QueryExecuter, transaction);

            Action<DataModifingQueryProxy<TbOfficialOrg>> populateAndExecute = (updateOrInsert) => {
                updateOrInsert.Table.Fields.Exclude(updateOrInsert.Table.flOrgId).Each(f =>
                    updateOrInsert.DataModifingQuery.Set(f, f.GetRowVal<object>(r.FirstRow) ?? DBNull.Value)
                );
                updateOrInsert.Execute(contextEnv.QueryExecuter, transaction);
            };

            var orgId = r.Query.flOrgId.GetRowVal(r.FirstRow);

            if (new TbOfficialOrg().AddFilter(t => t.flOrgId, orgId).Count(contextEnv.QueryExecuter, transaction) == 0) {
                populateAndExecute(
                    new TbOfficialOrg()
                    .Insert()
                    .Set(t => t.flOrgId, orgId)
                );

            } else {
                populateAndExecute(
                    new TbOfficialOrg()
                    .AddFilter(t => t.flOrgId, orgId)
                    .Update()
                );
            }


            new TbOfficialOrgActivityTypes().AddFilter(t => t.flOfficialOrgId, orgId).Remove().Execute(contextEnv.QueryExecuter, transaction);
            var activityTypes = r.Query.flActivityTypes.GetGoodVal(r.FirstRow);

            foreach (var item in activityTypes) {
                new TbOfficialOrgActivityTypes()
                    .Insert()
                    .SetT(t => t.flOfficialOrgId, orgId)
                    .SetT(t => t.flActivityType, item.Value)
                    .SetT(t => t.flFrom, item.From)
                    .SetT(t => t.flTo, item.To)
                    .Execute(contextEnv.QueryExecuter, transaction);
            }
        }

        private GrObjectsWithKatoSearchCollection getGrSearchCollection() {
            return (GrObjectsWithKatoSearchCollection)SearchCollectionsProvider.Instance.Get(GrObjectsWithKatoSearchCollection.CollectionName);
        }

        private ValuesBag getValuesBag(string bin, IYodaRequestContext context) {
            var obj = getGrSearchCollection().GetItem(bin, context);
            var tbRevs = new TbOfficialOrgRevisions();
            if (obj.ObjectData == null) {
                return new ValuesBag()
                    .Set(tbRevs.flBin, obj.SearchItemText)
                    .Set(tbRevs.flNameRu, obj.SearchItemText);
            }
            return new ValuesBag()
                .Set(tbRevs.flBin, obj.SearchItemId)
                .Set(tbRevs.flNameRu, obj.ObjectData.NameRu)
                .Set(tbRevs.flNameKz, obj.ObjectData.NameKz)
                .Set(tbRevs.flAdrCountry, obj.ObjectData.AdrCountry)
                .Set(tbRevs.flAdrIndex, obj.ObjectData.AdrIndex)
                .Set(tbRevs.flAdrObl, obj.ObjectData.AdrObl)
                .Set(tbRevs.flAdrReg, obj.ObjectData.AdrReg)
                .Set(tbRevs.flAdrMail, obj.ObjectData.AdrMail)
                .Set(tbRevs.flAdrAdr, obj.ObjectData.AdrAdr)
                .Set(tbRevs.flAdrMobile, obj.ObjectData.AdrMobile)
                .Set(tbRevs.flAdrPhone, obj.ObjectData.AdrPhone)
                .Set(tbRevs.flAdrFax, obj.ObjectData.AdrFax)
                .Set(tbRevs.flAdrWeb, obj.ObjectData.AdrWeb)
                .Set(tbRevs.flAdrRka, obj.ObjectData.AdrRca)
                .Set(tbRevs.flAccountant, obj.ObjectData.Accountant)
                .Set(tbRevs.flAccountantIin, obj.ObjectData.AccountantIin)
                .Set(tbRevs.flFirstPerson, obj.ObjectData.FirstPerson)
                .Set(tbRevs.flFirstPersonIin, obj.ObjectData.FirstPersonIin)
                .Set(tbRevs.flOpf, obj.ObjectData.Opf)
                .Set(tbRevs.flRegistrationDate, obj.ObjectData.RegistrationDate)
                .Set(tbRevs.flRegistrationNum, obj.ObjectData.RegistrationNumber)
                .Set(tbRevs.flState, obj.ObjectData.Status)
                .Set(tbRevs.flBlock, obj.ObjectData.Block);
        }
    }
}
