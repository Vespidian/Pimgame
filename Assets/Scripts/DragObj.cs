﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObj : MonoBehaviour
{

	Quaternion rotation;

	Rigidbody objRb;
	Vector3 dragObjRel;
	Vector3 playerDirection;
	Vector2 mousePos;
	Vector2 smoothV;
	Vector2 objRotation;
	Quaternion rot;
	Quaternion startRot;
	public float dragSpeed = 20;
	private bool stopRotation = false;
	private bool freezeObject = false;
	private bool toggleHold = false;

	private Character_Controller thePlayer;
	private Weapons playerTools;
	private CamMouseLook cameraVars; 

    // Start is called before the first frame update
    void Start()
    {
    	playerTools = GameObject.Find("Player").GetComponent<Weapons>();
    	thePlayer = GameObject.Find("Player").GetComponent<Character_Controller>();
    	cameraVars = GameObject.Find("Camera").GetComponent<CamMouseLook>();

        objRb = this.gameObject.GetComponent<Rigidbody>();
    }

    void OnMouseDown() {
    	if(playerTools.weapon == 1){
	    	stopRotation = true;
	    	freezeObject = false;
    		toggleHold = true;

    		startRot = transform.rotation;
    		//Transforms this objects position from worldspace to the camera's localspace
    		dragObjRel = GameObject.Find("Camera").transform.InverseTransformPoint(this.transform.position);
		   	thePlayer.scrollDist = dragObjRel.z;
		   	thePlayer.destPoint.transform.localPosition = new Vector3(0, 0, dragObjRel.z);

		   	/*thePlayer.scrollDist = thePlayer.hit.distance;
		   	thePlayer.destPoint.transform.localPosition = new Vector3(0, 0, thePlayer.hit.distance);*/
    	}
    	if(playerTools.weapon == 2){
    		ShootObject();
    	}
    }
    void OnMouseUp() {
    	if(playerTools.weapon == 1){
	    	stopRotation = false;
	    }
	    toggleHold = false;
	    cameraVars.mouseMove = true;
    }
    void OnMouseOver() {
    	if(Input.GetMouseButtonDown(1)){
    		freezeObject = true;
    	}
    }

    void FixedUpdate(){
    	if((playerTools.weapon == 1)){
	    	if((stopRotation == true) && (freezeObject == false)){
	    		HoldObject();

	    	}else if((stopRotation == false) && (freezeObject == false)){
		   		DynamicObject();

		    }else if(freezeObject == true){
	    		FreezeObject();
	    	}
		    if(Input.GetKeyDown(KeyCode.F)){
		   		DynamicObject();
		   	}

			//LineCasting();

		   	scrollDestination();
			if((Input.GetKey(KeyCode.E)) && (toggleHold == true)){
				cameraVars.mouseMove = false;
				mousePos = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
				objRotation += mousePos;

				rot = Quaternion.Euler(startRot.x + objRotation.x, startRot.y + -objRotation.y, startRot.z);
				transform.RotateAround(transform.position, GameObject.Find("Camera").transform.right, mousePos.y);
				transform.RotateAround(transform.position, GameObject.Find("Camera").transform.up, -mousePos.x);
			}else if(Input.GetKeyUp(KeyCode.E))
			{
				cameraVars.mouseMove = true;
			}
			if(Input.GetMouseButton(0)){

         	}
		}
		if(playerTools.weapon == 2){

		}
		if(playerTools.weapon == 4){
			if(Input.GetMouseButton(0)){
				toggleHold = true;
			   	scrollDestination();
				HoldObject();
			}
			if(Input.GetMouseButtonUp(0)){
				toggleHold = false;
				DynamicObject();
			}
		}
    }

    void OnCollisionEnter(Collision col){
    	if(col.gameObject.tag == "Trash"){
    		Destroy(this.gameObject);
    	}
    }

    void scrollDestination(){
    	if(toggleHold == true){
	    	if(Input.GetAxis("Mouse ScrollWheel") < 0){
			   	thePlayer.scrollDist -= thePlayer.scrollSpeed;
			}
		    if(Input.GetAxis("Mouse ScrollWheel") > 0){
			   	thePlayer.scrollDist += thePlayer.scrollSpeed;
			}
			thePlayer.scrollDist = Mathf.Clamp(thePlayer.scrollDist, thePlayer.scrollMin, thePlayer.scrollMax);
			thePlayer.destPoint.transform.localPosition = new Vector3(0, 0, thePlayer.scrollDist);
		}
    }

    void DynamicObject(){
    	objRb.useGravity = true;
		objRb.freezeRotation = false;
	  	objRb.isKinematic = false;

	  	stopRotation = false;
	  	freezeObject = false;
    }
    void HoldObject() {
    	objRb.useGravity = true;
		objRb.freezeRotation = true;
		objRb.isKinematic = false;
		objRb.velocity = (thePlayer.destPoint.position - this.transform.position) * dragSpeed;
		//objRb.velocity = ((thePlayer.destPoint.position + thePlayer.hit.point) - this.transform.position) * dragSpeed;
    }
    void FreezeObject() {
    	objRb.useGravity = false;
	    objRb.freezeRotation = true;
	    objRb.isKinematic = true;
    }
    void ShootObject(){
    	DynamicObject();
	    thePlayer.hit.rigidbody.AddForceAtPosition(GameObject.Find("Camera").transform.forward * thePlayer.pokeForce * 1000, thePlayer.hit.point);
    }
}