#pragma strict

public var tank1:GameObject;
public var tank2:GameObject;
public var tank3:GameObject;

private var moveDirection:String = "forward"; //or backward


function Start(){

	tank1.GetComponent(TankControls).PlayTrackWheelsAnimation(1);
	tank2.GetComponent(TankControls).PlayTrackWheelsAnimation(1);
	tank3.GetComponent(TankControls).PlayTrackWheelsAnimation(1);

}



function Update () {

	if(moveDirection == "forward"){
		// Move the object forward along its z axis 1 unit/second.
		tank1.transform.Translate(Vector3.forward * Time.deltaTime);
	
		tank2.transform.Translate(Vector3.forward * Time.deltaTime);
	
		tank3.transform.Translate(Vector3.forward * Time.deltaTime);
	
	}else if(moveDirection == "backward"){
		  
		tank1.GetComponent(TankControls).PlayTrackWheelsAnimation(-1); 
		tank2.GetComponent(TankControls).PlayTrackWheelsAnimation(-1); 
		tank3.GetComponent(TankControls).PlayTrackWheelsAnimation(-1);
		
		// Move the object backward along its z axis 1 unit/second.
		tank1.transform.Translate(-Vector3.forward * Time.deltaTime);
	
		tank2.transform.Translate(-Vector3.forward * Time.deltaTime);
	
		tank3.transform.Translate(-Vector3.forward * Time.deltaTime);
	
	}
	
}


function LateUpdate(){
	if(tank3.transform.position.z >= 12){ 
		
		tank1.GetComponent(TankControls).PlayTrackWheelsAnimation(-1);
		tank2.GetComponent(TankControls).PlayTrackWheelsAnimation(-1);
		tank3.GetComponent(TankControls).PlayTrackWheelsAnimation(-1);
	
		moveDirection = "backward";
	
	}else if(tank3.transform.position.z <= -3){
			
		tank1.GetComponent(TankControls).PlayTrackWheelsAnimation(1);
		tank2.GetComponent(TankControls).PlayTrackWheelsAnimation(1);
		tank3.GetComponent(TankControls).PlayTrackWheelsAnimation(1);
	
		moveDirection = "forward";
	}


}