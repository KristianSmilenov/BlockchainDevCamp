using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Blockchain.Serializers
{
    public class TransactionDataHashReseolver : DefaultContractResolver
    {
        public new static readonly TransactionDataHashReseolver Instance = new TransactionDataHashReseolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            property.ShouldSerialize = (i) => (property.PropertyName.ToLower() != "transactionhashhex");

            return property;
        }
    }
}
