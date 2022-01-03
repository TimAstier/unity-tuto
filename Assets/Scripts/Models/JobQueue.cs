using System;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue {
  Queue<Job> jobQueue;

  public JobQueue() {
    jobQueue = new Queue<Job>();
  }

  public void Enqueue(Job j) {
    jobQueue.Enqueue(j);

    GameEvents.current.JobCreated(j);
  }

  public Job Dequeue() {
    if (jobQueue.Count == 0)
      return null;

    return jobQueue.Dequeue();
  }
}
