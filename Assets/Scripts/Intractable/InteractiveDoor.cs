using System;
using UnityEngine;

public sealed class InteractiveDoor : Interactive
{
    [SerializeField] private string destinationRoom;
    [SerializeField] private string destinationSpawnName;

    // Override the parent Interactive Use funcion
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerSceneManager>().LoadScene(destinationRoom, destinationSpawnName);
    }

 
}
