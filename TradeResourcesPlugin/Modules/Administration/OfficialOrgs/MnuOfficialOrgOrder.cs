using System;
using System.Linq;
using Yoda.Interfaces;
using Yoda.Interfaces.Forms;
using Yoda.Interfaces.Forms.Components;
using YodaQuery;
using YodaHelpers.ActionMenus;
using YodaHelpers.Fields;
using YodaHelpers.OrderHelpers;
using YodaHelpers.SearchCollections;
using Source.SearchCollections;
using YodaApp.YodaHelpers.OrderHelpers;
using System.Threading.Tasks;
using CommonSource.QueryTables;
using CommonSource.Components;

namespace TradeResourcesPlugin.Modules.Administration.OfficialOrgs {
    public class MnuOfficialOrgOrder : MnuOrderBase<TbOfficialOrgOrderResult, TbOfficialOrgOrderNegotiations, OfficialOrgOrderQueryArgs> {
        public const string MnuName = "MnuOfficialOrgOrder";
        public MnuOfficialOrgOrder(string moduleName, OrderStandartPermissions perms) : base(moduleName, MnuName, "Приказ по компетентному органу", perms) {
            AsCallback();
        }

        public override bool IsCreateFromActionValid(ActionEnv<OfficialOrgOrderQueryArgs> env, out ActionValidityFailRedirectData redirectIfNotValid) {
            if (env.Args.OrgId == null) {
                redirectIfNotValid = new ActionValidityFailRedirectData(new ArgumentException());
                return false;
            }
            if (new TbOfficialOrg().AddFilter(t => t.flOrgId, env.Args.OrgId.Value).Count(env.QueryExecuter, applyDataAccessFilter: true) == 0) {
                redirectIfNotValid = new ActionValidityFailRedirectData(new ArgumentException("Запись не найдена"));
                return false;
            }
            redirectIfNotValid = null;
            return true;
        }

        public override Task<IsActionValidResult> IsActionValid(ActionEnv<OfficialOrgOrderQueryArgs> env) {

            var result = base.IsActionValid(env).Result;
            if (!result.IsValid) {
                return Task.FromResult(result);
            }
            if (env.CurAction.EqualsIgnoreCase(Actions.CreateNew)) {
                if (env.Args.Bin == null) {
                    return Task.FromResult(new IsActionValidResult(false, new ActionValidityFailRedirectData(Actions.Blank)));
                }
            }
            return Task.FromResult(new IsActionValidResult(true, null));
        }

        public override void RenderContent(RenderContentArgs args, RenderActionEnv<OfficialOrgOrderQueryArgs> contextEnv) {
            if (args.BlankPage != null) {
                var seletOrgBin = new SelectGrObjectWithNextBtn("flGrObjBin", contextEnv.RequestContext);
                contextEnv.Form.AddComponent(seletOrgBin);
                return;
            }

            Func<int, ValuesBag> getPrevVals = (revisionId) => {
                return new ValuesBag(new TbOfficialOrgRevisions()
                    .AddFilter(t => t.flRevisionId, revisionId)
                    .SelectFirst(t => t.Fields.ToFieldsAliases(), contextEnv.QueryExecuter)
                    .FirstRow
                );
            };

            var valsBag = new ValuesBag();

            if (args.CreateNew != null) {
                valsBag = getValuesBag(contextEnv.Args.Bin, contextEnv.RequestContext);
            } else if (args.CreateFrom != null) {
                valsBag = getValsBagWithNewGbdVals(contextEnv.Args.OrgId.Value, contextEnv.RequestContext);
            } else if (args.View != null) {
                valsBag = getPrevVals(args.View.RevisionId);
            } else if (args.Edit != null) {
                valsBag = getPrevVals(args.Edit.RevisionId);
            }
            var groupbox = new Accordion(contextEnv.T("Основные данные"));
            contextEnv.Form.AddComponent(groupbox);

            var tbRevs = new TbOfficialOrgRevisions();
            var readOnlyFields = getReadonlyFields(tbRevs);
            new Field[] {
                tbRevs.flBin,
                tbRevs.flNameRu,
                tbRevs.flNameKz,
                tbRevs.flOpf,
                tbRevs.flRegistrationDate,
                tbRevs.flRegistrationNum,
                tbRevs.flFirstPerson,
                tbRevs.flFirstPersonIin,
                tbRevs.flAccountant,
                tbRevs.flAccountantIin,

                tbRevs.flAdrCountry,
                tbRevs.flAdrIndex,
                tbRevs.flAdrObl,
                tbRevs.flAdrReg,
                tbRevs.flAdrAdr,
                tbRevs.flAdrRka,

                tbRevs.flAdrMobile,
                tbRevs.flAdrPhone,
                tbRevs.flAdrFax,
                tbRevs.flAdrMail,
                tbRevs.flAdrWeb,
                tbRevs.flNote,
                tbRevs.flActivityTypes
            }.Each(f => f.RenderCustom(groupbox, contextEnv, valsBag.GetValueOrDefault(f), readOnly: args.View != null || readOnlyFields.Contains(f)));
        }

