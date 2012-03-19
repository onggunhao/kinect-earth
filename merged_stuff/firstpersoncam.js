// firstpersoncam.js
/*
Copyright 2008 Google Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Code for a simple quake-style camera.
//
// Notes: This is a very simple camera and intended to be so. The 
// camera's altitude is always 0, relative to the surface of the
// earth.
//

//----------------------------------------------------------------------------
// Global Variables
//----------------------------------------------------------------------------

soundSwitch = false;
isSoundOn = false;

turnLeft = false;
turnRight = false;
tiltUp = false;
tiltDown = false;

moveForward = false;
moveBackward = false;
strafeLeft = false;
strafeRight = false;
altitudeUp = false;
altitudeDown = false;

forward_speed = 1;
turn_speed = 1;
augmented_reality = false;
//relocate_cam = false;
relocate_sf = false;
relocate_paris = false;

/* Augmented reality variables */
var networkLink = null;

/* Initialized Variables */
INITIAL_CAMERA_ALTITUDE = 1.8; // Roughly 6 feet tall
cameraAltitude = INITIAL_CAMERA_ALTITUDE;
//----------------------------------------------------------------------------
// Utility Functions
//----------------------------------------------------------------------------

// Keep an angle in [-180,180]
function fixAngle(a) {
  while (a < -180) {
    a += 360;
  }
  while (a > 180) {
    a -= 360;
  }
  return a;
}

//----------------------------------------------------------------------------
// Input Handlers
//----------------------------------------------------------------------------

function keyDown(event) {
  var me = this;

  if (!event) {
    event = window.event;
  }
  if (event.keyCode == 85) {  // Key U: Altitude Up
    playSound(true);
    altitudeUp = true;
    event.returnValue = false;
	jetpack.setVisibility(true);
  } else if (event.keyCode == 74) {  // Key J: Altitude Down
    playSound(false);
    altitudeDown = true;
    event.returnValue = false;
	jetpack.setVisibility(false);
  } else if (event.keyCode == 37 || event.keyCode == 75) {  // Key Left Arrow or K: Turn Left.
    turnLeft = true;
    turn_speed = 1;
    leftActive.setVisibility(true);
    leftInactive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 188) {    // Key Comma: Turn left FAST
  	turnLeft = true;
  	turn_speed = 2.75;
  	leftActive.setVisibility(true);
  	leftInactive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 190) {    // Key Period: Turn right FAST
  	turnRight = true;
  	turn_speed = 2.75;
  	rightActive.setVisibility(true);
  	rightInactive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 39 || event.keyCode == 76) {  // Key Right Arrow or L: Turn Right.
    turnRight = true;
    turn_speed = 1;
    rightActive.setVisibility(true);
    rightInactive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 38) {  // Tilt Up.
    tiltUp = true;
    event.returnValue = false;
  } else if (event.keyCode == 40) {  // Tilt Down.
    tiltDown = true;
    event.returnValue = false;
  } else if (event.keyCode == 65 || 
             event.keyCode == 97) {  // Strafe Left.
    strafeLeft = true;
    event.returnValue = false;
  } else if (event.keyCode == 68 || 
             event.keyCode == 100) {  // Strafe Right.
    strafeRight = true;
    event.returnValue = false;
  } else if (event.keyCode == 87 || 
             event.keyCode == 119) {  // Key W: Move Forward.
    moveForward = true;
    upActive.setVisibility(true);
    upInactive.setVisibility(false);
    forward_speed = 1;
    event.returnValue = false;    
  } else if (event.keyCode == 50) {	  // Key 2: Move Forward, faster
    moveForward = true;
    upActive.setVisibility(true);
    upInactive.setVisibility(false);
  	forward_speed = 3;
  	event.returnValue = false;
  } else if (event.keyCode == 83 || 
             event.keyCode == 115) {  // Key S: Move Backward

    moveBackward = true;
    downActive.setVisibility(true);
    downInactive.setVisibility(false);
    event.returnValue = false;
  } else if (augmented_reality == false && event.keyCode == 89) {   // Y: Show augmented reality
  		var link = ge.createLink('');		// class ge inherited from index2.html
		//var href = 'http://www.stanford.edu/~hyunggu/etc/cs247_twit2.kml';
		var href = 'http://cs247.eidus.org/tweets.kml';
	  	link.setHref(href);
	  	networkLink = ge.createNetworkLink('');
	  	networkLink.set(link, true, true); // Sets the link, refreshVisibility, and flyToView.
	  	ge.getFeatures().appendChild(networkLink);
		augmented_reality = true;
  		event.returnValue = false;
  } else if (event.keyCode == 48) {   // Key 0: relocate to SF
  	relocate_sf = true;
  	event.returnValue = false;
  } else if (event.keyCode == 57) {    // Key 9: relocate to Paris
    relocate_paris = true;
    event.returnValue = false;
  } else {
    return true;
  }
  return false;
}

