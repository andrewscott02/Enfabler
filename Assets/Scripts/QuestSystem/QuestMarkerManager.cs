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

    public GameObject AddQuestMarker(GameObject attachGO, Vector3 offset, bool attachAsParent)
    {
        GameObject marker;
        if (attachAsParent)
        {
            marker = Instantiate(questMarkerPrefab, attachGO.transform) as GameObject;
            marker.transform.position += offset;
        }
        else
        {
            marker = Instantiate(questMarkerPrefab, attachGO.transform.position + offset, attachGO.transform.rotation) as GameObject;
        }

        questMarkers.Add(marker);

        return marker;
    }

    public void RemoveQuestMarker(GameObject marker)
    {
        questMarkers.Remove(marker);
        Destroy(marker);
    }
}
