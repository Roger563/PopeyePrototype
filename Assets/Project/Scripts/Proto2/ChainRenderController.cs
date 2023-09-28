using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ChainRenderController : MonoBehaviour
{
    private LineRenderer _line;
    [SerializeField] private Transform playerTransform;
    void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawChain();
    }
    
    void DrawChain()
    {
        _line.SetPosition(0,playerTransform.position);
        _line.SetPosition(1,transform.position);
    }
}
