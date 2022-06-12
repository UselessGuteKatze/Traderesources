using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraderesourcesApi.Models;
using Yoda.Application.Queries;
using YodaQuery;
using LandSource.QueryTables.LandObject;
using LandSource.References.LandObject;
using System.Data;
using System.Linq;
using ForestSource.Helpers.Object;

namespace TraderesourcesApi.Controllers {
    [ApiController]
    [Authorize("TraderesourcesApiScope")]
    [Route("[controller]")]
    public class TraderesourcesObjectsController : ControllerBase {
        private readonly ILogger<TraderesourcesObjectsController> _logger;
        private readonly IQueryExecuterSu _queryExecuter;

        public TraderesourcesObjectsController(ILogger<TraderesourcesObjectsController> logger, IQueryExecuterProvider queryExecuterProvider) {
            _logger = logger;
            _queryExecuter = queryExecuterProvider.CreateQueryExecuterSuperUser();
        }


        [HttpGet("get/{objectType}")]
        public async Task<ObjectModelResponse> Get([Required] ObjectType objectType) {

            try
            {

                var objects = new List<ObjectModelResult>();

                if (new[] { ObjectType.ALL, ObjectType.HYDROCARBON }.Contains(objectType))
                {
                    _logger.LogInformation("Selecting subsoil objects...");
                    objects.AddRange(GetHydrocarbonObjects(_queryExecuter));
                }

                if (new[] { ObjectType.ALL, ObjectType.LANDOBJECT }.Contains(objectType))
                {
                    _logger.LogInformation("Selecting land objects...");
                    objects.AddRange(GetLandObjects(_queryExecuter));
                }

                if (new[] { ObjectType.ALL, ObjectType.HUNTINGOBJECT }.Contains(objectType))
                {
                    _logger.LogInformation("Selecting hunt objects...");
                    objects.AddRange(GetHuntingObjects(_queryExecuter));
                }

                if (new[] { ObjectType.ALL, ObjectType.FISHINGOBJECT }.Contains(objectType))
                {
                    _logger.LogInformation("Selecting fish objects...");
                    objects.AddRange(GetFishingObjects(_queryExecuter));
                }

                if (new[] { ObjectType.ALL, ObjectType.FORESTOBJECT }.Contains(objectType))
                {
                    _logger.LogInformation("Selecting forest objects...");
                    objects.AddRange(GetForestObjects(_queryExecuter));
                }

                return new ObjectModelResponse(objects.ToArray(), ObjectModelResponseStatus.Ok);

            } catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                _logger.LogInformation(e.StackTrace);
                return new ObjectModelResponse(null, ObjectModelResponseStatus.Error);
            }
        }

