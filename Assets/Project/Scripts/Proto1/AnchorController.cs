using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorController : MonoBehaviour
{
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private LineRenderer line;
    public float maxDistance;
    [HideInInspector] public bool ChainCompletelyTensed = false;
    private PlayerControler _playerControler;

    [SerializeField] Color midTensedColor;

    // Start is called before the first frame update
    void Start()
    {
        line.positionCount = 2;
        line.startColor = Color.green;
        line.endColor = Color.green;
        _playerControler = gameObject.GetComponent<PlayerControler>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawChain();
        CheckIfChainIsTensed();
    }

    void DrawChain()
    {
        line.SetPosition(0,anchorTransform.position);
        line.SetPosition(1,transform.position);
    }

    void CheckIfChainIsTensed()
    {
        float distance = Vector3.Distance(anchorTransform.position, transform.position);
        if ( distance>= maxDistance && !ChainCompletelyTensed)
        {
            ChainCompletelyTensed = true;
            line.startColor = Color.red;
            line.endColor = Color.red;
            _playerControler.ChainTensed = true;
            
            

        }
        else if (distance>= maxDistance-0.6f && !ChainCompletelyTensed)
        {
            line.startColor = midTensedColor;
            line.endColor = midTensedColor;
            _playerControler.ChainTensed = false;
            _playerControler._canDash = true;
        }
        else if (distance < maxDistance)
        {
            ChainCompletelyTensed = false;
            line.startColor = Color.green;
            line.endColor = Color.green;
            _playerControler._canDash = false;

        }
    }
}
