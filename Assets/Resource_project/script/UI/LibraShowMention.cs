using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraShowMention : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mention;
 

    // Update is called once per frame
    public void ShowMention()
    {
        mention.SetActive(true);
    }
    public void CloseMetion()
    {
        mention.SetActive(false);
    }

}
