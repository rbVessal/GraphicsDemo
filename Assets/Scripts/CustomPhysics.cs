//This class contains physics
//that can be applied to any mesh
using UnityEngine;
using System.Collections;

public class CustomPhysics : MonoBehaviour 
{	
	public static bool RayCastHit(ref Vector3 startPoint, ref Vector3 endPoint, ref Vector3 aVector, ref Vector3 bVector, ref Vector3 cVector)
	{
		Vector3 ab = bVector - aVector;
		Vector3 ac = cVector - aVector;
		Vector3 ray = startPoint - endPoint;
		
		//Get the triangle normal
		Vector3 normal = Vector3.Cross(ab, ac);
		
		//Check to see if the ray is parallel to or
		//points away from the triangle
		//Remember dot product:
		//0 - perpendicular or in this case
		//the ray is parallel(90 degrees to the normal)
		// -1 - the ray is pointing in the opposite direction (more than 90 degrees to the normal)
		float angleBetweenRayAndNormal = Vector2.Dot(ray, normal);
		if(angleBetweenRayAndNormal <= 0.0f)
		{
			return false;
		}
		
		//Compute the intersection point
		Vector3 intersectionVector = startPoint - aVector;
		float intersectionVectorAngle = Vector3.Dot(intersectionVector, normal);
		if(intersectionVectorAngle < 0.0f)
		{
			return false;
		}
		
		//Compute barycentric coordinate components and test if within bounds
		//v, w are barycentric coordinates
		Vector3 e = Vector3.Cross(ray, intersectionVector);
		float v = Vector3.Dot(ac, e);
		if(v < 0.0f
		   || v > angleBetweenRayAndNormal)
		{
			return false;
		}
		
		float w = - Vector3.Dot(ab, e);
		if(w < 0.0f
		   || v + w > angleBetweenRayAndNormal)
		{
			return false;
		}
		
		return true;
	}
}
