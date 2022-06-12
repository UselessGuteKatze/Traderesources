//using Source.Classes.Objects;
//using Source.DataSchema.Objects;
//using Source.References.Objects;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradeResourcesPlugin.Modules.Menus.Objects;
//using UsersResources;
//using Yoda.Interfaces;
//using YodaQuery;

//namespace TradeResourcesPlugin.Helpers {
//    public static class RemoveOrder {
//        public static void ForHydroCarbonObject(string objNumber, int regUserId, int execUserId, IQueryExecuter qe, ITransaction transaction = null)
//        {
//            var dateTime = qe.GetDateTime();
//            var revId = new TbObjectsOrderResult().flSubjectId.GetNextId(qe);
//            var regActId = new TbObjectsOrderNegotiations().flActionId.GetNextId(qe);
//            var execActId = new TbObjectsOrderNegotiations().flActionId.GetNextId(qe);

//            var objId = new TbObjects().AddFilter(t => t.flAreaNumber, objNumber).AddFilter(t => t.flSubType, "Hydrocarbon").SelectScalar(t => t.flId, qe).Value;

//            var objectModel = ObjectsModelHelper.GetObjectModel(objId, qe);
//            objectModel.RevisionId = revId;
//            if (objectModel.Status != "Active" || objectModel.Block != ObjectBlocks.ActiveFree)
//            {
//                throw new NotImplementedException($"Можно удалять только активные объекты. Статус объекта: '{objectModel.Status}'");
//            }
//            objectModel.Status = "Deleted";
//            objectModel.Block = ObjectBlocks.DeletedExcempted;

//            var tb = new TbObjects().AddFilter(t => t.flId, objId).Update();
//            MnuObjectOrderBaseV2.SetModel(tb, objectModel);
//            tb.Execute(qe, transaction);

//            var tbRev = new TbObjectsRevisions().Insert();
//            MnuObjectOrderBaseV2.SetModel(tbRev, objectModel);
//            tbRev.Execute(qe, transaction);

//            new TbObjectsOrderResult().Insert()
//                .Set(t => t.flSubjectId, revId)
//                .Set(t => t.flStatus, "Complished")
//                .Set(t => t.flRegUserId, regUserId)
//                .Set(t => t.flRegDate, dateTime)
//                .Set(t => t.flExecUserId, execUserId)
//                .Set(t => t.flExecDate, dateTime)
//                .Set(t => t.flType, "Create")
//                .Set(t => t.flOrderNote, "")
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
//                .Set(t => t.flNote, "")
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
//                .Set(t => t.flNote, "")
//                .Set(t => t.flIsActive, true)
//                .Execute(qe, transaction);
//        }

//        public static void ForHydroCarbonObjects(string[] objNumbers, int regUserId, int execUserId, IQueryExecuter qe, ITransaction transaction)
//        {
//            objNumbers.Each(objNumber => ForHydroCarbonObject(objNumber, regUserId, execUserId, qe, transaction));
//        }
//    }
//}