using UnityEngine;
using UnityEditor;
using System;

public class ImporterOverride : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        String name = importer.assetPath.ToLower();
        if (name.Substring(name.Length - 6, 6) == ".blend")
        {
            importer.importMaterials = false;
            importer.animationType = ModelImporterAnimationType.None;
        }
        else if (name.Substring(name.Length - 4, 4) == ".obj")
        {
            importer.importMaterials = false;
            importer.animationType = ModelImporterAnimationType.None;
        }
        else if (name.Substring(name.Length - 4, 4) == ".fbx")
        {
            importer.importMaterials = false;
        }
    

    }

    void OnPreprocessTexture()
    {
        if (assetPath.Contains("_nm"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.convertToNormalmap = true;
        }

    }
}