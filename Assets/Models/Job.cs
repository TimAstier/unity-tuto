using System;
using System.Collections;
using System.ComponentModel;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class Job {
  public Tile tile {
    get; protected set;
  }
  float jobTime;


  public string jobObjectType {
    get; protected set;
  }

  Action<Job> cbJobComplete;
  Action<Job> cbJobCancel;

  public Job(Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 1f) {
    this.tile = tile;
    this.jobObjectType = jobObjectType;
    this.cbJobComplete += cbJobComplete;
  }

  public void RegisterJobCompleteCallback(Action<Job> cb) {
    this.cbJobComplete += cb;
  }

  public void RegisterJobCancelCallback(Action<Job> cb) {
    this.cbJobComplete += cb;
  }

  public void UnregisterJobCompleteCallback(Action<Job> cb) {
    this.cbJobComplete -= cb;
  }

  public void UnregisterJobCancelCallback(Action<Job> cb) {
    this.cbJobComplete -= cb;
  }

  public void DoWork(float workTime) {
    jobTime -= workTime;

    if (jobTime < 0) {
      if (cbJobComplete != null) {
        cbJobComplete(this);
      }
    }
  }

  public void CancelJob() {
    if (cbJobCancel != null) {
      cbJobCancel(this);
    }
  }

}