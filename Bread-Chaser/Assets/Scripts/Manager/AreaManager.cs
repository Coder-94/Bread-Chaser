using UnityEngine;

public class AreaManager
{
    public float        totalLength;
    public void SpawnArea(string name, ref float totalLength, float movedRange = 0, bool isRandom = true, bool isInit = false)
    {
        GameObject go = null;

        if (isRandom == false)
        {
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area1");
        }
        else
        {
            int index = Random.Range(1, 4);
            go = Managers.Resource.Instantiate($"Area/{name}/{name}Area{index}");
        }
        go.transform.position = new Vector3(0, 0, totalLength - movedRange);
        if (isInit == true)
            totalLength += go.GetComponent<Area>().AreaSize;
    }

    public void Clear()
    {
        totalLength = 0;

        GameObject[] areas = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject area in areas)
            if (area != null && area.gameObject != null)
            {
                area.GetComponent<Area>().Clear();
            }
    }

}
