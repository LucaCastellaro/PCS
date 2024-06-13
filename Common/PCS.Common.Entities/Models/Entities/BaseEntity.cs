using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PCS.Common.Entities.Models.Entities;

[Serializable]
[BsonIgnoreExtraElements]
public abstract record BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public string Id { get; set; }
}