function keyUp(event) {
  var me = this;

  if (!event) {
    event = window.event;
  } 
  if (event.keyCode == 85) {  // Altitude Up
    altitudeUp = false;
    event.returnValue = false;
  } else if (event.keyCode == 74) {  // Altitude Down
    altitudeDown = false;
    event.returnValue = false;
  } else if (event.keyCode == 37 || event.keyCode == 75) {  // Left.
    turnLeft = false;
    leftInactive.setVisibility(true);
    leftActive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 188) {    // Turn left FAST
    turnLeft = false;
    leftInactive.setVisibility(true);
    leftActive.setVisibility(false);
  	turn_speed = 1;
    event.returnValue = false;
  } else if (event.keyCode == 190) {    // Turn right FAST
    turnRight = false;
    rightInactive.setVisibility(true);
    rightActive.setVisibility(false);
  	turn_speed = 1;
    event.returnValue = false;
  } else if (event.keyCode == 39 || event.keyCode == 76) {  // Right.
    turnRight = false;
    rightInactive.setVisibility(true);
    rightActive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 38) {  // Up.
    tiltUp = false;
    event.returnValue = false;
  } else if (event.keyCode == 40) {  // Down.
    tiltDown = false;
    event.returnValue = false;   
  } else if (event.keyCode == 65 || 
             event.keyCode == 97) {  // Strafe Left.
    strafeLeft = false;
    event.returnValue = false;
  } else if (event.keyCode == 68 || 
             event.keyCode == 100) {  // Strafe Right.
    strafeRight = false;
    event.returnValue = false;
  } else if (event.keyCode == 87 || 
             event.keyCode == 119) {  // Move Forward.
    moveForward = false;
    upInactive.setVisibility(true);
    upActive.setVisibility(false);
    event.returnValue = false;    
  } else if (event.keyCode == 50) {	  // Move Forward, faster
  	moveForward = false;
  	forward_speed = 1;
  	upInactive.setVisibility(true);
  	upActive.setVisibility(false);
  	event.returnValue = false;
  } else if (event.keyCode == 83 || 
             event.keyCode == 115) {  // Move Backward

    moveBackward = false;
    downInactive.setVisibility(true);
    downActive.setVisibility(false);
    event.returnValue = false;
  } else if (event.keyCode == 89) {   // Y: Show augmented reality
  	ge.getFeatures().removeChild(networkLink);
  	augmented_reality = false;
  	event.returnValue = false;
  } else if (event.keyCode == 48) {   // Key 0: relocate to SF
  	relocate_sf = false;
  	event.returnValue = false;
  } else if (event.keyCode == 57) {   // Key 9: relocate to Paris
    relocate_paris = false;
    event.returnValue = false;
    } 
  return false;
}



//----------------------------------------------------------------------------
// JSObject - FirstPersonCamera
//----------------------------------------------------------------------------

