using Siccity.GLTFUtility;
using UnityEngine;

namespace Komodo.AssetImport 
{
    public class MDSiccityGLTFLoader : ModelDownloaderAndLoader
    {
        public override void LoadLocalFile(string localFilename, System.Action<GameObject> callback)
        {
            if (callback == null)
            {
                throw new System.Exception("callback was null");
            }

            callback(Importer.LoadFromFile(localFilename));
        }
    }
}

//https://cdn.jsdelivr.net/ghparseccentric/komodo-test-models/pyrite-conversion-VMDScene001.glb
