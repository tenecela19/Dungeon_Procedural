using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyIntruser : MonoBehaviour
{
    private bool _onCollision;

    public bool OnCollision { get => _onCollision; set => _onCollision = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<DestoyIntruser>() )
        {       
            Destroy(ModularWorldGenerator.Instance.NewModule.gameObject);
        }

    }
}
