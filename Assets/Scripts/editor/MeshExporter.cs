using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public static class MeshExporter
{
    /// <summary>
    /// Creates and returns a mesh
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="uv"></param>
    /// <param name="triangles"></param>
    /// <returns></returns>
    public static Mesh CreateMesh(Vector3[] verts, Vector2[] uv, int[][] triangles)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.subMeshCount = triangles.GetLength(0);

        for (int i = 0; i < triangles.GetLength(0); i++)
            mesh.SetTriangles(triangles[i], i, false);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;

    }
    /// <summary>
    /// Returns a list of created Materials 
    /// </summary>
    /// <param name="numberOfMaterials"></param>
    /// <param name="shaderType"></param>
    /// <param name="materialName"></param>
    /// <returns></returns>
    public static Material[] CreateMaterial(int numberOfMaterials, string shaderType = "Diffuse", string materialName = "material_")
    {
        Material[] material = new Material[numberOfMaterials];
        for (int i = 0; i<material.Length; i++)
        {
            material[i] = new Material(Shader.Find(shaderType));
            material[i].name = materialName + i;
        }

        return material;
    }
    public static void MeshToFile(Mesh mesh, Material[] material, string defaultName)
    {
       string path = PlayerPrefs.GetString("BB_MeshExport");

       path = EditorUtility.SaveFilePanelInProject("Save Mesh", defaultName + ".obj", "obj",
                                           "Please enter a file name to save the mesh to", path);
        if (path.Length != 0)
        {
            PlayerPrefs.SetString("BB_MeshExport",path);

            string givenName = path.Remove(0, path.LastIndexOf('/')+1);
            givenName = givenName.Remove(givenName.Length - 4);

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(MeshToString(mesh, material, givenName));
            }
            using (StreamWriter sw = new StreamWriter(path.Remove(path.Length - 4) + ".mtl"))
            {
                sw.Write(MaterialToString( material));
            }

            AssetDatabase.Refresh();
        }
    }
    private static string MeshToString(Mesh mesh, Material[] material, string meshName)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("mtllib ").Append(meshName).Append(".mtl\n");
        sb.Append("o ").Append(meshName).Append("\n");
        foreach (Vector3 v in mesh.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in mesh.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            sb.Append("usemtl ").Append(material[i].name).Append("\n");

            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
            }
        }
        return sb.ToString();
    }
    private static string MaterialToString(Material[] material)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < material.Length; i++)
        {
            sb.Append("\n");
            sb.Append("newmtl ").Append(material[i].name).Append("\n");
            sb.Append("Ns 96.078431").Append("\n");
            sb.Append("Ka 1.000000 1.000000 1.000000").Append("\n");
            sb.Append("Kd 0.640000 0.640000 0.640000").Append("\n");
            sb.Append("Ks 0.500000 0.500000 0.500000").Append("\n");
            sb.Append("Ke 0.000000 0.000000 0.000000").Append("\n");
            sb.Append("Ni 1.000000").Append("\n");
            sb.Append("d 1.000000").Append("\n");
            sb.Append("illum 2").Append("\n");
        }
        return sb.ToString();
    }
}
