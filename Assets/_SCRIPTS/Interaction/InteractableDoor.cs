using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] private SceneList TargetScene;

    public void InteractDoor()
    {
        LevelManager.instance.LoadLevel(TargetScene);
    }

}
