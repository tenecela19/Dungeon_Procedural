using UnityEngine;


public class ModuleConnector : MonoBehaviour
{
	#region Singleton
	private static ModuleConnector _instance;

	public static ModuleConnector Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<ModuleConnector>();
			}

			return _instance;
		}
	}


    #endregion
    public string[] Tags;
	public bool IsDefault;
	private bool _isConnected;
	private ModuleConnector _otherConnector;
	public Module Wall;
	public ModuleConnector OtherConnector { get => _otherConnector; set => _otherConnector = value; }
	public bool IsConnected { get => _isConnected; set => _isConnected = value; }
	public void AddNewModuleConnector(ModularWorldGenerator item)
	{
		if(!_isConnected && _otherConnector == null)
        {
			item.PendingExits.Add(this);
		}
	
	}
	void OnDrawGizmos()
	{
		var scale = 1.0f;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * scale);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.right * scale);
		Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.up * scale);

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.125f);
	}

	public void Update()
	{
		if (_otherConnector == null || _otherConnector.gameObject.activeSelf == false)
		{
			_isConnected = false;
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (other.GetComponent<ModuleConnector>())
		{
			if(!ModularWorldGenerator.Instance.DungeonHasEnded)
			_isConnected = true;
			_otherConnector = other.GetComponent<ModuleConnector>();
		}

	}
}
