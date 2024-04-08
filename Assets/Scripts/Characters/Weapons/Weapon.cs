using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponTrail, bloodTrail, unblockableTrail;
    public GameObject weaponBase, weaponTip;
    public GameObject weaponBaseHit, weaponTipHit;

    public Object hitEnvironmentFX;

    public AudioClip attackClip, chargeClip;
    public AudioClip hitClip, blockClip;

    public float soundVolume = 2f, chargeVolume = 3;

    public bool dropOnCharacterDeath = true;

    public Dictionary<MeshRenderer, Material> meshMatDict;
    public Material parryMat;

    Collider col;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        weaponTrail.SetActive(false);
        bloodTrail.SetActive(false);
        unblockableTrail.SetActive(false);

        SetupMaterialDictionary();
    }

    public void Disarm()
    {
        if (!dropOnCharacterDeath) return;

        col.enabled = true;
        rb.isKinematic = false;
        transform.parent = null;
    }

    public void ParryEffect(bool parrying)
    {
        SetupMaterialDictionary();

        foreach (var item in meshMatDict)
        {
            item.Key.material = parrying ? parryMat : item.Value;
        }
    }

    void SetupMaterialDictionary()
    {
        if (meshMatDict != null)
            return;

        meshMatDict = new Dictionary<MeshRenderer, Material>();

        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            meshMatDict.Add(item, item.material);
        }
    }

    public void SpawnDefaultHitFX(Vector3 hitPos)
    {
        Instantiate(hitEnvironmentFX, hitPos, new Quaternion(0, 0, 0, 0));
    }
}
