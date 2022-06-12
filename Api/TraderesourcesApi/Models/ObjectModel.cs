using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using JsonConstructorAttribute = System.Text.Json.Serialization.JsonConstructorAttribute;

namespace TraderesourcesApi.Models
{
    public class ObjectModelResponse
    {
        [JsonConstructor]
        public ObjectModelResponse(ObjectModelResult[] objectResult, ObjectModelResponseStatus responseStatus)
        {
            ObjectResult = objectResult;
            ResponseStatus = responseStatus;
        }

        public ObjectModelResult[] ObjectResult { get; }
        public ObjectModelResponseStatus ResponseStatus { get; }
    }

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ObjectModelResponseStatus
    {
        Ok = 0,
        Error = 1
    }

    public class ObjectModelResult
    {
        [JsonConstructor]
        public ObjectModelResult(int objectId, int objectRevisionId, ObjectType type, ObjectStatus status, string name, string info, decimal area, string wkt)
        {
            ObjectId = objectId;
            ObjectRevisionId = objectRevisionId;
            Type = type;
            Status = status;
            Name = name;
            Info = info;
            Area = area;
            WKT = wkt;
        }

        public int ObjectId { get; }
        public int ObjectRevisionId { get; }
        public ObjectType Type { get; }
        public ObjectStatus Status { get; }
        public string Name { get; }
        public string Info { get; }
        public decimal Area { get; }
        public string WKT { get; }
    }

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ObjectType
    {
        ALL,
        HYDROCARBON,
        TPI,
        LANDOBJECT,
        FISHINGOBJECT,
        HUNTINGOBJECT,
        FORESTOBJECT,
    }
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ObjectStatus
    {
        FREE,
        APPLICATIONTOTRADE,
        TRADEDECISION,
        TOBETRADED,
        SALED,
        REALIZED,
        DELETED 
    }

}
