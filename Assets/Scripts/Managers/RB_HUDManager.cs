using UnityEngine;

public class RB_HUDManager : MonoBehaviour  {

    [SerializeField] HUDTYPE _currentHUD;

    private void RefreshHud(){

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        RefreshHud();
    }
}
