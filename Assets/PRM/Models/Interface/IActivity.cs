using UnityEngine;
using System.Collections;

public interface IActivity
{

    bool isActivityReady { get; }
    void StartActivity();
    void EndActivity(bool force = false);

}