using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgBase : MonoBehaviour
{
    private List<Renderer> _rendererList = null;

    private List<Material> _materialList = null;

    // Start is called before the first frame update
    void Start()
    {
        _materialList = new List<Material>();
        _rendererList = new List<Renderer>();

        Renderer[] rendererArray = GetComponentsInChildren<Renderer>();
        _rendererList.AddRange(rendererArray);

        for ( int i = 0; i < rendererArray.Length; i++ )
        {
            Material material = new Material(rendererArray[i].sharedMaterial);
            rendererArray[i].material = material;
            _materialList.Add(material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
