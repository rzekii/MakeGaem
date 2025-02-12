using UnityEngine;

public class KeepStatsText : MonoBehaviour
{
    void Awake()
    {
        // Ensure this object is not destroyed or deactivated unintentionally
        DontDestroyOnLoad(gameObject);
    }
}