        private ValuesBag getValsBagWithNewGbdVals(int orgId, IYodaRequestContext context) {
            var valsBag = new ValuesBag(new TbOfficialOrg()
                .AddFilter(t => t.flOrgId, orgId)
                .SelectFirst(t => t.Fields.ToFieldsAliases(), context.QueryExecuter)
                .FirstRow
            );
            var bin = (string)valsBag.GetValue(new TbOfficialOrg().flBin);
            var gbdVals = getValuesBag(bin, context);

            var tbRevs = new TbOfficialOrgRevisions();
            getReadonlyFields(tbRevs).Each(f => {
                if (gbdVals.Keys.Contains(f.FieldName)) {
                    valsBag.Set(f, gbdVals.GetValue(f));
                }
            });

            return valsBag;
        }

        public override void ValidateContent(ValidateContentArgs args, ActionEnv<OfficialOrgOrderQueryArgs> contextEnv) {
            Action validateInput = () => {
                var tbRevs = new TbOfficialOrgRevisions();
                new Field[] { tbRevs.flActivityTypes }.Each(f => f.Validate(contextEnv));
            };

            Action addDuplicateBinError = () => {
                var tbSellerRevs = new TbOfficialOrgRevisions();
                contextEnv.AddError(tbSellerRevs.flBin.FieldName, "Указанные БИН уже имеется в системе");
            };
            if (args.BlankPage != null) {
                var seletOrgBin = new SelectGrObjectWithNextBtn("flGrObjBin", contextEnv.RequestContext);
                var bin = seletOrgBin.GetSelectedBin(contextEnv.FormCollection);
                if (string.IsNullOrEmpty(bin)) {
                    contextEnv.AddError("flGrObjBin", "Необходимо выбрать объект из ГР");
                    return;
                }
                if (new TbOfficialOrg().AddFilter(t => t.flBin, bin).Count(contextEnv.QueryExecuter) > 0) {
                    addDuplicateBinError();
                    return;
                }

                var collection = getGrSearchCollection();

                if (collection.GetItem(bin, contextEnv.RequestContext).SearchItemId == null) {
                    contextEnv.AddError("flGrObjBin", "Объект с указанным БИН не найден в ГР");
                    return;
                }
                SetRedirectToAction(contextEnv, Actions.CreateNew, new OfficialOrgOrderQueryArgs { MenuAction = Actions.CreateNew, Bin = bin });
            } else if (args.CreateNew != null) {
                var bin = contextEnv.Args.Bin;
                if (new TbOfficialOrg().AddFilter(t => t.flBin, bin).Count(contextEnv.QueryExecuter) > 0) {
                    addDuplicateBinError();
                }
            } else if (args.CreateFrom != null) {
                validateInput();
            } else if (args.Edit != null) {
                validateInput();
            } else if (args.Accept != null) {
                var tbRevs = new TbOfficialOrgRevisions()
                    .AddFilter(t => t.flRevisionId, args.Accept.RevisionId);
                var orgId = (int)tbRevs
                    .SelectScalar(t => t.flOrgId, contextEnv.QueryExecuter);
                var bin = (string)tbRevs.SelectScalar(t => t.flBin, contextEnv.QueryExecuter);
                if (new TbOfficialOrg().AddFilterNot(t => t.flOrgId, orgId).AddFilter(t => t.flBin, bin).Count(contextEnv.QueryExecuter) > 0) {
                    addDuplicateBinError();
                }
            } else {
                throw new NotImplementedException();
            }
        }

