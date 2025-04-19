using UnityEngine;

public class AreaManager
{
    public float        totalLength;
    public GameObject   player;


    public void SpawnArea(string name, ref float totalLength, bool isRandom = true)
    {
        GameObject go = null;

        if (isRandom == false)
        {
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area1");
        }
        else
        {
            int index = Random.Range(1, 6);
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area{index}");
        }
        go.transform.position = new Vector3(0, 0, totalLength);
        totalLength += go.GetComponent<Area>().AreaSize;
    }

}
