﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

    //Public Vars
    //public float speed;

    //Private Vars
    private Vector3 mousePosition;
    //private Vector3 direction;
    //private float distanceFromObject;
    public new Camera camera;


    void FixedUpdate()
    {


        //Grab the current mouse position on the screen
        mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - camera.transform.position.z));
        Debug.Log("Mouse Position =" + mousePosition);

        //Rotates toward the mouse
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((mousePosition.y - transform.position.y), (mousePosition.x - transform.position.x)) * Mathf.Rad2Deg + 90);

        //Judge the distance from the object and the mouse
        //distanceFromObject = (Input.mousePosition - camera.WorldToScreenPoint(transform.position)).magnitude;

        //Move towards the mouse
        //rigidbody.AddForce(direction * speed * distanceFromObject * Time.deltaTime);

    }
}
