﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    Rigidbody draggedObjectRb;

	private Text weaponTextMain;
    private Text weaponTextPrev;
    private Text weaponTextNext;
	public int numOfWeapons = 3;
	private bool canChangeWeapon = true;

    private Text selectedWeapon;
    private Text selectedWeaponFunction;

	public int weapon = 1;

    private GameObject weldObj0;
    private GameObject weldObj1;
    private int weldObjectNum;
    private FixedJoint joint;

    private ConfigurableJoint destConfigJoint;
    private Quaternion objStartRotation;


    private int dupeObjectNum = 1;

    private GameObject dupeObject;
    private Quaternion dupeRotation;
    private GameObject previewObject;
    private bool previewVisible = false;
    private string previewName = "";

    //WEAPONS
    string weapon1 = "Physics Gun";
    string weapon2 = "Impulse Gun";
    string weapon3 = "Tool Gun";
    /*
    == List of tools in order starting with 1 ==
    
    Delete Tool
    Weld Tool
    Thruster Tool
    Wheel Tool
    Duplicator Tool
    Hinge Tool
    Gravity Toggle
    Spawn Balloon
    */

    [Header("Tool Status")]
    public bool deleteTool = true;
    public bool weldTool = true;
    public bool thrusterTool = true;
    public bool wheelTool = true;
    public bool duplicatorTool = true;
    public bool hingeTool = true;
    public bool gravityTool = true;
    public bool balloonTool = true;

    public int tool = 1;

    //SCRIPTS
    private CharController thePlayer;
    private DragObj draggedObjectScript;
    private CamMouseLook cameraVars;
    private PhysicsRestrictions objectRestrictions;
    private ToggleUI weaponButtons;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GameObject.Find("Player").GetComponent<CharController>();
        cameraVars = GameObject.Find("Camera").GetComponent<CamMouseLook>();
        weaponTextPrev = GameObject.Find("WeaponPrev").GetComponent<Text>();
        weaponTextMain = GameObject.Find("WeaponMain").GetComponent<Text>();
        weaponTextNext = GameObject.Find("WeaponNext").GetComponent<Text>();
        weaponButtons = GameObject.Find("Canvas").GetComponent<ToggleUI>();

        selectedWeapon = GameObject.Find("Weapon").GetComponent<Text>();
        selectedWeaponFunction = GameObject.Find("Function").GetComponent<Text>();

        destConfigJoint = cameraVars.gameObject.transform.Find("Destination").GetComponent<ConfigurableJoint>();

        ToolDescriptor();
        SetWeaponStatus();
    }

    // Update is called once per frame
    void Update()
    {
        //Scroll through weapons
	    if(canChangeWeapon == true && weaponButtons.AlwaysShown.activeSelf == true){
        	if(Input.GetAxisRaw("Mouse ScrollWheel") > 0){//-
	   			if(weapon != 1){
	   				weapon--;
	   			}else{
	   				weapon = numOfWeapons;
	   			}
                SpawnPreview("delete");
                CheckWeapon();
	   		}if(Input.GetAxisRaw("Mouse ScrollWheel") < 0){//+
	   			if(weapon != numOfWeapons){
	   				weapon++;
	   			}else{
	   				weapon = 1;
	   			}
                SpawnPreview("delete");
                CheckWeapon();
	   		}
        }
        if(cameraVars.mouseMove == true){
            if(weapon == 3){
                switch(tool){
                    case 3://Thruster Tool
                        SpawnPreview("thruster");
                        break;
                    case 4://Wheel Tool
                        SpawnPreview("wheel");
                        break;
                    default:
                        SpawnPreview("delete");
                        break;
                }
            }
            if(thePlayer.hit.collider != null){
                if(Input.GetMouseButtonDown(0)){//PRESSED LEFT MOUSE BUTTON
                    canChangeWeapon = false;
                    if(weapon == 1){
                        if(draggedObjectRb != null && objectRestrictions != null){
                            if(objectRestrictions.allowDragging == true){
                                objStartRotation = draggedObjectRb.gameObject.transform.rotation;
                                destConfigJoint.connectedBody = draggedObjectRb;
                                destConfigJoint.targetRotation = objStartRotation;

                            }
                        }
                    }else if(weapon == 2){
                        ImpulseObject();

                    }else if(weapon == 3){
                        if(tool == 1){
                            DeleteObject();
                        }else if(tool == 2){
                            WeldObjects("weld");
                        }else if(tool == 3){
                            SpawnThruster();
                        }else if(tool == 4){
                            SpawnWheel();
                        }else if(tool == 5){
                            DuplicateObject("left");
                        }else if(tool == 6){
                            SpawnHinge();
                        }else if(tool == 7){
                            ToggleGravity();
                        }else if(tool == 8){
                            SpawnBalloon();
                        }
                    }
                }


                if(Input.GetMouseButtonUp(0)){
                    canChangeWeapon = true;
                    if(weapon == 1){
                        destConfigJoint.connectedBody = null;
                    }
                }

                if(Input.GetMouseButtonDown(1)){//PRESSED RIGHT MOUSE BUTTON
                    if(weapon == 3){
                        if(tool == 2){
                            WeldObjects("remove");
                        }else if(tool == 5){
                            DuplicateObject("right");
                        }
                    }
                }else if(Input.GetMouseButton(1)){
                    if(weapon == 3){
                        if(tool == 1){
                            DeleteObject();
                        }
                    }else if(weapon == 2){
                        ImpulseObject();
                    }
                }

                if(Input.GetKeyDown(KeyCode.R)){
                    if(weapon == 1){
                        if(draggedObjectScript != null){
                            draggedObjectScript.DynamicObject();
                        }
                    }else if(weapon == 3){
                        if(tool == 2){
                            WeldObjects("remove");
                        }
                    }
                }
            }
        }
    }//end Update

    void FixedUpdate(){
        if(thePlayer.hit.collider != null){
            //if(thePlayer.hit.collider.gameObject.GetComponent<DragObj>() != null){
                draggedObjectScript = thePlayer.hit.collider.gameObject.GetComponent<DragObj>();
                objectRestrictions = thePlayer.hit.collider.gameObject.GetComponent<PhysicsRestrictions>();
            //}
            //if(thePlayer.hit.collider.gameObject.GetComponent<Rigidbody>() != null){
                draggedObjectRb = thePlayer.hit.collider.gameObject.GetComponent<Rigidbody>();
            //}
        }
        
    }//end FixedUpdate

    public void SetWeaponStatus(){
        if(deleteTool){
            weaponButtons.deleteTool.SetActive(true);
        }else {
            weaponButtons.deleteTool.SetActive(false);
        }

        if(weldTool){
            weaponButtons.weldTool.SetActive(true);
        }else {
            weaponButtons.weldTool.SetActive(false);
        }

        if(thrusterTool){
            weaponButtons.thrusterTool.SetActive(true);
        }else {
            weaponButtons.thrusterTool.SetActive(false);
        }

        if(wheelTool){
            weaponButtons.wheelTool.SetActive(true);
        }else {
            weaponButtons.wheelTool.SetActive(false);
        }

        if(duplicatorTool){
            weaponButtons.duplicatorTool.SetActive(true);
        }else {
            weaponButtons.duplicatorTool.SetActive(false);
        }

        if(hingeTool){
            weaponButtons.hingeTool.SetActive(true);
        }else {
            weaponButtons.hingeTool.SetActive(false);
        }

        if(gravityTool){
            weaponButtons.gravityTool.SetActive(true);
        }else {
            weaponButtons.gravityTool.SetActive(false);
        }

        if(balloonTool){
            weaponButtons.balloonTool.SetActive(true);
        }else {
            weaponButtons.balloonTool.SetActive(false);
        }
    }

    public void CheckWeapon(){
    	if(weapon == 1){//PHYSICS GUN
    		PhysicsGun();
            ToolDescriptor();
    	}
    	if(weapon == 2){//IMPULSE
    		ImpulseGun();
            ToolDescriptor();
    	}
        if(weapon == 3){
            ToolGun();
            ToolDescriptor();
        }
    }
    //WEAPON UI
    public void SetTextAliases(){
        weaponTextPrev = GameObject.Find("WeaponPrev").GetComponent<Text>();
        weaponTextMain = GameObject.Find("WeaponMain").GetComponent<Text>();
        weaponTextNext = GameObject.Find("WeaponNext").GetComponent<Text>();
        selectedWeapon = GameObject.Find("Weapon").GetComponent<Text>();
        selectedWeaponFunction = GameObject.Find("Function").GetComponent<Text>();
    }
    void PhysicsGun() {
        weaponTextPrev.text = weapon3;//CHANGE THIS WHEN ADDING WEAPONS

        weaponTextMain.text = weapon1;
        weaponTextNext.text = weapon2;
    }
    void ImpulseGun() {
        weaponTextPrev.text = weapon1;
        weaponTextMain.text = weapon2;
        weaponTextNext.text = weapon3;
        //ImpulseObject();
    }
    void ToolGun() {
        weaponTextPrev.text = weapon2;
        weaponTextMain.text = weapon3;

        weaponTextNext.text = weapon1;//CHANGE THIS WHEN ADDING WEAPONS
    }//END WEAPON UI

    //TOOL FUNCTIONS
    void WeldObjects(string function) {
        if(function == "weld"){
            if(objectRestrictions.allowWeld == true){
                if(weldObjectNum == 0){//Selecting first object
                    if(draggedObjectRb != null){
                        weldObj0 = thePlayer.hit.collider.gameObject;
                        weldObjectNum = 1;
                        ShowSelector();
                    }
                }else if(weldObjectNum == 1) {//Selecting second object
                    if(draggedObjectRb != null){
                        weldObj1 = thePlayer.hit.collider.gameObject;
                        weldObjectNum = 0;

                        if(weldObj0 != weldObj1){
                            joint = weldObj0.AddComponent<FixedJoint>();
                            joint.connectedBody = weldObj1.GetComponent<Rigidbody>();
                            ShowSelector();
                        }
                    }
                }
            }
        }else if(function == "remove"){
            Destroy(thePlayer.hit.collider.gameObject.GetComponent<FixedJoint>());
            ShowSelector();
        }
    }

    void ImpulseObject() {
        if(draggedObjectRb != null){
            ShowSelector();
            draggedObjectScript.DynamicObject();
            thePlayer.hit.rigidbody.AddForceAtPosition(GameObject.Find("Camera").transform.forward * thePlayer.pokeForce * 10, thePlayer.hit.point, ForceMode.Impulse);
        }
    }

    void DeleteObject() {
        if(draggedObjectScript != null){
            if(objectRestrictions.allowDelete == true){
                if(objectRestrictions.allowDelete == true){
                    Destroy(thePlayer.hit.collider.gameObject);
                    ShowSelector();
                    thePlayer.numberOfItems--;
                    Debug.Log(thePlayer.numberOfItems);
                }
            }
        }
    }

    void SpawnThruster() {
        if(draggedObjectRb != null){
            if(objectRestrictions.allowThruster == true){
                ShowSelector();
                var tmpThrust = Instantiate(thePlayer.thruster, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                tmpThrust.transform.parent = thePlayer.hit.collider.gameObject.transform;
            }
        }
    }

    void SpawnHinge() {
        if(draggedObjectRb != null){
            if(objectRestrictions.allowWheel == true){
                //ShowSelector();
                if(thePlayer.hit.collider.gameObject.GetComponent<Steering>() != null){
                    var tmpSteer = Instantiate(thePlayer.steering, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                    tmpSteer.GetComponent<FixedJoint>().connectedBody = thePlayer.hit.collider.gameObject.GetComponent<HingeJoint>().connectedBody.gameObject.GetComponent<Rigidbody>();

                }else{
                    var tmpSteer = Instantiate(thePlayer.steering, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                    tmpSteer.GetComponent<FixedJoint>().connectedBody = thePlayer.hit.collider.gameObject.GetComponent<Rigidbody>();
                }
            }
        }
    }

    void SpawnWheel() {
        if(draggedObjectRb != null){
            if(objectRestrictions.allowWheel == true){
                ShowSelector();
                if(thePlayer.hit.collider.gameObject.GetComponent<Steering>() != null){
                    var tmpWheel = Instantiate(thePlayer.wheel, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                    tmpWheel.GetComponent<HingeJoint>().connectedBody = thePlayer.hit.collider.gameObject.GetComponent<HingeJoint>().connectedBody.gameObject.GetComponent<Rigidbody>();

                }else{
                    var tmpWheel = Instantiate(thePlayer.wheel, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                    tmpWheel.GetComponent<HingeJoint>().connectedBody = thePlayer.hit.collider.gameObject.GetComponent<Rigidbody>();
                }
            }
        }
    }

    void ToggleGravity(){
        Rigidbody rb = thePlayer.hit.collider.gameObject.GetComponent<Rigidbody>();
        if(rb != null){
            if(rb.useGravity == true){
                rb.useGravity = false;
            }else if(rb.useGravity == false){
                rb.useGravity = true;
            }
        }
    }

    void SpawnBalloon(){
        if(draggedObjectRb != null){
            var tmpBalloon = Instantiate(thePlayer.balloon, thePlayer.hit.point, Quaternion.identity);
            tmpBalloon.GetComponent<Balloon>().SetFixPoint(thePlayer.hit.point, thePlayer.hit.collider.gameObject);
        }
    }

    void DuplicateObject(string click) {
        dupeObjectNum = 1;
        if(click == "left"){
            if(thePlayer.hit.collider != null && draggedObjectRb != null){
                dupeObject = thePlayer.hit.collider.gameObject;
                dupeRotation = dupeObject.transform.rotation;
                dupeObjectNum = 2;
            }
        }
        if(click == "right" && dupeObject != null){
            Instantiate(dupeObject, thePlayer.hit.point + (thePlayer.hit.normal + Vector3.up)*1, dupeRotation);
        }
    }

    //For any tools that place objects
    void SpawnPreview(string item) {
        previewName = item;
        if(thePlayer.hit.collider != null && draggedObjectRb != null && weapon == 3 && item != "delete"){
            if(previewVisible == false){
                if(item == "thruster"){
                    previewVisible = true;
                    previewObject = Instantiate(thePlayer.thrusterPreview, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                }else if(item == "wheel"){
                    previewVisible = true;
                    previewObject = Instantiate(thePlayer.wheelPreview, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
                }
            }
            previewObject.transform.rotation = Quaternion.LookRotation(thePlayer.hit.normal);
            previewObject.transform.position = thePlayer.hit.point;
        }else if(previewObject != null && item == "delete"){
            Destroy(previewObject);
            previewVisible = false;
        }
    }

    //Shows effect point of certain tools
    public void ShowSelector() {
        var selector = Instantiate(thePlayer.selector, thePlayer.hit.point, Quaternion.LookRotation(thePlayer.hit.normal));
        Destroy(selector, 0.2f);
    }

    public void ToolDescriptor() {
        if(weapon == 1){
            selectedWeapon.text = "Physics Gun";
            selectedWeaponFunction.text = "Click on an object to manipulate it. Press e to rotate object\nRight click to freeze object. Scroll to change distance of object";
        
        }else if(weapon == 2){
            selectedWeapon.text = "Impulse Gun";
            selectedWeaponFunction.text = "Click on an object to give it a nudge\nHold right click to continuosly push it";
        
        }else if(weapon == 3){
            switch(tool){
                case 1://DELETE TOOL
                    selectedWeapon.text = "Delete Tool";
                    selectedWeaponFunction.text = "Left click to single delete \nHold right click to continuosly delete";
                    break;
                case 2://WELD TOOL
                    selectedWeapon.text = "Weld Tool";
                    if(weldObjectNum == 0){
                        selectedWeaponFunction.text = "Left click to select first object to weld \nRight click or press R while hovering over object to unweld object";
                    }else if(weldObjectNum == 1){
                        selectedWeaponFunction.text = "Left click to select second object to weld";
                    }
                    break;
                case 3://THRUSTER TOOL
                    selectedWeapon.text = "Thruster Tool";
                    selectedWeaponFunction.text = "Press the up and down arrow keys to fire thrusters";
                    break;
                case 4://WHEEL TOOL
                    selectedWeapon.text = "Wheel Tool";
                    selectedWeaponFunction.text = "They rotate about the point you place them";
                    break;
                case 5://DUPLICATOR TOOL
                    selectedWeapon.text = "Duplicator Tool";
                    if(dupeObjectNum == 1){
                        selectedWeaponFunction.text = "Click on an object to be able to duplicate it";
                    }else if(dupeObjectNum == 2){
                        selectedWeaponFunction.text = "Click on an object to be able to duplicate it \nRight click anywhere to place the object";
                    }
                    break;
                case 6://HINGE TOOL
                    selectedWeapon.text = "Hinge Tool";
                    selectedWeaponFunction.text = "Press the left and right keys to turn hinges \nPress e while hovering over a hinge to change its direction";
                    break;
                case 7://GRAVITY TOGGLE
                    selectedWeapon.text = "Gravity Toggle";
                    selectedWeaponFunction.text = "Press the left and right keys to turn hinges \nPress e while hovering over a hinge to change its direction";
                    break;
                case 8:
                    selectedWeapon.text = "Spawn Balloon";
                    selectedWeaponFunction.text = "Click on an object to spawn a ballon on it.";
                    break;
            }
        }
    }
    /*
    == List of tools in order (1 > n) ==
    
        Delete Tool
        Weld Tool
        Thruster Tool
        Wheel Tool
        Duplicator Tool
    */
}