        public override void ProcessContent(ProcessContentArgs args, ActionEnv<OfficialOrgOrderQueryArgs> contextEnv) {
            Action<DataModifingQueryProxy<TbOfficialOrgRevisions>> populate = (q) => {
                new Field[] { q.Table.flActivityTypes }.Each(f => q.DataModifingQuery.Set(f, f.GetValue(contextEnv)));
            };

            if (args.CreateNew != null) {
                var valsBag = getValuesBag(contextEnv.Args.Bin, contextEnv.RequestContext);
                var updateOrInsert = new TbOfficialOrgRevisions()
                    .Self(out var tbOrg)
                    .Insert();

                updateOrInsert.Table.Fields.Each(f => updateOrInsert.DataModifingQuery.Set(f, valsBag.GetValueOrDefault(f, DBNull.Value)));
                updateOrInsert
                    .Set(t => t.flRevisionId, args.CreateNew.RevisionId)
                    .Set(t => t.flOrgId, new TbOfficialOrg().flOrgId.GetNextId(contextEnv.QueryExecuter))
                    .Set(t => t.flNote, tbOrg.flNote.GetVal(contextEnv));

                populate(updateOrInsert);
                updateOrInsert.Execute(contextEnv.QueryExecuter, args.CreateNew.Transaction);
            } else if (args.CreateFrom != null) {
                var valsBag = getValsBagWithNewGbdVals(contextEnv.Args.OrgId.Value, contextEnv.RequestContext);

                var updateOrInsert = new TbOfficialOrgRevisions()
                    .Self(out var tbOrg)
                    .Insert();
                updateOrInsert.Table.Fields.Each(f => updateOrInsert.DataModifingQuery.Set(f, valsBag.GetValueOrDefault(f, DBNull.Value)));

                updateOrInsert.Set(t => t.flRevisionId, args.CreateFrom.RevisionId)
                    .Set(t => t.flOrgId, contextEnv.Args.OrgId.Value)
                    .Set(t => t.flNote, tbOrg.flNote.GetVal(contextEnv))
                    ;
                populate(updateOrInsert);
                updateOrInsert.Execute(contextEnv.QueryExecuter, args.CreateFrom.Transaction);
            } else if (args.Edit != null) {
                var updateOrInsert = new TbOfficialOrgRevisions()
                    .AddFilter(t => t.flRevisionId, args.Edit.RevisionId)
                    .Update();
                populate(updateOrInsert);
                updateOrInsert.Execute(contextEnv.QueryExecuter, args.Edit.Transaction);
            } else if (args.Accept != null) {
                var r = new TbOfficialOrgRevisions()
                    .AddFilter(t => t.flRevisionId, args.Accept.RevisionId)
                    .SelectFirst(t => t.Fields.ToFieldsAliases(), contextEnv.QueryExecuter, args.Accept.Transaction);

                Action<DataModifingQueryProxy<TbOfficialOrg>> populateAndExecute = (updateOrInsert) => {
                    updateOrInsert.Table.Fields.Exclude(updateOrInsert.Table.flOrgId).Each(f =>
                        updateOrInsert.DataModifingQuery.Set(f, f.GetRowVal<object>(r.FirstRow) ?? DBNull.Value)
                    );
                    updateOrInsert.Execute(contextEnv.QueryExecuter, args.Accept.Transaction);
                };

                var orgId = r.Query.flOrgId.GetRowVal(r.FirstRow);

                if (new TbOfficialOrg().AddFilter(t => t.flOrgId, orgId).Count(contextEnv.QueryExecuter, args.Accept.Transaction) == 0) {
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


                new TbOfficialOrgActivityTypes().AddFilter(t => t.flOfficialOrgId, orgId).Remove().Execute(contextEnv.QueryExecuter, args.Accept.Transaction);
                var activityTypes = r.Query.flActivityTypes.GetGoodVal(r.FirstRow);

                foreach (var item in activityTypes) {
                    new TbOfficialOrgActivityTypes()
                        .Insert()
                        .SetT(t => t.flOfficialOrgId, orgId)
                        .SetT(t => t.flActivityType, item.Value)
                        .SetT(t => t.flFrom, item.From)
                        .SetT(t => t.flTo, item.To)
                        .Execute(contextEnv.QueryExecuter, args.Accept.Transaction);
                }

            } else {
                throw new NotImplementedException();
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
        private Field[] getReadonlyFields(TbOfficialOrgRevisions tbRevs) {
            return new Field[]{
                tbRevs.flBin,
                tbRevs.flNameRu,
                tbRevs.flNameKz,
                tbRevs.flAdrCountry,
                tbRevs.flAdrObl,
                tbRevs.flAdrReg,
                tbRevs.flAdrAdr,
                tbRevs.flAdrIndex,
                tbRevs.flAdrFax,
                tbRevs.flAdrNote,
                tbRevs.flAdrMail,
                tbRevs.flAdrMobile,
                tbRevs.flAdrPhone,
                tbRevs.flAdrRka,
                tbRevs.flAdrWeb,
                tbRevs.flAccountant,
                tbRevs.flAccountantIin,
                tbRevs.flFirstPerson,
                tbRevs.flFirstPersonIin,
                tbRevs.flOpf,
                tbRevs.flRegistrationDate,
                tbRevs.flRegistrationNum,
            };
        }
    }
    public class OfficialOrgOrderQueryArgs : OrderQueryArgs {
        public int? OrgId { get; set; }
        public string Bin { get; set; }
    }


}