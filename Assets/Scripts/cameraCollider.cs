using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraCollider : MonoBehaviour {
    [HideInInspector]
    public List<CameraMovement.Ground> grounds;
    private void Awake()
    {
        grounds = new List<CameraMovement.Ground>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "ground")
        {
            Debug.Log("ADD ground");
            grounds.Add(new CameraMovement.Ground(col.gameObject));
        }
        else if (col.tag == "wall")
        {

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "ground")
        {
            for (int i = 0; i < grounds.Count; i++)
            {
                if (grounds[i].ownGObject == col.gameObject)
                {
                    Debug.Log("Remove ground");
                    grounds.RemoveAt(i);
                }
            }
        }
        else if (col.tag == "wall")
        {

        }
    }
}
