using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class AnimateTexture : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Vector2 offset;
    private float xOffset;
    private float yOffset;

    [SerializeField]private float xSpeed = 0.01f;
    [SerializeField]private float ySpeed = 0.01f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        xOffset = 0;
        yOffset = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        xOffset += xSpeed * Time.deltaTime;
        yOffset += ySpeed * Time.deltaTime;
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));

        if (xOffset > 1.0f)
            xOffset -= 1.0f;
        if (yOffset > 1.0f)
            yOffset -= 1.0f;

	}
}
