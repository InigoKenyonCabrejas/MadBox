using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner instance;
     
    void Start() 
    {
        CoroutineRunner.instance = this;
    }
}