using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GrammarGuide.Services.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Theme { get; set; }
    
    public int Balance { get; set; }
    
    public List<GuideProgress> Progresses { get; set; }
}