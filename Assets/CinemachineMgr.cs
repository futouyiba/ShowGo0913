using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace ET
{
    public class CinemachineMgr : MonoBehaviour
    {
        public List<GameObject> VCamLvlRoots;

        /// <summary>
        /// because it's already configs right now, we don't delete this list, to avoid serialization issues.
        /// </summary>
        public List<PlayableAsset> PlayableAssets;

        public List<PlayableDirector> PlayableDirectors;

        public List<float> PlayableSpeeds;


        // Start is called before the first frame update
        void Awake()
        {
            foreach (var camLvlRoot in VCamLvlRoots)
            {
                camLvlRoot.SetActive(false);
            }

            // Time.timeScale = 1.5f; // move to timeline play speed.
        }

        public void PlayAssetByLevel(int lvl, bool isSkip=false)
        {
            var speed = PlayableSpeeds[lvl];
            this.PlayDirectorByLevelBySpeed(lvl, speed, isSkip);

            // Debug.Log($"play asset : level {lvl}");
            // PlayableDirectors.ForEach(director =>
            // {
            //     director.Stop();
            // });
            // foreach (var camLvlRoot in VCamLvlRoots)
            // {
            //     camLvlRoot.SetActive(false);
            // }
            //
            // VCamLvlRoots[lvl].SetActive(true);
            // PlayableDirectors[lvl].Play();
        }

        /// <summary>
        /// it cannot be config within unity event for it has 2 parameters. 
        /// </summary>
        /// <param name="lvl"></param>
        /// <param name="speed"></param>
        /// <param name="isSkip"></param>
        protected void PlayDirectorByLevelBySpeed(int lvl, float speed, bool isSkip)
        {
            Debug.Log($"play asset : level {lvl}");
            PlayableDirectors.ForEach(director =>
            {
                director.Stop();
            });
            
            foreach (var camLvlRoot in VCamLvlRoots)
            {
                camLvlRoot.SetActive(false);
            } 

            VCamLvlRoots[lvl].SetActive(true);
            var director = PlayableDirectors[lvl];
            director.RebuildGraph();
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
            director.Play();
            if (isSkip)
            {
                director.time = director.duration;
            }
        }
    }
}