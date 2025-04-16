using UnityEngine;

public class AreaManager
{
    [SerializeField]
    private int             _currentStageLV = 1;

    [SerializeField]
    public GameObject[]     _areaPrefabs = new GameObject[5];

    private int             _startSpawnAreaCount = 3;

    [SerializeField]
    private float           _areaDistance;

    public void LoadAreas(string name)
    {
        for(int i=0; i< _areaPrefabs.Length; i++)
        {
            _areaPrefabs[i] = Managers.Resource.Load<GameObject>($"Area/{name}/{name}Area{i+1}");
        }
        
    }

    public void SpawnArea(string name, bool isRandom = true)
    {
        GameObject go = null;

        if(isRandom == false)
        {
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area1");
        }
        else
        {
            int index = Random.Range(1, 6);
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area{index}");
        }
    }
}
