using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TypesObjectsDungeon
{
    public enum  ValidConnectiontypes
    {
        HallWay = 0,
        Room = 1,
        other =2
    }
    public ValidConnectiontypes [] connectiontypes;
}

public class ChamberConnectionPoint : MonoBehaviour
{
    public TypesObjectsDungeon TypesDungeonParts = new TypesObjectsDungeon();
    public float Rotation = 90;
    public bool IsConnnected;
    public GameObject HallWay;
    private ChamberConnectionPoint connectionPoint;
    private GameObject objectType;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsConnnected)
        {
            switch (TypesDungeonParts.connectiontypes[0])
            {
                case TypesObjectsDungeon.ValidConnectiontypes.HallWay:
                   Instantiate(HallWay, transform.position - new Vector3(2, 0), Quaternion.Euler(0, Rotation, 0));
                    break;
                case TypesObjectsDungeon.ValidConnectiontypes.Room:
                     Instantiate(HallWay, transform.position - new Vector3(5.5f, 0), Quaternion.Euler(0, Rotation, 0));

                    break;
                default:
                    break;
            }
        }
        else return;
    }
    private void Update()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ConnectionPoint"))
        {
            IsConnnected = true;
        }
    }
}
