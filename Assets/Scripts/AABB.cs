using UnityEngine;
using System.Collections;

public class AABB : MonoBehaviour 
{
	Vector3 center;
	Vector3 halfExtent;
	// Use this for initialization
	void Start () 
	{
		halfExtent = Vector3.zero;
	}

	public void SetCenter(ref Vector3 newCenter)
	{
		center = newCenter;
	}

	public void SetHalfExtent(ref Vector3 newHalfExtent)
	{
		halfExtent = newHalfExtent;
	}

	public bool ContainsPoint(ref Vector3 point)
	{
		float maxX = center.x + halfExtent.x;
		float minX = center.x - halfExtent.x;
		float maxY = center.y + halfExtent.y;
		float minY = center.y - halfExtent.y;
		float maxZ = center.z + halfExtent.z;
		float minZ = center.z - halfExtent.z;

		if(point.x <= maxX && point.x >= minX
		   && point.y <= maxY && point.y >= minY
		   && point.z <= maxZ && point.z >= minZ)
		{
			return true;
		}
		return false;
	}
}
