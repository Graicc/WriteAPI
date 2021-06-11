using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Numerics;

namespace WriteAPI
{
	public class QuaternionContractResolver : DefaultContractResolver
	{
		public static QuaternionContractResolver Instance { get; } = new QuaternionContractResolver();

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);
			if (typeof(Quaternion).IsAssignableFrom(member.DeclaringType) && member.Name == nameof(Quaternion.IsIdentity))
			{
				property.Ignored = true;
			}
			return property;
		}
	}
}
