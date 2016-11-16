using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PolygonTester : MonoBehaviour
{
    void Start()
    {
        TestPoly2Tri();

        /*
        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(0,50),
            new Vector2(50,50),
            new Vector2(50,100),
            new Vector2(0,100),
            new Vector2(0,150),
            new Vector2(150,150),
            new Vector2(150,100),
            new Vector2(100,100),
            new Vector2(100,50),
            new Vector2(150,50),
            new Vector2(150,0),
        };

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        */
          }

    void TestPoly2Tri()
    {
        Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
        poly.outside = new List<Vector3>() {
            //new Vector3(20,0,20),
            //new Vector3(20,0,30),
            //new Vector3(30,0,30),
            //new Vector3(30,0,20),

            new Vector3(20,0,20),
            new Vector3(30,0,20),  
            new Vector3(30,0,30),
            new Vector3(20,0,30),
            
       
           
          
            

            /*
            new Vector3(0,0,0),
            new Vector3(0,50,50),
            new Vector3(50,50,50),
            new Vector3(50,100,100),
            new Vector3(0,100,100),
            new Vector3(0,150,150),
            new Vector3(150,150,150),
            new Vector3(150,100,100),
            new Vector3(100,100,100),
            new Vector3(100,50,50),
            new Vector3(150,50,50),
            new Vector3(150,0,0),
            */
        };

        poly.CalcPlaneNormal(Vector3.up);
        /*
        poly.holes.Add(new List<Vector3>() {
            new Vector3(60,110,110),
            new Vector3(90,110,110),
            new Vector3(90,140,140),
            new Vector3(60,140,140),
        });
        */

        // Set up game object with mesh;
        Poly2Mesh.CreateGameObject(poly);
    }
}