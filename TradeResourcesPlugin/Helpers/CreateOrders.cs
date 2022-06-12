//using FishingSource.Helpers.Reservoir;
//using FishingSource.Models;
//using FishingSource.QueryTables.Reservoir;
//using Source.Classes.Objects;
//using Source.DataSchema;
//using Source.DataSchema.Objects;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradeResourcesPlugin.Modules.FishingMenus.Reservoirs;
//using TradeResourcesPlugin.Modules.Menus.Objects;
//using UsersResources;
//using Yoda.Interfaces;
//using YodaQuery;
//namespace TradeResourcesPlugin.Helpers {
//    public static class CreateOrders {

//        //country, region, district - reference values
//        public static void ForReservoirObject(string name, string country, string region, string district, decimal? area, int regUserId, int execUserId, IQueryExecuter qe, ITransaction transaction = null)
//        {
//            var dateTime = qe.GetDateTime();
//            var revId = new TbReservoirOrderResult().flSubjectId.GetNextId(qe);
//            var regActId = new TbReservoirOrdersNegotiations().flActionId.GetNextId(qe);
//            var execActId = new TbReservoirOrdersNegotiations().flActionId.GetNextId(qe);

//            var objectModel = new ReservoirModel();
//            objectModel.flId = revId;
//            objectModel.flRevisionId = revId;
//            objectModel.flStatus = "Active";
//            objectModel.flName = name;
//            objectModel.flCountry = country;
//            objectModel.flRegion = region;
//            objectModel.flDistrict = district;
//            objectModel.flArea = area;

//            var tb = new TbReservoirs().Insert();
//            tb
//                .SetT(t => t.flId, objectModel.flId)
//                .SetT(t => t.flRevisionId, objectModel.flRevisionId)
//                .Set(t => t.flStatus, objectModel.flStatus)
//                .Set(t => t.flName, objectModel.flName)
//                .Set(t => t.flArea, objectModel.flArea)
//                .Set(t => t.flCountry, objectModel.flCountry)
//                .Set(t => t.flRegion, objectModel.flRegion)
//                .Set(t => t.flDistrict, objectModel.flDistrict);
//            tb.Execute(qe, transaction);

//            var tbRev = new TbReservoirsRevisions().Insert();
//            tbRev
//                .SetT(t => t.flId, objectModel.flId)
//                .SetT(t => t.flRevisionId, objectModel.flRevisionId)
//                .Set(t => t.flStatus, objectModel.flStatus)
//                .Set(t => t.flName, objectModel.flName)
//                .Set(t => t.flArea, objectModel.flArea)
//                .Set(t => t.flCountry, objectModel.flCountry)
//                .Set(t => t.flRegion, objectModel.flRegion)
//                .Set(t => t.flDistrict, objectModel.flDistrict);
//            tbRev.Execute(qe, transaction);

//            new TbReservoirOrderResult().Insert()
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flStatus, "Complished")
//                .Set(t => t.flRegUserId, regUserId)
//                .Set(t => t.flRegDate, dateTime)
//                .Set(t => t.flExecUserId, execUserId)
//                .Set(t => t.flExecDate, dateTime)
//                .Set(t => t.flType, "Create")
//                .Set(t => t.flOrderNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flNoteNegotiations, "")
//                .Execute(qe, transaction);

//            new TbReservoirOrdersNegotiations().Insert()
//                .Set(t => t.flActionId, regActId)
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flUserId, regUserId)
//                .Set(t => t.flUserXin, YodaUserHelpers.GetUserXin(regUserId, qe))
//                .Set(t => t.flUserXinWithPrefix, YodaUserHelpers.GetUserXinWithPrefix(regUserId, qe))
//                .Set(t => t.flRole, "registrator")
//                .Set(t => t.flActionType, "Run")
//                .Set(t => t.flActionDate, dateTime)
//                .Set(t => t.flNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flIsActive, true)
//                .Execute(qe, transaction);