function FirstPersonCam() {
  var me = this;
 
  // The anchor point is where the camera is situated at. We store
  // the current position in lat, lon, altitude and in cartesian 
  // coordinates.
  me.localAnchorLla = [37.79333, -122.40, 0];  // San Francisco
  me.localAnchorCartesian = V3.latLonAltToCartesian(me.localAnchorLla);

  // Heading, tilt angle is relative to local frame
  me.headingAngle = 0;
  me.tiltAngle = 10 * Math.PI / 180.0;

  // Initialize the time
  me.lastMillis = (new Date()).getTime();  
  
  // Used for bounce.
  me.distanceTraveled = 0;              

  // prevent mouse navigation in the plugin
  ge.getOptions().setMouseNavigationEnabled(false);

  // Updates should be called on frameend to help keep objects in sync.
  // GE does not propogate changes caused by KML objects until an
  // end of frame.
  google.earth.addEventListener(ge, "frameend",
                                function() { me.update(); });
}

FirstPersonCam.prototype.updateOrientation = function(dt) {
  var me = this;

  // Based on dt and input press, update turn angle.
  if (turnLeft || turnRight) {  
    var turnSpeed = 15.0 * turn_speed; // radians/sec
    if (turnLeft)
      turnSpeed *= -1.0;
    me.headingAngle += turnSpeed * dt * Math.PI / 180.0;
  }
  if (tiltUp || tiltDown) {
    var tiltSpeed = 40.0; // radians/sec
    if (tiltDown)
      tiltSpeed *= -1.0;
    me.tiltAngle = me.tiltAngle + tiltSpeed * dt * Math.PI / 180.0;
    // Clamp
    var tiltMax = 60.0 * Math.PI / 180.0;
    var tiltMin = -50.0 * Math.PI / 180.0;
    if (me.tiltAngle > tiltMax)
      me.tiltAngle = tiltMax;
    if (me.tiltAngle < tiltMin)
      me.tiltAngle = tiltMin;
  } 

}

FirstPersonCam.prototype.updatePosition = function (dt) {
    var me = this;

    // Convert local lat/lon to a global matrix. The up vector is 
    // vector = position - center of earth. And the right vector is a vector
    // pointing eastwards and the facing vector is pointing towards north.
    var localToGlobalFrame = M33.makeLocalToGlobalFrame(me.localAnchorLla);

    // Move in heading direction by rotating the facing vector around
    // the up vector, in the angle specified by the heading angle.
    // Strafing is similar, except it's aligned towards the right vec.
    var headingVec = V3.rotate(localToGlobalFrame[1], localToGlobalFrame[2],
                             -me.headingAngle);
    var rightVec = V3.rotate(localToGlobalFrame[0], localToGlobalFrame[2],
                             -me.headingAngle);
    var strafe = 0;

    if (relocate_sf) {
        me.localAnchorLla = [37.79333, -122.40, 0];  // San Francisco
        me.localAnchorCartesian = V3.latLonAltToCartesian(me.localAnchorLla);
        me.headingAngle = 0;
        me.tiltAngle = 0;
    } else if (relocate_paris) {
        me.localAnchorLla = [48.8583, 2.2945, 0];  // Paris
        me.localAnchorCartesian = V3.latLonAltToCartesian(me.localAnchorLla);
        me.headingAngle = 0;
        me.tiltAngle = 0;
    } else {
        // Calculate strafe/forwards  
        if (strafeLeft || strafeRight) {
            var strafeVelocity = 20;
            if (strafeLeft)
                strafeVelocity *= -1;
            strafe = strafeVelocity * dt;
        }
        var forward = 0;
        if (moveForward || moveBackward) {
            var forwardVelocity = 15 * forward_speed * (1 + 10*(cameraAltitude/500));
            if (moveBackward)
                forwardVelocity *= -1;
            forward = forwardVelocity * dt;
        }
        if (altitudeUp) {
            cameraAltitude += Math.min(0.5, 0.5 * (cameraAltitude / 15));

            // Handles tilt
            target_angle = (-40.0 * Math.PI / 180.0) * (cameraAltitude / 500); 	// 500 is the height at which it tapers
            if (me.tiltAngle != target_angle) {
                angle_difference = me.tiltAngle - target_angle;
                me.tiltAngle = me.tiltAngle - angle_difference / 10;
            }

        } else if (altitudeDown) {
            jetpack.setVisibility(false);
            tiltDownSpeed = 70.0;

            // Handles tilt
            if (cameraAltitude > 30.0) {
                cameraAltitude -= 1.0;
                me.tiltAngle = me.tiltAngle - tiltDownSpeed * dt * Math.PI / 180.0;
                var tiltMinimum = -75.0 * Math.PI / 180.0;
                if (me.tiltAngle < tiltMinimum) {
                    me.tiltAngle = tiltMinimum;
                }
            } else {
                cameraAltitude -= 0.5;
                target_angle = 0;
                if (me.tiltAngle != target_angle) {
                    angle_difference = me.tiltAngle - target_angle;
                    me.tiltAngle = me.tiltAngle - angle_difference / 10;
                }
            }
		} else {
			//if (cameraAltitude < 30.0) target_angle = 0;
			//else target_angle = (-40.0 * Math.PI / 180.0) * (cameraAltitude / 500);
			
			target_angle = (-70.0 * Math.PI / 180.0) * (cameraAltitude / 500);
            if (me.tiltAngle != target_angle) {
                angle_difference = me.tiltAngle - target_angle;
            	me.tiltAngle = me.tiltAngle - angle_difference / 10;
            }
		}
        /*
        else {
	  	
        target_angle = (-30.0 * Math.PI / 180.0) * (cameraAltitude / 500);		// Negative number
        if (me.tiltAngle != target_angle) {
        angle_difference = me.tiltAngle - target_angle;						
        me.tiltAngle = me.tiltAngle - angle_difference/10;
        }
        }
        */
        cameraAltitude = Math.max(1.8, cameraAltitude);

        me.distanceTraveled += forward;

        // Add the change in position due to forward velocity and strafe velocity 
        me.localAnchorCartesian = V3.add(me.localAnchorCartesian,
									   V3.scale(rightVec, strafe));
        me.localAnchorCartesian = V3.add(me.localAnchorCartesian,
									   V3.scale(headingVec, forward));

        // Convert cartesian to Lat Lon Altitude for camera setup later on.
        me.localAnchorLla = V3.cartesianToLatLonAlt(me.localAnchorCartesian);
    }
}

