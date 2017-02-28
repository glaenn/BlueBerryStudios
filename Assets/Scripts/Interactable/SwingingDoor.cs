using UnityEngine;
using System.Collections;

public class SwingingDoor : Interactive
{
    private float rotation = 0;
    [SerializeField] float maxRotation = 150;
    [SerializeField] float openTime = 1.5f;

    void Awake()
    {
        if (state == 0)
        {
            rotation = 0;
            state = 0;
        }
        else if (state == 1)
        {
            rotation = maxRotation;
            state = 151;
        }
    }

    public override void Activate(GameObject player)
    {
        if (state < 151)
            state =  (int)rotation + 151;
        else if (state > 150)
            state = (int)rotation;

        base.Activate(player);
    }

    protected override void SetToState()
    {
       base.SetToState();

       StopAllCoroutines();
       StartCoroutine(Animate(NetworkSaveData.instance.serverTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody.gameObject.tag == "Player")
        {
            if(rotation > 0 && rotation < maxRotation)
                Activate(collision.rigidbody.gameObject);
        }  
    }

    IEnumerator Animate(double currenServerTime)
    {
        //How far the door have turned, based on server time
        double serverAnimationEstimate = ((currenServerTime - serverTimeStamp) / openTime) * maxRotation;

        //Close Door
        if (state < 151)
        {
            rotation = maxRotation;  //The rotation of the door
            transform.localEulerAngles = new Vector3(0, -maxRotation, 0); //Set the start close state

            rotation -= 150 - state;      //Subtract the rotation based from the lid starting rotation
            transform.Rotate(0, 150 - state, 0); //Rotate based on the lid starting rotation

            rotation -= (float)serverAnimationEstimate; //Calculate the rotation based on serverTime
            rotation = Mathf.Clamp(rotation, 0, maxRotation); //We clamp the rotation for safetyPurposes
            transform.Rotate(Mathf.Clamp((float)serverAnimationEstimate, 0, maxRotation), 0, 0); //Rotate clamped the rotation for safetyPurposes

            while (rotation > 0)
            {
                transform.Rotate(0, 1, 0);
                rotation--;
                yield return new WaitForSeconds(openTime/maxRotation);
            }

            rotation = 0;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        //Open Chest
        else if (state > 150)
        {
            rotation = 0;
            transform.localEulerAngles = new Vector3(0, 0, 0);

            rotation += state-151;
            transform.Rotate(0,-(state - 151), 0);

            rotation += (float)serverAnimationEstimate;
            rotation = Mathf.Clamp(rotation, 0, maxRotation);
            transform.Rotate(-Mathf.Clamp((float)serverAnimationEstimate, 0, maxRotation), 0, 0);

            while (rotation < maxRotation)
            {
                transform.Rotate(0, -1, 0);
                rotation++;
                yield return new WaitForSeconds(openTime / maxRotation);
            }
            rotation = maxRotation;
            transform.localEulerAngles = new Vector3(0, -maxRotation, 0);

        }
    }

}
