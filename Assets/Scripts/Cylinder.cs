//This class is responsible for
//procedural creating a cylinder
//given the parameters of halfaxis,
//radius, and capResolution
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Cylinder : MonoBehaviour 
{
	public Vector3 halfAxis;
	public float radius;
	public int capResolution;

	private Vector3 initialHalfAxis;

	const int MIN_CAP_RESOLUTION = 3;
	const float MIN_RADIUS = 1.0f;
	const int NUMBER_OF_VERTICES_PER_TRIANGLE = 3;
	const float MAX_SCALE = 1000.0f;

	//Body drawing state
	//for drawing the triangles
	//of the faces of the cylinder's body
	enum BodyDrawingState
	{
		TopHalf,
		BottomHalf
	};

	//Drawing direction enum
	//for triangle indices
	//for the caps
	enum DrawingDirection
	{
		Clockwise,
		Counterclockwise
	};
	
	void Start() 
	{
		//Clamp the cap resolution
		//if it is lower than 3 since
		//a cylinder can't be generated
		//with less than 3 points on the cap
		if(capResolution < MIN_CAP_RESOLUTION)
		{
			capResolution = MIN_CAP_RESOLUTION;
		}
		//Clamp the radius so that
		//we don't have any 0 or negative
		//radius, which are invalid
		//for generating a cylinder
		if(radius < MIN_RADIUS)
		{
			radius = MIN_RADIUS;
		}
		if(halfAxis == Vector3.zero)
		{
			halfAxis = initialHalfAxis;
		}

		//Generate the cylinder first
		//with it facing towards up
		initialHalfAxis = Vector3.up;
		Mesh mesh = GenerateCylinder();

		//Configure the cylinder
		Setup(ref mesh);

		//Check to see if the given world point exists in the cylinder
		WorldPointInside(ref mesh);
	}

	//Check to see if a given world point exists in the cylinder
	bool WorldPointInside(ref Mesh mesh)
	{
		CustomMesh customMesh = GetComponent<CustomMesh>();
		Vector3 worldPoint = new Vector3(0.0f, 1.0f, 0.0f);
		int[] indices = mesh.triangles;
		Vector3[]vertices = mesh.vertices;
		return customMesh.ContainsWorldPoint(ref worldPoint, ref indices, ref vertices);
	}

	//Configures the cylinder
	void Setup(ref Mesh mesh)
	{
		mesh.name = "Cylinder";
		//Scale then rotate in order to get the correct appearance
		//Multiply the local scale by the half axis to get the correct size
		float halfAxisXScale = Mathf.Clamp(Mathf.Abs(halfAxis.x/2.0f), 1.0f, MAX_SCALE);
		float halfAxisYScale = Mathf.Clamp(Mathf.Abs(halfAxis.y/2.0f), 1.0f, MAX_SCALE);
		float halfAxisZScale = Mathf.Clamp(Mathf.Abs(halfAxis.z/2.0f), 1.0f, MAX_SCALE);
		transform.localScale = new Vector3(halfAxisXScale, halfAxisYScale, halfAxisZScale);
		//Rotate the vertices from the initial half axis vector to the half axis vector
		//to represent the actual direction
		transform.rotation = Quaternion.FromToRotation(initialHalfAxis, halfAxis);
		//You must call this in order for the normals to be generated
		//based on the triangle indices and vertices
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	//Construct a cyclinder programmatically
	//given the following parameters:
	//halfAxis for the length and direction of cyclinder
	//capResolution for the number of points on the diameter
	//of the cap of the cylinder
	Mesh GenerateCylinder() 
	{
		Mesh mesh = new Mesh();
		//The number of vertices is determined by the 2 caps resolution
		//plus the body which is 2 caps resolution plus the center points of the 2 caps
		//plus a duplicate of the first body index vertices
		int numberOfVertices = (4 * capResolution) + 4;
		Vector3[] vertices = new Vector3[numberOfVertices];
		//Every vertice has a UV coordinate
		Vector2[] uvs = new Vector2[numberOfVertices];
		
		//Calculate the number of triangles
		//for each cap, there will be capResolution triangles
		//for the body of the cyclinder, it will be capResolution triangles * 2
		//therefore the formula is: 
		//(2 * capResolution) + (2 * capResolution)
		//which simplifies to 4 * capResolution
		//Now since they are indices, it would be
		//the number of triangles * 3
		//which simiplies this even further to be:
		//12 * capResolution
		int numberOfIndices = 12 * capResolution;
		int[] indices = new int[numberOfIndices];
		
		//Generate the vertices for the cyclinder
		GenerateVertices(ref vertices);
		//Create triangle indices for the cyclinder to make the faces
		CreateTriangleIndices(ref indices);
		//Create the UV coordinates
		CreateUVCoordinates(ref uvs);

		//Assign the vertices, indices, and uvs
		//Note: Normals are automatically generated by hardware
		//based on the direction produced by the order
		//of the triangle indices
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		
		return mesh;
	}        
	
	//Generate vertices for the cyclinder
	void GenerateVertices(ref Vector3[]vertices)
	{
		//First start at the center of the top cap of the cyclinder
		//which is simply just the initialHalfAxis
		vertices[0] = new Vector3(initialHalfAxis.x, 
		                          initialHalfAxis.y, 
		                          initialHalfAxis.z);
		//Then use the radius to draw the first point on the cyclinder
		//cap outline/circumference

		vertices[1] = new Vector3(vertices[0].x + radius,
		                          vertices[0].y,
		                          vertices[1].z);

		//Now rotate that first point on the cyclinder's cap's outline/circumference
		//capResolution - 1 times by rotating around the half axis and degrees
		int numberOfRotations = capResolution - 1;
		float degreesToRotate = 360.0f/((float)capResolution);
		Vector3 previousVertex = vertices[1];
		int startingIndex = 2;
		int bodyIndex = (capResolution * 2) + startingIndex;
		vertices[bodyIndex] = vertices[1];
		//Duplicate the first body vertex for proper UV layout
		vertices[bodyIndex + capResolution] = vertices[1];
		bodyIndex++;
		for(int i = 0; i < numberOfRotations; i++)
		{
			vertices[startingIndex] = Quaternion.AngleAxis(degreesToRotate, initialHalfAxis) * previousVertex;
			//Duplicate the vertex of the top cap's circumference that was just created
			//to represent the top part of the body
			vertices[bodyIndex] = vertices[startingIndex];
			previousVertex = vertices[startingIndex];
			startingIndex++;
			bodyIndex++;
		}
		//Offset bodyIndex by 1 since the first body index
		//on the bottom was duplicated and takes the index
		//of what the first bottom body index would be
		bodyIndex++;

		bool didDuplicateFirstBodyIndex = false;
		//Then draw the bottom cap of the cyclinder
		for(int j = 0; j < capResolution + 1; j++)
		{
			Vector3 aboveCapVertex = vertices[j];
			int bottomCapVertexIndex = capResolution + 1 + j;
			vertices[bottomCapVertexIndex] = new Vector3(aboveCapVertex.x - (initialHalfAxis.x * 2),
			                                              aboveCapVertex.y - (initialHalfAxis.y * 2),
			                                              aboveCapVertex.z - (initialHalfAxis.z * 2));
			if(j > 0)
			{
				//Duplicate the vertex of the bottom cap's circumference that was just created
				//to represent the bottom part of the body
				vertices[bodyIndex] = vertices[bottomCapVertexIndex];
				//Duplicate the first body index vertex on the bottom
				if(!didDuplicateFirstBodyIndex)
				{
					vertices[bodyIndex + capResolution] = vertices[bottomCapVertexIndex];
					didDuplicateFirstBodyIndex = true;
				}
				bodyIndex++;
			}
		}
	}

	//Create triangle indices
	void CreateTriangleIndices(ref int[]indices)
	{
		//Do the top cap first
		CreateTriangleIndicesForCap(0, 1, 2, 0, ref indices, DrawingDirection.Clockwise);
		//Then the bottom cap
		int bottomCapStartingIndex = capResolution * 3;
		int bottomCapStartingPoint = capResolution + 1;
		CreateTriangleIndicesForCap(bottomCapStartingPoint, bottomCapStartingPoint + 1, bottomCapStartingPoint + 2, bottomCapStartingIndex, ref indices, DrawingDirection.Counterclockwise);
		int bodyStaringIndex = capResolution * 6;
		int bodyStartingPoint = (capResolution * 2) + 2;
		//Finally the body
		CreateTriangleIndicesForBody(bodyStartingPoint, bodyStaringIndex, ref indices);
	}

	void CreateTriangleIndicesForBody(int startingPoint, int startingIndex, ref int[]indices)
	{
		int numberOfIndicesGenerated = 0;
		int totalNumberOfIndicesToBeGenerated = capResolution * 6;
		int point1 = startingPoint;
		int point2 = 0;
		int point3 = 0;
		BodyDrawingState bodyDrawingState = BodyDrawingState.TopHalf;
		
		while(numberOfIndicesGenerated < totalNumberOfIndicesToBeGenerated)
		{
			switch(bodyDrawingState)
			{
				case BodyDrawingState.TopHalf:
				{
					indices[startingIndex] = point1;
					startingIndex++;
					
					point2 = point1 + 1;
					
					point3 = point1 + capResolution + 1;

					indices[startingIndex] = point3;
					startingIndex++;
					indices[startingIndex] = point2;
					startingIndex++;
					
					point1++;
					
					numberOfIndicesGenerated += NUMBER_OF_VERTICES_PER_TRIANGLE;
					
					bodyDrawingState = BodyDrawingState.BottomHalf;
					break;
				}
				case BodyDrawingState.BottomHalf:
				{
					indices[startingIndex] = point1;

					startingIndex++;
					
					point2 = point3 + 1;
										
					point3 = point2 - 1;

					indices[startingIndex] = point3;
					startingIndex++;
					indices[startingIndex] = point2;
					startingIndex++;
					
					numberOfIndicesGenerated += NUMBER_OF_VERTICES_PER_TRIANGLE;
					
					bodyDrawingState = BodyDrawingState.TopHalf;
					break;
				}
			}
		}
	}
	
	void CreateTriangleIndicesForCap(int point1, int point2, int point3, int startingIndex, ref int[]indices, DrawingDirection drawingDirection)
	{
		int originalPoint2 = point2;
		for(int i = 0; i < capResolution; i++)
		{
			//If final triangle of cap
			if(i == capResolution - 1)
			{
				point3 = originalPoint2;
			}
			indices[startingIndex] = point1;
			startingIndex++;
			switch(drawingDirection)
			{
				case DrawingDirection.Clockwise:
				{
					indices[startingIndex] = point2;
					startingIndex++;
					point2++;
					indices[startingIndex] = point3;
					startingIndex++;
					point3++;
					break;
				}
				case DrawingDirection.Counterclockwise:
				{
					indices[startingIndex] = point3;
					startingIndex++;
					point3++;
					indices[startingIndex] = point2;
					startingIndex++;
					point2++;
					break;
				}
			}

		}
	}
	
	void CreateUVCoordinates(ref Vector2[]uvs)
	{
		//Create uv coordinates for top cap
		int startingIndex = 0;
		int endingIndex = capResolution + 1;
		CreateUVCoordinatesForCap(ref uvs, startingIndex, endingIndex);
		//Create uv coordinates for bottom cap
		startingIndex = endingIndex;
		endingIndex = (capResolution * 2) + 2;
		CreateUVCoordinatesForCap(ref uvs, startingIndex, endingIndex);
		//Create uv coordinates for body
		startingIndex = endingIndex + 1;
		CreateUVCoordinatesForBody(ref uvs);
	}

	void CreateUVCoordinatesForCap(ref Vector2[]uvs, int startingIndex, int endingIndex)
	{
		uvs[startingIndex] = new Vector2(0.5f, 0.5f);
		float angle = (360.0f/capResolution) * Mathf.PI/180.0f;
		float cumulativeAngle = angle;
		for(int i = startingIndex + 1; i < endingIndex; i++)
		{
			//Since the range for sin, cos, and tan is from [-1,1],
			//then the range magnitude is 2.  To get the coordinates
			//into texture coordinate space which range from [0,1],
			//use cumulative angle/2 to get into the range of [0,1]
			//and then add the center of the texture coordinate space
			//which is 0.5, 0.5
			float x = (Mathf.Cos(cumulativeAngle)/2) + 0.5f;
			float y = (Mathf.Sin(cumulativeAngle)/2) + 0.5f;
			uvs[i] = new Vector2(x,y);
			cumulativeAngle += angle;
		}
	}

	//Create UV coordinates for cylinder body
	void CreateUVCoordinatesForBody(ref Vector2[]uvs)
	{
		//The x percentage of texture to be 
		//exactly 1/capResolution because there is a 
		//duplicate body vertex point at the wrapping point
		float xPercentageOfTexture = (1.0f/((float)capResolution)) * 2.0f;
		float xPercentageOfTextureCounter = 0.0f;
		int bodyIndex = (capResolution * 2) + 2;
		//Create the UV coordinates for the top part of the body
		for(int i = 0; i < capResolution + 1; i++)
		{
			uvs[bodyIndex] = new Vector2(xPercentageOfTextureCounter, 1.0f);
			bodyIndex++;
			xPercentageOfTextureCounter += xPercentageOfTexture;
		}

		xPercentageOfTextureCounter = 0;
		//Create the UV coordinates for the bottom part of the body
		for(int j = 0; j < capResolution + 1; j++)
		{
			uvs[bodyIndex] = new Vector2(xPercentageOfTextureCounter, 0.0f);
			bodyIndex++;
			xPercentageOfTextureCounter += xPercentageOfTexture;
		}
	}
}