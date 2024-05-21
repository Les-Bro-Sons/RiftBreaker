using BehaviorTree;
using UnityEngine;

public class RB_AI_PlayerInFov : RB_BTNode
{
    private Transform _transform;
    private static int _layerMask = 1 << 7;
    //private Animator _animator;

    public RB_AI_PlayerInFov(Transform transform)
    {
        _transform = transform;
        //Debug.Log("PlayerInRange" + _transform);
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Debug.Log("Recherche de la cible...");
            Collider[] colliders = Physics.OverlapSphere(_transform.position, RB_AIInf_BTTree.FovRange, _layerMask);
            if (colliders.Length > 0)
            {
                Debug.Log("Cible trouvée, assignation en cours...");
                Parent.Parent.SetData("target", colliders[0].transform);
                //t = colliders[0].transform;
            }
            else
                Debug.Log("Aucune cible trouvée dans la portée.");
        }

        Transform target = (Transform)t;

        if (target == null)
        {
            Debug.Log("target est toujours null après la recherche.");
            _state = BTNodeState.FAILURE;
            return _state;
        }
        else
            Debug.Log("Cible actuelle: " + t.GetType().Name.ToString());


        if (SeesPlayer(target))
        {
            // _animator.SetBool("Attacking", true);
            // _animator.SetBool("Walking", false);

            _state = BTNodeState.SUCCESS;
            return _state;
        }

        _state = BTNodeState.FAILURE;
        return _state;
    }

    bool SeesPlayer(Transform target)
    {
        Vector3 targetDir = target.position - _transform.position;
        float angle = Vector3.Angle(targetDir, _transform.forward);
        if (angle >= -RB_AIInf_BTTree.FovAngle / 2 && angle <= RB_AIInf_BTTree.FovAngle / 2)
        {
            RaycastHit hit;

            Debug.DrawLine(_transform.position, _transform.position + targetDir.normalized * RB_AIInf_BTTree.FovRange, Color.red);
            if (Physics.Raycast(_transform.position, targetDir, out hit, RB_AIInf_BTTree.FovRange, _layerMask))
            {
                if (hit.transform == target.parent)
                {
                    // Dessine un rayon vert si le joueur est détecté.
                    Debug.DrawLine(_transform.position, hit.point, Color.green);
                    return true;
                }
                else
                {
                    // Dessine un rayon jaune si un autre objet est touché.
                    Debug.DrawLine(_transform.position, hit.point, Color.yellow);
                }
            }
        }
        return false;
    }
}