FirstPersonCam.prototype.updateCamera = function() {
  var me = this;
           
  var lla = me.localAnchorLla;
  lla[2] = ge.getGlobe().getGroundAltitude(lla[0], lla[1]); 
  
  // Will put in a bit of a stride if the camera is at or below 1.7 meters
  var bounce = 0;  
  if (cameraAltitude <= INITIAL_CAMERA_ALTITUDE /* 1.7 */) {
    bounce = 0.3 * Math.abs(Math.sin(25 * me.distanceTraveled *
                                     Math.PI / 180)); 
  }
    
  // Update camera position. Note that tilt at 0 is facing directly downwards.
  //  We add 90 such that 90 degrees is facing forwards.
  var la = ge.createCamera('');
  la.set(me.localAnchorLla[0], me.localAnchorLla[1],
         cameraAltitude + bounce,
         ge.ALTITUDE_RELATIVE_TO_SEA_FLOOR,
         fixAngle(me.headingAngle * 180 / Math.PI), /* heading */         
         me.tiltAngle * 180 / Math.PI + 90, /* tilt */         
         0 /* altitude is constant */         
         );  
  ge.getView().setAbstractView(la);         
};

FirstPersonCam.prototype.update = function() {
  var me = this;
  
  ge.getWindow().blur();
  
  // Update delta time (dt in seconds)
  var now = (new Date()).getTime();  
  var dt = (now - me.lastMillis) / 1000.0;
  if (dt > 0.25) {
    dt = 0.25;
  }  
  me.lastMillis = now;    
    
  // Update orientation and then position  of camera based
  // on user input   
  me.updateOrientation(dt);
  me.updatePosition(dt);
           
  // Update camera
  me.updateCamera();
};

function playSound(soundSwitch)	{
	if (soundSwitch && !isSoundOn) 	{
		jpsound.src="http://www.stanford.edu/~hyunggu/etc/cs247/jetpack.mp3";
		document.getElementById('jpsound').volume = 0;
		isSoundOn = true;
	}
	else if (!soundSwitch && isSoundOn) {
		//jpsound.src="";
		document.getElementById('jpsound').volume = -10000;
		isSoundOn = false;
	}
};
