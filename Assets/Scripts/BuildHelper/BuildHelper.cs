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
            
            // AotPreBuilder.PreCloudBuild();
            
            //since addressable is handled by unity cloud, do nothing here


        }
        
        
    }
}