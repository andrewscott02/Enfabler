using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMarkerManager : MonoBehaviour
{
    public static QuestMarkerManager instance;

    public Object questMarkerPrefab;
    List<GameObject> questMarkers;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        questMarkers = new List<GameObject>();
    }

    public GameObject AddQuestMarker(GameObject attachGO, Vector3 offset)
    {
        GameObject marker = Instantiate(questMarkerPrefab, attachGO.transform) as GameObject;
        marker.transform.position += offset;
        questMarkers.Add(marker);

        return marker;
    }

    public void RemoveQuestMarker(GameObject marker)
    {
        questMarkers.Remove(marker);
        Destroy(marker);
    }
}
