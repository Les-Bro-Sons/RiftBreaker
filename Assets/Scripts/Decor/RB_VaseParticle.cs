using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(RB_VaseParticle))]
public class Rb_CustomEditorVaseParticle : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RB_VaseParticle vaseParticle = (RB_VaseParticle)target;

        if (GUILayout.Button("Explose"))
        {
            vaseParticle.Explose();
        }

    }
}


#endif


public class RB_VaseParticle : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> _particlesRigidbody;
    [SerializeField] private float _impulseForce;

    private void Start()
    {
        Explose(); // When the particle is instantiated, explode
    }

    /// <summary>
    /// Applies an explosive force to all rigidbodies in the list in random directions.
    /// </summary>
    public void Explose()
    {
        foreach (Rigidbody particleRigidbody in _particlesRigidbody)
        {
            Vector3 forceApplied = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _impulseForce; // Explode randomly
            particleRigidbody.AddForce(forceApplied);
        }
    }
}