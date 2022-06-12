using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    metalOre,
    rockOre
}
public class Nodes : MonoBehaviour
{

    public GameObject resourcePrefab;

    public Transform[] nodes;

    public ResourceType curremtRespurce;
    // Start is called before the first frame update
    void Start()
    {
        SpawnNodes();
        Debug.Log("number " + (1 + 0.5 * (6 - 1)));
    }
    public void SpawnNodes()
    {
        
        NodeManager nodeManager = new NodeManager(nodes);

        curremtRespurce = nodeManager.returnRandomResourceNode;
        for (int i = 0; i < nodes.Length; i++)
        {
            Vector3 nodePos = nodes[nodeManager.randomNodeSelection].transform.position;
            if (curremtRespurce == ResourceType.metalOre || curremtRespurce == ResourceType.rockOre) {
                
                if (!nodeManager.doesResourceExist(nodePos))
                {
                    GameObject nodeSpawned = Instantiate(resourcePrefab);
                    float posZ = (1 + 0.5f * (nodeSpawned.transform.localScale.z - 1) + nodePos.z - 0.5f);
                    nodeSpawned.transform.position = transform.position + nodeSpawned.transform.forward ;

                    nodeSpawned.transform.SetParent(this.transform);

                    nodeManager.nodeDuplicateCheck.Add(nodeSpawned.transform.position , nodePos);
                    
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public class NodeManager {

        private Transform[] nodes;

        public Hashtable nodeDuplicateCheck = new Hashtable();

        public NodeManager (Transform[] node)
        {
            this.nodes = node;
        }
        public int randomNodeSelection
        {
            get { return Random.Range(0, nodes.Length); }
        }
        public int randomResourceSelection
        {
            get { return Random.Range(0, 100) % 50; }
        }

        public bool doesResourceExist(Vector3 pos)
        {
            return nodeDuplicateCheck.ContainsKey(pos);
        }
        public ResourceType returnRandomResourceNode
        {
            get
            {
                if (randomResourceSelection >= 20) {
                    return ResourceType.metalOre;
                }
                else return ResourceType.rockOre;
            }
        }
    }
}
