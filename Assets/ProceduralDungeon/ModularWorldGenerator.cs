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
    [Header("Dungeon Modules (Prefabs)")]
    //public Variables
    public Module[] Modules;
    public Module StartModule;
    [Header("Dungeon size")]
    public int Iterations = 5;
    [Header("Dungeon Time Creation")]
    [Range(0.02f, 1)]
    public float TimeToSpawn;
    [Header("Dungeon Parent Transform")]
    public Transform Dungeon;
    //private variables
    private List<ModuleConnector> _newExits = new List<ModuleConnector>();

    private List<ModuleConnector> _pendingExits = new List<ModuleConnector>();

    private Module newModule;

    private Coroutine CurrentCoroutine;

    private bool _dungeonHasEnded;

    //get & set variables
    public List<ModuleConnector> PendingExits { get => _pendingExits; set => _pendingExits = value; }
    public Module NewModule { get => newModule; set => newModule = value; }
    public bool DungeonHasEnded { get => _dungeonHasEnded; set => _dungeonHasEnded = value; }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        _dungeonHasEnded = true;
        GenerateDungeon();
    }
    /// <summary>
    /// Generates the dungeon.
    /// </summary>
    public void GenerateDungeon()
    {
        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
        }
        CurrentCoroutine = StartCoroutine(CorrutineGenerateDungeon());
    }
    /// <summary>
    /// Corrutine to generate the dungeon
    /// </summary>
    /// <returns></returns>
    IEnumerator CorrutineGenerateDungeon()
    {
        //destroy All Object Internal
        foreach (Transform _T in Dungeon)
        {
            Destroy(_T.gameObject);
        }
        _dungeonHasEnded = false;
        var startModule = (Module)Instantiate(StartModule, transform.position, transform.rotation, Dungeon);
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
                        newModule = (Module)Instantiate(newModulePrefab, Dungeon);
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
                }


            }
        }

        foreach (var item in FindObjectsOfType<ModuleConnector>())
        {
            if (!item.IsConnected && item.OtherConnector == null)
            {
                newModule = (Module)Instantiate(item.Wall, Dungeon);
                var newModuleExits = newModule.GetExits();
                var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandom(newModuleExits);
                MatchExits(item, exitToMatch);
            }
        }
        _dungeonHasEnded = true;
        CurrentCoroutine = null;
    }

    /// <summary>
    ///  Match the exits of the new module to the exits of the old module
    /// </summary>
    /// <param name="oldExit"></param>
    /// <param name="newExit"></param>
    private void MatchExits(ModuleConnector oldExit, ModuleConnector newExit)
    {
        var newModule = newExit.transform.parent;
        var forwardVectorToMatch = -oldExit.transform.forward;
        var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
        var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
        newModule.transform.position += correctiveTranslation;
    }

    /// <summary>
    ///  Get a random element from a list
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="TItem"></typeparam>
    /// <returns></returns>
    private static TItem GetRandom<TItem>(TItem[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    /// <summary>
    ///  Get a random element from a list with a tag
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="tagToMatch"></param>
    /// <returns></returns>
    private static Module GetRandomWithTag(IEnumerable<Module> modules, string tagToMatch)
    {
        var matchingModules = modules.Where(m => m.Tags.Contains(tagToMatch)).ToArray();
        return GetRandom(matchingModules);
    }

    /// <summary>
    /// Get the azimuth of a vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }
}