//            new TbReservoirOrdersNegotiations().Insert()
//                .Set(t => t.flActionId, execActId)
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flUserId, execUserId)
//                .Set(t => t.flUserXin, YodaUserHelpers.GetUserXin(execUserId, qe))
//                .Set(t => t.flUserXinWithPrefix, YodaUserHelpers.GetUserXinWithPrefix(execUserId, qe))
//                .Set(t => t.flRole, "executer")
//                .Set(t => t.flActionType, "Accept")
//                .Set(t => t.flActionDate, dateTime)
//                .Set(t => t.flNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flIsActive, true)
//                .Execute(qe, transaction);
//        }
//        public static void ForHydroCarbonObject(string objNumber, int regUserId, int execUserId, IQueryExecuter qe, ITransaction transaction = null)
//        {
//            if (new TbObjectsRevisions().AddFilter(t => t.flAreaNumber, objNumber).Count(qe) > 0)
//            {
//                throw new NotImplementedException($"Этот объект уже был создан: '{objNumber}'");
//            }

//            var tbSubSoils = new TbSubsoilsObjects();
//            tbSubSoils.AddFilter(t => t.flNumber, objNumber);
//            var r = tbSubSoils.SelectFirst(t => t.Fields.ToFieldsAliases(), qe);

//            var dateTime = qe.GetDateTime();

//            var objectModel = new ObjectModel();
//            objectModel.CompetentOrgId = -1;
//            objectModel.Xin = "140940023346"; //CompetentOrgBin
//            if (!string.IsNullOrEmpty(r.GetVal(t => t.flNumber)))
//            {
//                objectModel.AreaNumber = r.GetVal(t => t.flNumber);
//            }
//            if (!string.IsNullOrEmpty(r.GetVal(t => t.flRegion)))
//            {
//                objectModel.Name = r.GetVal(t => t.flRegion);
//            }
//            if (!string.IsNullOrEmpty(r.GetVal(t => t.flInfo)))
//            {
//                objectModel.Description = r.GetVal(t => t.flInfo);
//            }
//            objectModel.Status = "Active";
//            if (r.GetValOrNull(t => t.flArea).HasValue)
//            {
//                objectModel.Area = r.GetVal(t => t.flArea);
//            }
//            if (r.GetValOrNull(t => t.flBlockCount).HasValue)
//            {
//                objectModel.BlocksCount = r.GetValOrNull(t => t.flBlockCount);
//            }
//            if (!string.IsNullOrEmpty(r.GetVal(t => t.flExceptions)))
//            {
//                objectModel.Exceptions = r.GetVal(t => t.flExceptions);
//            }
//            objectModel.Type = "Subsoil";
//            objectModel.SubType = "Hydrocarbon";
//            objectModel.Block  = Source.References.Objects.ObjectBlocks.ActiveFree;
//            objectModel.SubsoilId = r.GetVal(t => t.flId);
//            objectModel.LandUrl = $"https://lands.qoldau.kz/ru/lands-map/subsoils?zoomToSubsoilNumber={r.GetVal(t => t.flNumber)}";
//            if (!string.IsNullOrEmpty(r.GetVal(t => t.flWKT)))
//            {
//                objectModel.Wkt = r.GetVal(t => t.flWKT);
//            }

//            var revId = new TbObjectsOrderResult().flSubjectId.GetNextId(qe);
//            var regActId = new TbObjectsOrderNegotiations().flActionId.GetNextId(qe);
//            var execActId = new TbObjectsOrderNegotiations().flActionId.GetNextId(qe);

//            objectModel.Id = revId;
//            objectModel.RevisionId = revId;