        private static List<ObjectModelResult> GetHydrocarbonObjects(IQueryExecuterSu qe)
        {

            Dictionary<string, ObjectStatus> Statuses = new Dictionary<string, ObjectStatus>() {
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.ActiveFree.ToString(), ObjectStatus.FREE },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.ActiveHasApplication.ToString(), ObjectStatus.APPLICATIONTOTRADE },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.ActivePositiveDecision.ToString(), ObjectStatus.TRADEDECISION },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.ActiveToBeTraded.ToString(), ObjectStatus.TOBETRADED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.SaledComplete.ToString(), ObjectStatus.SALED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.SaledProt.ToString(), ObjectStatus.SALED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.SaledAgr.ToString(), ObjectStatus.SALED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.DeletedSoldBefore.ToString(), ObjectStatus.REALIZED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.DeletedSold.ToString(), ObjectStatus.SALED },
                { HydrocarbonSource.References.Object.HydrocarbonObjectBlocks.DeletedExcempted.ToString(), ObjectStatus.DELETED }
            };

            var tbObjects = new HydrocarbonSource.QueryTables.Object.TbObjects();
            var rows = tbObjects.Select(new FieldAlias[] { tbObjects.flId, tbObjects.flRevisionId, tbObjects.flBlock, tbObjects.flDescription, tbObjects.flNumber, tbObjects.flName, tbObjects.flArea, tbObjects.flWKT }, qe);

            var objects = rows.AsEnumerable().Select(r => {
                return new ObjectModelResult(
                    tbObjects.flId.GetRowVal(r),
                    tbObjects.flRevisionId.GetRowVal(r),
                    ObjectType.HYDROCARBON,
                    Statuses[tbObjects.flBlock.GetRowVal(r)],
                    $"{tbObjects.flNumber.GetRowVal(r)} - {tbObjects.flName.GetRowVal(r)}",
                    tbObjects.flDescription.GetRowVal(r),
                    tbObjects.flArea.GetRowVal(r),
                    tbObjects.flWKT.GetRowVal(r)
                );
            }).ToList();

            return objects;

        }
        private static List<ObjectModelResult> GetLandObjects(IQueryExecuterSu qe)
        {

            Dictionary<string, ObjectStatus> Statuses = new Dictionary<string, ObjectStatus>() {
                { LandObjectBlocks.ActiveFree.ToString(), ObjectStatus.FREE },
                { LandObjectBlocks.ActiveToBeTraded.ToString(), ObjectStatus.TOBETRADED },
                { LandObjectBlocks.SaledComplete.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledProt.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledAgr.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.DeletedDisqualified.ToString(), ObjectStatus.DELETED },
                { LandObjectBlocks.DeletedDoubled.ToString(), ObjectStatus.DELETED },
            };

            var tbLandObjects = new TbLandObjects();
            var rows = tbLandObjects.Select(new FieldAlias[] { tbLandObjects.flId, tbLandObjects.flRevisionId, tbLandObjects.flBlock, tbLandObjects.flDescription, tbLandObjects.flName, tbLandObjects.flLandArea, tbLandObjects.flWKT }, qe);

            var objects = rows.AsEnumerable().Select(r => {
                return new ObjectModelResult(
                    tbLandObjects.flId.GetRowVal(r),
                    tbLandObjects.flRevisionId.GetRowVal(r),
                    ObjectType.LANDOBJECT,
                    Statuses[tbLandObjects.flBlock.GetRowVal(r)],
                    tbLandObjects.flName.GetRowVal(r),
                    tbLandObjects.flDescription.GetRowVal(r),
                    tbLandObjects.flLandArea.GetRowVal(r),
                    tbLandObjects.flWKT.GetRowVal(r)
                );
            }).ToList();

            return objects;

        }
        private static List<ObjectModelResult> GetHuntingObjects(IQueryExecuterSu qe)
        {

            Dictionary<string, ObjectStatus> Statuses = new Dictionary<string, ObjectStatus>() {
                { LandObjectBlocks.ActiveFree.ToString(), ObjectStatus.FREE },
                { LandObjectBlocks.ActiveToBeTraded.ToString(), ObjectStatus.TOBETRADED },
                { LandObjectBlocks.SaledComplete.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledProt.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledAgr.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.DeletedDisqualified.ToString(), ObjectStatus.DELETED },
                { LandObjectBlocks.DeletedDoubled.ToString(), ObjectStatus.DELETED },
            };

            var tbObjects = new HuntingSource.QueryTables.Object.TbObjects();
            var rows = tbObjects.Select(new FieldAlias[] { tbObjects.flId, tbObjects.flRevisionId, tbObjects.flBlock, tbObjects.flDescription, tbObjects.flName, tbObjects.flHuntArea, tbObjects.flWKT }, qe);

            var objects = rows.AsEnumerable().Select(r => {
                return new ObjectModelResult(
                    tbObjects.flId.GetRowVal(r),
                    tbObjects.flRevisionId.GetRowVal(r),
                    ObjectType.HUNTINGOBJECT,
                    Statuses[tbObjects.flBlock.GetRowVal(r)],
                    tbObjects.flName.GetRowVal(r),
                    tbObjects.flDescription.GetRowVal(r),
                    tbObjects.flHuntArea.GetRowVal(r),
                    tbObjects.flWKT.GetRowVal(r)
                );
            }).ToList();

            return objects;

        }
        private static List<ObjectModelResult> GetFishingObjects(IQueryExecuterSu qe)
        {

            Dictionary<string, ObjectStatus> Statuses = new Dictionary<string, ObjectStatus>() {
                { LandObjectBlocks.ActiveFree.ToString(), ObjectStatus.FREE },
                { LandObjectBlocks.ActiveToBeTraded.ToString(), ObjectStatus.TOBETRADED },
                { LandObjectBlocks.SaledComplete.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledProt.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.SaledAgr.ToString(), ObjectStatus.SALED },
                { LandObjectBlocks.DeletedDisqualified.ToString(), ObjectStatus.DELETED },
                { LandObjectBlocks.DeletedDoubled.ToString(), ObjectStatus.DELETED },
            };

            var tbObjects = new FishingSource.QueryTables.Object.TbObjects();
            var rows = tbObjects.Select(new FieldAlias[] { tbObjects.flId, tbObjects.flRevisionId, tbObjects.flBlock, tbObjects.flDescription, tbObjects.flName, tbObjects.flArea, tbObjects.flWKT }, qe);

            var objects = rows.AsEnumerable().Select(r => {
                return new ObjectModelResult(
                    tbObjects.flId.GetRowVal(r),
                    tbObjects.flRevisionId.GetRowVal(r),
                    ObjectType.FISHINGOBJECT,
                    Statuses[tbObjects.flBlock.GetRowVal(r)],
                    tbObjects.flName.GetRowVal(r),
                    tbObjects.flDescription.GetRowVal(r),
                    tbObjects.flArea.GetRowVal(r),
                    tbObjects.flWKT.GetRowVal(r)
                );
            }).ToList();

            return objects;

        }

    
        private static List<ObjectModelResult> GetForestObjects(IQueryExecuterSu qe)
        {

            Dictionary<string, ObjectStatus> Statuses = new Dictionary<string, ObjectStatus>() {
                { ForestSource.References.Object.ForestryPieceBlocks.ActiveFree.ToString(), ObjectStatus.FREE },
                { ForestSource.References.Object.ForestryPieceBlocks.ActiveConnected.ToString(), ObjectStatus.FREE },
                { ForestSource.References.Object.ForestryPieceBlocks.ActiveToBeTraded.ToString(), ObjectStatus.TOBETRADED },
                { ForestSource.References.Object.ForestryPieceBlocks.SaledComplete.ToString(), ObjectStatus.SALED },
                { ForestSource.References.Object.ForestryPieceBlocks.SaledProt.ToString(), ObjectStatus.SALED },
                { ForestSource.References.Object.ForestryPieceBlocks.SaledAgr.ToString(), ObjectStatus.SALED },
                { ForestSource.References.Object.ForestryPieceBlocks.DeletedDisqualified.ToString(), ObjectStatus.DELETED },
                { ForestSource.References.Object.ForestryPieceBlocks.DeletedDoubled.ToString(), ObjectStatus.DELETED },
            };

            var join = new ForestSource.QueryTables.Object.TbForestryPieces()
                .GetForestryQuarterJoin(out var tbForestries, out var tbQuarters);
            var rows = join.Select(t => new FieldAlias[] { t.R.flId, t.R.flRevisionId, t.R.flBlock, t.R.flDescription, t.L.L.flName, t.L.R.flNumber.ToAlias("flQuarterNumber"), t.R.flNumber.ToAlias("flForestryPieceNumber"), t.R.flArea, t.R.flWKT }, qe);

            var objects = rows.AsEnumerable().Select(r => {
                return new ObjectModelResult(
                    r.GetVal(t => t.R.flId),
                    r.GetVal(t => t.R.flRevisionId),
                    ObjectType.FORESTOBJECT,
                    Statuses[r.GetVal(t => t.R.flBlock)],
                    $"{r.GetVal(t => t.L.L.flName)}, квартал {r.GetVal(t => t.L.R.flNumber, "flQuarterNumber")}, выдел {r.GetVal(t => t.R.flNumber, "flForestryPieceNumber")}",
                    r.GetVal(t => t.R.flDescription),
                    r.GetVal(t => t.R.flArea),
                    r.GetVal(t => t.R.flWKT)
                );
            }).ToList();

            return objects;

        }

    }
}
