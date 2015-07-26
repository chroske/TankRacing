#pragma strict

//control track's speed
public var trackSpeed:float = 1.0;


function Start () {
	
	PlayTrackWheelsAnimation(1);
	
}


//Play Tracks and Wheels animations
//Parameter: trackDirection
//control track's moving direction
//1 is moving forward
//-1 is moving backward
function PlayTrackWheelsAnimation(trackDirection:int){

	var tracks:GameObject = transform.Find("tracks").transform.gameObject;

	//set track's speed
	//tracks.GetComponent(Animation)["track_animation"].speed = trackDirection * trackSpeed;
	tracks.GetComponent(Animation).Play();
	
	//find all the wheels
	var allWheels:GameObject[] = GameObject.FindGameObjectsWithTag("wheel");
	if(allWheels.length > 0){
		for(var wheel:GameObject in allWheels){
			wheel.GetComponent(Animation)["wheel_rotation_animation"].speed = trackDirection * trackSpeed;
			wheel.GetComponent(Animation).Play();
		
		}
	}

}