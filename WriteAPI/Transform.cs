using System.Numerics;

namespace WriteAPI
{
	public class Transform
	{
		public Vector3 position = Vector3.Zero;
		public Quaternion rotation = Quaternion.Identity;

		public Transform()
		{
		}

		public Transform(Vector3 pos, Quaternion rot)
		{
			position = pos;
			rotation = rot;
		}

		public override string ToString()
		{
			return $"Transform: {position} {rotation}";
		}
	}
}