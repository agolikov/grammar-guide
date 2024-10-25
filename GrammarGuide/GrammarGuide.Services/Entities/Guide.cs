using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GrammarGuide.Services.Entities;

public class Guide
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string SourceLanguage { get; set; }
    public string DestinationLanguage { get; set; }
    public List<UnitGroup> UnitGroups { get; set; }
}