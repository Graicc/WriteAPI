using System.Numerics;

namespace WriteAPI
{
	public class CameraTransform
	{
		public Vector3 position = Vector3.Zero;
		public Quaternion rotation = Quaternion.Identity;

		public CameraTransform() { }

		public CameraTransform(Vector3 pos, Quaternion rot)
		{
			position = pos;
			rotation = rot;
		}

		public override string ToString()
		{
			return $"CameraTransform: {position} {rotation}";
		}
	}
}
