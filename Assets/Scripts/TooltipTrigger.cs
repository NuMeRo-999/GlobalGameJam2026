using System.Collections;
using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{

    [SerializeField] private GameObject tooltipText;
    [SerializeField] private float displayDuration = 2f;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Show tooltip
            StartCoroutine(ShowTooltip());
        }
    }

    private IEnumerator ShowTooltip()
    {
        // Display the tooltip
        tooltipText.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the tooltip
        tooltipText.SetActive(false);
    }
}
