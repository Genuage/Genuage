using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class create_cube : MonoBehaviour
{

	void Start()
	{
		MakeCube();
	}

	private void MakeCube()
	{
		Vector3[] vertices = {
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (1, 1, 0),
			new Vector3 (0, 1, 0),
			new Vector3 (0, 1, 1),
			new Vector3 (1, 1, 1),
			new Vector3 (1, 0, 1),
			new Vector3 (0, 0, 1),
		};

		int[] triangles = {
			0, 2, 1, 
			0, 3, 2,
			2, 3, 4,
			2, 4, 5,
			1, 2, 5, 
			1, 5, 6,
			0, 7, 4, 
			0, 4, 3,
			5, 4, 7,
			5, 7, 6,
			0, 6, 7, 
			0, 1, 6
		};

		Mesh mesh =new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().mesh=mesh;
	}
}