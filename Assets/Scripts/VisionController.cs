using UnityEngine;

public class VisionController : MonoBehaviour
{
    public bool hasMask = false;
    [SerializeField] private GameObject NormalTilemap;
    [SerializeField] private GameObject HighTilemap;

    void Start()
    {

    }

    void Update()
    {
        if (hasMask)
        {
            NormalTilemap.SetActive(false);
            HighTilemap.SetActive(true);
        }
        else
        {
            NormalTilemap.SetActive(true);
            // HighTilemap.SetActive(false);
        }
    }
}
