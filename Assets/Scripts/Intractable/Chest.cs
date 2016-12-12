using UnityEngine;
using System.Collections;

//Fully synced over the network
public class Chest : Interactive
{
    private float rotation = 0;
    [SerializeField] float lidMaxRotation = 150;
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
            rotation = lidMaxRotation;
            state = 151;
        }
    }

    protected override void SendServerCommands()
    {
        if (state < 151)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, (int)rotation + 151);
        else if (state > 150)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, (int)rotation);
    }

    protected override void GetState()
    {
        if (!NetworkGameController.instance.HasGameData(objectID))
            return;

       NetworkGameController.instance.GetGameData(objectID, ref state, ref serverTimeStamp);

       StopAllCoroutines();
       StartCoroutine(Animate(NetworkGameController.instance.serverTime));
    }

    IEnumerator Animate(double currenServerTime)
    {
        //How far should the lid have gotten, based on server time
        double serverAnimationEstimate = ((currenServerTime - serverTimeStamp) / openTime) * lidMaxRotation;
   
        //Close Chest
        if (state < 151)
        {
            rotation = lidMaxRotation;  //The rotation of the lid is equal the lidMaxRotation
            transform.localEulerAngles = new Vector3(-lidMaxRotation, 0, 0); //Set the start close state

            rotation -= 150-state;      //Subtract the rotation based from the lid starting rotation
            transform.Rotate(150-state, 0, 0); //Rotate based on the lid starting rotation

            rotation -= (float)serverAnimationEstimate; //Calculate the rotation based on serverTime
            rotation = Mathf.Clamp(rotation, 0, lidMaxRotation); //We clamp the rotation for safetyPurposes
            transform.Rotate(Mathf.Clamp((float)serverAnimationEstimate, 0, lidMaxRotation), 0, 0); //Rotate clamped the rotation for safetyPurposes

            while (rotation > 0)
            {
                transform.Rotate(1, 0, 0);
                rotation--;
                yield return new WaitForSeconds(openTime/lidMaxRotation);
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
            transform.Rotate(-(state - 151), 0, 0);

            rotation += (float)serverAnimationEstimate;
            rotation = Mathf.Clamp(rotation, 0, lidMaxRotation);
            transform.Rotate(-Mathf.Clamp((float)serverAnimationEstimate, 0, lidMaxRotation), 0, 0);

            while (rotation < lidMaxRotation)
            {
                transform.Rotate(-1, 0, 0);
                rotation++;
                yield return new WaitForSeconds(openTime / lidMaxRotation);
            }
            rotation = lidMaxRotation;
            transform.localEulerAngles = new Vector3(-lidMaxRotation, 0, 0);

        }
    }
}
