using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class ModularWorldGenerator : MonoBehaviour
{
	#region Singleton
	private static ModularWorldGenerator _instance;

	public static ModularWorldGenerator Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<ModularWorldGenerator>();
			}

			return _instance;
		}
	}
	#endregion
	//public Variables
	public Module[] Modules;
	public Module StartModule;	
	public int Iterations = 5;
	[Range(0.02f, 1)]
	public float TimeToSpawn;

	//private variables
	private List<ModuleConnector> _newExits = new List<ModuleConnector>();
	private List<ModuleConnector> _pendingExits = new List<ModuleConnector>();
	private Module newModule;
	private bool _dungeonHasEnded;

	//get & set variables
	public List<ModuleConnector> PendingExits { get => _pendingExits; set => _pendingExits = value; }
	public Module NewModule { get => newModule; set => newModule = value; }
    public bool DungeonHasEnded { get => _dungeonHasEnded; set => _dungeonHasEnded = value; }

    IEnumerator Start()
	{
		_dungeonHasEnded = false;
		var startModule = (Module)Instantiate(StartModule, transform.position, transform.rotation);
		_pendingExits = new List<ModuleConnector>(startModule.GetExits());

		for (int iteration = 0; iteration < Iterations; iteration++)
		{
			
			_newExits = new List<ModuleConnector>();

			foreach (var pendingExit in _pendingExits.ToList())
			{
				_pendingExits.RemoveAll(s => s == null);
				if (pendingExit == null)
				{				
					ModuleConnector.Instance.AddNewModuleConnector(this);
				}
				else
                {
                    if (!pendingExit.IsConnected)
                    {
						var newTag = GetRandom(pendingExit.Tags);
						var newModulePrefab = GetRandomWithTag(Modules, newTag);
						newModule = (Module)Instantiate(newModulePrefab);
					}

					if (newModule != null)
					{
						var newModuleExits = newModule.GetExits();
						var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandom(newModuleExits);
						MatchExits(pendingExit, exitToMatch);
						_newExits.AddRange(newModuleExits.Where(e => e != exitToMatch));
						yield return new WaitForSeconds(TimeToSpawn);
					}
					_pendingExits = _newExits;
					_pendingExits = _newExits;
				}
			

			}
		}
		
		foreach (var item in FindObjectsOfType<ModuleConnector>())
		{
			if (!item.IsConnected && item.OtherConnector == null)
			{
				newModule = (Module)Instantiate(item.Wall);
				var newModuleExits = newModule.GetExits();
				var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandom(newModuleExits);
				MatchExits(item, exitToMatch);
			}
		}
		yield return new WaitForSeconds(1f);
		_dungeonHasEnded = true;
	}



	private void MatchExits(ModuleConnector oldExit, ModuleConnector newExit)
	{
		var newModule = newExit.transform.parent;
		var forwardVectorToMatch = -oldExit.transform.forward;
		var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
		newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
		var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
		newModule.transform.position += correctiveTranslation;
	}


	private static TItem GetRandom<TItem>(TItem[] array)
	{
		return array[Random.Range(0, array.Length)];
	}


	private static Module GetRandomWithTag(IEnumerable<Module> modules, string tagToMatch)
	{
		var matchingModules = modules.Where(m => m.Tags.Contains(tagToMatch)).ToArray();
		return GetRandom(matchingModules);
	}


	private static float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}
}
