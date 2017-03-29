using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace WebCall.Interfaces
{
    public interface IJsonSerializer : ISerializer, IDeserializer
    {
        
    }
}