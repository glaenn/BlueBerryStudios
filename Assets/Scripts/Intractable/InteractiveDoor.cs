using UnityEngine;

public class InteractiveDoor : Interactive
{
    [SerializeField] private string destinationRoom;
    [SerializeField] private string destinationSpawnName;

    // Use this for initialization
    public override void Use(GameObject player = null)
    {
        player.GetComponent<PlayerNetworkData>().LoadScene(destinationRoom, destinationSpawnName);
    }
}
