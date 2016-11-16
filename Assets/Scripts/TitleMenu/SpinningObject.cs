using UnityEngine;
public class SpinningObject : MonoBehaviour
{
    [SerializeField][Range(0,20)]    private float speed = 10;
  
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);

	}
}
