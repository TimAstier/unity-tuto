using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {
  FurnitureSpriteController fsc;
  Dictionary<Job, GameObject> jobGamObjectMap;


  // Start is called before the first frame update
  void Start() {
    jobGamObjectMap = new Dictionary<Job, GameObject>();
    fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
    WorldController.Instance.world.jobQueue.RegisterJobCreationCallback(OnJobCreated);
  }

  void OnJobCreated(Job job) {


    GameObject job_go = new GameObject();
    jobGamObjectMap.Add(job, job_go);

    job_go.name = "JOB_" + job.jobObjectType + "_" + job.tile.X + "_" + job.tile.Y;
    job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
    job_go.transform.SetParent(this.transform, true);

    SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
    sr.sprite = fsc.GetSpriteForFurniture(job.jobObjectType);
    sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);
    sr.sortingLayerName = "TileUI";

    job.RegisterJobCompleteCallback(OnJobEnded);
    job.RegisterJobCancelCallback(OnJobEnded);
  }

  void OnJobEnded(Job job) {
    GameObject job_go = jobGamObjectMap[job];
    job.UnregisterJobCancelCallback(OnJobEnded);
    job.UnregisterJobCompleteCallback(OnJobEnded);
    Destroy(job_go);
  }

}
