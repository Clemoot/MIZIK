using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GamePlaneHole"))
            GameManager.Instance.EndGame();
    }
}
