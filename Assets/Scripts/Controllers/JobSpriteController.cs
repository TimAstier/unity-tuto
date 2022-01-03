using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

  FurnitureSpriteController fsc;
  Dictionary<Job, GameObject> jobGameObjectMap;

  void Start() {
    jobGameObjectMap = new Dictionary<Job, GameObject>();
    fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();
    GameEvents.current.onJobCreated += OnJobCreated;
  }

  private void OnDestroy() {
    GameEvents.current.onJobCreated -= OnJobCreated;
  }

  void OnJobCreated(Job job) {
    GameObject job_go = new GameObject();

    if (jobGameObjectMap.ContainsKey(job)) {
      return;
    }

    if (jobGameObjectMap.ContainsKey(job)) {
      Debug.LogError("OnJobCreated for a jobGO that already exists -- most likely a job being RE-QUEUED, as opposed to created.");
      return;
    }

    jobGameObjectMap.Add(job, job_go);

    job_go.name = "JOB_" + job.jobObjectType + "_" + job.tile.X + "_" + job.tile.Y;
    job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
    job_go.transform.SetParent(this.transform, true);

    SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
    sr.sprite = fsc.GetSpriteForFurniture(job.jobObjectType);
    sr.color = new Color(0.5f, 1f, 0.5f, 0.25f);
    sr.sortingLayerName = "Jobs";

    job.RegisterJobCompleteCallback(OnJobEnded);
    job.RegisterJobCancelCallback(OnJobEnded);
  }

  void OnJobEnded(Job job) {
    // This executes whether a job was COMPLETED or CANCELLED

    // FIXME: We can only do furniture-building jobs.

    GameObject job_go = jobGameObjectMap[job];

    job.UnregisterJobCompleteCallback(OnJobEnded);
    job.UnregisterJobCancelCallback(OnJobEnded);

    Destroy(job_go);

  }



}
