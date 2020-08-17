using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("In ms^-1")] [SerializeField] float xControlSpeed = 11f;   //speed of which the ship moves in the x axis
    [Tooltip("In ms")] [SerializeField] float xRange = 6.3f;    //keeps the ship within the bounderies of the screen
    [Tooltip("In ms^-1")] [SerializeField] float yControlSpeed = 11f;   //speed of which the ship moves in the x axis
    [Tooltip("In ms")] [SerializeField] float yRange = 3.3f;      //keeps the ship within the bounderies of the screen
    [SerializeField] GameObject[] guns;

    [Header("Screen-position Based")]
    float positionPitchFactor = -5f;    //pitches angle of the ship to the camera (high up will be pitched down vise versa)
    float positionYawFactor = 6f;    //yaws angle of the ship to the camera (far right will be yawed left vise versa)

    [Header("Control-throw Based")]
    float controlPitchFactor = -20f;    //ships curve into the direction (leans back when going up vise versa)
    float controlRollFactor = -20f;    //ships roll into the direction (tips right when going right vise versa)                 30

    float xThrow, yThrow;   // x and y current axis value (-1 to 1)
    bool isControlEnabled = true;

        
    //------------------------------ ROTATION RESETS/LOCKS FOR MAX AXIS RANGES

    bool maxUp;         //bool for which y pos is maxed
    bool maxDown;       //bool for which y pos is maxed
    bool alreadyMaxY;   //when the player is already at the top/bottom (ie dont rotate reset again)
    bool up, down;

    bool maxLeft;       //bool for which x pos is maxed
    bool maxRight;      //bool for which x pos is maxed
    bool alreadyMaxX;   //when the player is already at the right/left (ie dont rotate reset again) 
    //bool right, left;

    //ensures that all the bools are awake they've started in safe area against rotation resets
    private void Start()
    {
        maxUp = false;
        maxDown = false;
        alreadyMaxY = false;

        up = down = false;

        maxLeft = false;
        maxRight = false;
        alreadyMaxX = false;

        //right = left = false;
    }

    //catches the maxes (x and y positions)
    private void Update() 
    {
        if (isControlEnabled)
        {
            YPositionMaxRangeCatch();
            XPositionMaxRangeCatch();
            ProcessFiring();
        }
    }


    //FixedUpdate as working with physics
    void FixedUpdate()
    {
        if (isControlEnabled)
        {
            ProcessTranslation();   //position
            ProcessRotation();  //rotation
        }
    }

    void OnPlayerDeath()
    {
        isControlEnabled = false;   
    }

    //from update
    private void YPositionMaxRangeCatch()   //pitch
    {
        if (transform.localPosition.y < 3.3f && transform.localPosition.y > -3.3f)
        {
            alreadyMaxY = false;    //theyre in a max range for the y pos axis
        }
        if (!alreadyMaxY)   //if it hasnt reset yet, itll pick mark the correct bool so it knows which rotation (- or +) is needed 
        {
            if (transform.localPosition.y == 3.3f)
            {
                maxUp = true;
                up = true;
            }
            else if (transform.localPosition.y == -3.3f)
            {
                maxDown = true;
                down = true;
            }
            else
            {
                maxUp = maxDown = false;    //if known of the ranges are currently reached (ensures next if doesnt happen)
            }
        }


        if (maxUp || maxDown)
        {
            alreadyMaxY = true;  //so that this wont happen again as long as the y pos hasnt changed
            StartCoroutine(Pitch(0.15f, maxUp.Equals(true)));                                                               //WAS 0.15 DURING TESTING WITH RESET!!!!!!!!!!!!!!!!!!!!!!
            maxUp = maxDown = false;            
        }
    }

    //from update
    private void XPositionMaxRangeCatch()   //roll (same procedure as above with Y)
    {
        if (transform.localPosition.x < 6.3f && transform.localPosition.x > -6.3f)
        {
            alreadyMaxX = false;
        }
        if (!alreadyMaxX)
        {
            if (transform.localPosition.x == 6.3f)
            {
                maxRight = true;
                //right = true;
            }
            else if (transform.localPosition.x == -6.3f)
            {
                maxLeft = true;
                //left = true;
            }
            else
            {
                maxRight = maxLeft = false;
            }
        }


        if (maxRight || maxLeft)
        {
            alreadyMaxX = true;
            StartCoroutine(Roll(0.15f, maxRight.Equals(true)));
            maxRight = maxLeft = false;
        }
    }


    //resets the pitch of the ship
    IEnumerator Pitch(float duration, bool position)
    {
        float angle;
        if (position)   //which way should it be resetting to...
        {   //TOP
            angle = 16f;    //the higher the number, the more it'll overshoot the final angle it'll reach by the end of the while loop
        }
        else
        {   //BOTTOM
            angle = -20f;  // WAS 60 DURING TESTING WITH RESET
        }
        float startRotation = transform.localEulerAngles.x;
        float endRotation = startRotation + angle;  
        float t = 0.0f;
        while (t < duration)
        {
            if (up && (transform.localEulerAngles.x >= 346.0f))     //I NEED TO MAKE IT SO THAT THESE 2 CONDITIONS KNOW IF IM MEANT TO BE PITCHING UP OR DOWN OTHERWISE THE CONDITION WILL BE MET FOR THE WRONG THING (PS maxUp WONT WORK FOR THIS AS THEYRE SENSITIVE TO ANOTHER PART, MAYBE I COULD MAKE ANOTHER 2 CONDITIONS THAT CAN BE RESET AFTER THE WHILE AND IN THE CONDITIONS EXECUTIONS)
            {   //if the ship's angle goes below the flat angle for the top, it should cancel the rest of the rotation downwards
                up = false;
                down = false;
                yield break;
            }
            else if (down && (transform.localEulerAngles.x <= 16.4f))  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!FOR SOME REASON, LANDING IN THE BOTTOM LEFT OR RIGHT MAKES THIS ACTIVATE LIKE IM GENTLY TOUCHING IT WHICH IM NOT DOING????????? (BUT THEN HITTING TOP RIGHT OR LEFT FIXES IT)                       
            {   //if the ship's angle goes above the flat angle for the bottom, it should cancel the rest of the rotation upwards
                up = false;
                down = false;
                yield break;
            }
            else               
            {
                t += Time.deltaTime;    
                float xRotation = Mathf.Lerp(startRotation, endRotation, t / duration);     //next position for this while loop iteration
                transform.localEulerAngles = new Vector3(xRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
                yield return null;
            }
        }
    }

    //resets the roll of the ship
    IEnumerator Roll(float duration, bool position)
    {
        float angle;
        if (position)   //which way should it be resetting to...
        {
            angle = 20.0f;    //the higher the number, the more it'll overshoot the final angle it'll reach by the end of the while loop
        }
        else
        {
            angle = -20.0f; //20
        }
        float startRotation = transform.localEulerAngles.z;
        float endRotation = startRotation + angle;
        float t = 0.0f;

        if (xThrow != -1 && xThrow != 1)    //there were too many factors effecting the rotation for me to use the same as pitch so this is the best option without it getting overly complex for something vertually unnoticable
        {
            //right = false;
            //left = false;
            yield break;
        }
        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration);     //next position for this while loop iteration
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, zRotation);
            yield return null;
        }
    }
    
    //-----------------------------


    //Ships Position
    private void ProcessTranslation()
    {
        xThrow = CrossPlatformInputManager.GetAxis("Horizontal"); //current x axis value between/from -1 and 1 
        float xOffset = xThrow * xControlSpeed * Time.deltaTime;   //desired position is made with input and a speed to set the distance

        float rawXPos = transform.localPosition.x + xOffset;    //current x plus the new frame distance added 
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);  //how far the ship moves on the x axis (local position with be the chosen value unless it exceeds the range which will cap it at the range)


        yThrow = CrossPlatformInputManager.GetAxis("Vertical"); //current y axis value between/from -1 and 1 
        float yOffset = yThrow * yControlSpeed * Time.deltaTime;   //desired position is made with input and a speed to set the distance

        float rawYPos = transform.localPosition.y + yOffset;    //current y plus the new frame distance added 
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);  //how far the ship moves on the x axis (local position with be the chosen value unless it exceeds the range which will cap it at the range)


        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z); //new transform position for this frame
    }

    
    //Ships Rotation
    private void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor; //angle 
        float pitchDueToControl;  //tip/curve
         
        float pitch;
        if (alreadyMaxY)    //ie dont use the tip/curve up or down
        {
            pitch = pitchDueToPosition;
        }
        else
        {
            pitchDueToControl = yThrow * controlPitchFactor;
            pitch = pitchDueToPosition + pitchDueToControl;
        }

        float yaw = transform.localPosition.x * positionYawFactor;  //angle

        float roll;
        if (alreadyMaxX)    //ie dont use the tilt right or left
        {
            roll = transform.localRotation.z; 
        }
        else
        {
            roll = xThrow * controlRollFactor;
        }

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);   //new transform rotation using the above 3 axis (Quaternion.Eular for the correct order of rotation)
    }


    void ProcessFiring()
    {
        if (CrossPlatformInputManager.GetButton("Fire"))
        {
            //ActivateGuns();
            Shooting(true);
        }
        else
        {
            Shooting(false);
            //DeactivateGuns();
        }
    }

    private void Shooting(bool firing) 
    {
        foreach (GameObject gun in guns)
        {
            var emissionModule = gun.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = firing;
        }
    }



    /*private void ActivateGuns()
    {
        foreach (GameObject gun in guns)
        {
            gun.SetActive(true);    //only use 1 method that will set it to opposite of current 
        }
    }

    private void DeactivateGuns()
    {
        foreach (GameObject gun in guns)
        {
            gun.SetActive(false);
        }
    }*/
}