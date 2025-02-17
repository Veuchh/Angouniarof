using UnityEngine;

public class MultiDisplayManager : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log ("displays connected: " + Display.displays.Length);
    
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}