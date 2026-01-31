using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public float percentageMaxScale = 0.5f;
    public float speed = 1f;

    public GameObject objectToScale;


    void Start()
    {
        objectToScale = this.gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        float scale = percentageMaxScale + (Mathf.Sin(Time.time * speed) + 1f) / 2f * (1f - percentageMaxScale);
        objectToScale.transform.localScale = new Vector3(scale, scale, scale);
    }
}
