using UnityEngine;
using System.Collections;

public class Persistance : MonoBehaviour
{
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }
}
