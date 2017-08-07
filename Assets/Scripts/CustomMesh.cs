//This class that contains methods
//that can be applied to any mesh
using UnityEngine;
using System.Collections;

public class CustomMesh : MonoBehaviour 
{
	private Vector3 startPoint;
	private Vector3 directionVector;
	//Determine if a world point exists in the 
	//mesh
	public bool ContainsWorldPoint(ref Vector3 worldPoint, ref int[]indices, ref Vector3[]vertices) 
	{
		bool containsWorldPoint = false;
		//First do a broad phase contains point detection
		//using AABB contains point method
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		AABB aabb = GetComponent<AABB>();
		Vector3 center = Vector3.zero;
		aabb.SetCenter(ref center);
		Vector3 extents = meshRenderer.bounds.extents;
		aabb.SetHalfExtent(ref extents);

		directionVector = center - worldPoint;

		if(aabb.ContainsPoint(ref worldPoint))
		{			
			startPoint = worldPoint;

			int numberOfTriangles = indices.Length/3;
			int index = 0;
			int numberOfRayCastHits = 0;
			bool isWorldPointSameAsMeshVertex = false;
			for(int i = 0; i < numberOfTriangles; i++)
			{
				int a = indices[index];
				index++;
				int b = indices[index];
				index++;
				int c = indices[index];

				Vector3 aVector = vertices[a];
				Vector3 bVector = vertices[b];
				Vector3 cVector = vertices[c];

				//Check to see if the world point is same as one of the mesh
				//vertices
				//If it is then, exit out and simply return true
				if(worldPoint == aVector
				   || worldPoint == bVector
				   || worldPoint == cVector)
				{
					isWorldPointSameAsMeshVertex = true;
					break;
				}

				bool hit = CustomPhysics.RayCastHit(ref startPoint, ref directionVector,  ref aVector, ref bVector, ref cVector);
				if(hit)
				{
					numberOfRayCastHits++;
				}
			}

			//If it hits once or none, then we know the point is inside the mesh
			//Also if the point is the mesh vertex, we consider that inside the mesh as
			//well
			if(numberOfRayCastHits == 0
			   || numberOfRayCastHits == 1
			   || isWorldPointSameAsMeshVertex)
			{
				containsWorldPoint = true;
			}
			//If it hits more than once, then we know the point is outside the mesh
			else if(numberOfRayCastHits > 1)
			{
				containsWorldPoint = false;
			}
		}
		Debug.Log("contains point" + worldPoint + " " + containsWorldPoint);
		return containsWorldPoint;
	}
	
	void Update()
	{
		Debug.DrawRay(startPoint, directionVector, Color.red);
	}
}
