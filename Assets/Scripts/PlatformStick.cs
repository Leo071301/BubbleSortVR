using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformStick : MonoBehaviour
{
    public void AttachPlayer(GameObject player)
    {
        player.transform.SetParent(transform);
    }

    public void DetachPlayer(GameObject player)
    {
        player.transform.SetParent(null);
    }
}
