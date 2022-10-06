using System;
using Ludiq;
using UnityEditor;
using UnityEngine;

namespace BuildHelper
{
    public class BuildHelper
    {
        // [MenuItem("Tools/BuildBoltsAotNow")]
        public static void preExport()
        {
            Debug.Log($"test preExport here");
            //build Bolts AOT
            try
            {
                AotPreBuilder.GenerateLinker();
                AotPreBuilder.GenerateStubScript();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                AotPreBuilder.DeleteLinker();
                AotPreBuilder.DeleteStubScript();
            }
            //since addressable is handled by unity cloud, do nothing here
            
            
        }
        
        
    }
}