using UnityEngine;

public class InteractiveDoor : Interactive
{
    [SerializeField] private string destinationRoom;
    [SerializeField] private string destinationSpawnName;

    // Override the parent Interactive Use funcion
    public override void Use(GameObject player = null)
    {
        player.GetComponent<PlayerSceneManager>().LoadScene(destinationRoom, destinationSpawnName);
    }
}