//            var tb = new TbObjects().Insert();
//            tb
//                .SetT(t => t.flId, objectModel.Id)
//                .SetT(t => t.flRevisionId, objectModel.RevisionId)
//                .Set(t => t.flCompetentOrgId, objectModel.CompetentOrgId)
//                .Set(t => t.flCompetentOrgBin, objectModel.Xin)
//                .Set(t => t.flAreaNumber, objectModel.AreaNumber)
//                .Set(t => t.flName, objectModel.Name)
//                .Set(t => t.flDescription, objectModel.Description)
//                .Set(t => t.flStatus, objectModel.Status)
//                .Set(t => t.flArea, objectModel.Area)
//                .Set(t => t.flBlocksCount, objectModel.BlocksCount)
//                .Set(t => t.flExceptions, objectModel.Exceptions)
//                .Set(t => t.flType, objectModel.Type)
//                .Set(t => t.flSubType, objectModel.SubType)
//                .Set(t => t.flBlock, objectModel.Block.ToString())
//                .Set(t => t.flSubsoilId, objectModel.SubsoilId)
//                .Set(t => t.flLandUrl, objectModel.LandUrl)
//                .Set(t => t.flWkt, objectModel.Wkt)
//                ;
//            tb.Execute(qe, transaction);

//            var tbRev = new TbObjectsRevisions().Insert();
//            tbRev
//                .SetT(t => t.flId, objectModel.Id)
//                .SetT(t => t.flRevisionId, objectModel.RevisionId)
//                .Set(t => t.flCompetentOrgId, objectModel.CompetentOrgId)
//                .Set(t => t.flCompetentOrgBin, objectModel.Xin)
//                .Set(t => t.flAreaNumber, objectModel.AreaNumber)
//                .Set(t => t.flName, objectModel.Name)
//                .Set(t => t.flDescription, objectModel.Description)
//                .Set(t => t.flStatus, objectModel.Status)
//                .Set(t => t.flArea, objectModel.Area)
//                .Set(t => t.flBlocksCount, objectModel.BlocksCount)
//                .Set(t => t.flExceptions, objectModel.Exceptions)
//                .Set(t => t.flType, objectModel.Type)
//                .Set(t => t.flSubType, objectModel.SubType)
//                .Set(t => t.flBlock, objectModel.Block.ToString())
//                .Set(t => t.flSubsoilId, objectModel.SubsoilId)
//                .Set(t => t.flLandUrl, objectModel.LandUrl)
//                .Set(t => t.flWkt, objectModel.Wkt)
//                ;
//            tbRev.Execute(qe, transaction);

//            new TbObjectsOrderResult().Insert()
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flStatus, "Complished")
//                .Set(t => t.flRegUserId, regUserId)
//                .Set(t => t.flRegDate, dateTime)
//                .Set(t => t.flExecUserId, execUserId)
//                .Set(t => t.flExecDate, dateTime)
//                .Set(t => t.flType, "Create")
//                .Set(t => t.flOrderNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flNoteNegotiations, "")
//                .Execute(qe, transaction);

//            new TbObjectsOrderNegotiations().Insert()
//                .Set(t => t.flActionId, regActId)
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flUserId, regUserId)
//                .Set(t => t.flUserXin, YodaUserHelpers.GetUserXin(regUserId, qe))
//                .Set(t => t.flUserXinWithPrefix, YodaUserHelpers.GetUserXinWithPrefix(regUserId, qe))
//                .Set(t => t.flRole, "registrator")
//                .Set(t => t.flActionType, "Run")
//                .Set(t => t.flActionDate, dateTime)
//                .Set(t => t.flNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flIsActive, true)
//                .Execute(qe, transaction);

//            new TbObjectsOrderNegotiations().Insert()
//                .Set(t => t.flActionId, execActId)
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flUserId, execUserId)
//                .Set(t => t.flUserXin, YodaUserHelpers.GetUserXin(execUserId, qe))
//                .Set(t => t.flUserXinWithPrefix, YodaUserHelpers.GetUserXinWithPrefix(execUserId, qe))
//                .Set(t => t.flRole, "executer")
//                .Set(t => t.flActionType, "Accept")
//                .Set(t => t.flActionDate, dateTime)
//                .Set(t => t.flNote, "Приказ создан путём заливки данных")
//                .Set(t => t.flIsActive, true)
//                .Execute(qe, transaction);
//        }
//        public static void ForHydroCarbonObjects(string[] objNumbers, int regUserId, int execUserId, IQueryExecuter qe, ITransaction transaction = null)
//        {
//            objNumbers.Each(objNumber => ForHydroCarbonObject(objNumber, regUserId, execUserId, qe, transaction));
//        }
//    }
//}
