using System.Collections;
using UnityEngine;

public class RB_EntityDetector : MonoBehaviour
{
    //Components
    private RB_Room _room;

    private bool _triggerStayActivated = false;

    private void Start()
    {
        _room = GetComponentInParent<RB_Room>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health entityHealth))
        {
            if (entityHealth.Team == TEAMS.Ai)//If it the entity is an ai then add it to the enemy list
                _room.AddDetectedEnemy(entityHealth);
            else if (entityHealth.Team == TEAMS.Player && entityHealth.TryGetComponent<RB_PlayerMovement>(out RB_PlayerMovement playerMovement) && !_room.IsPlayerInRoom)//Otherwise if it's a player check the IsPlayerInRoom bool
            {
                _room.IsPlayerInRoom = true;
                print("player enter room");
            }
            else if(entityHealth.Team == TEAMS.Player)
                _room.AddDectedAlly(entityHealth);
            _room.AddDetectedEntity(entityHealth);
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (_triggerStayActivated && RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health entityHealth))
        {
            if (entityHealth.Team == TEAMS.Ai)//If it the entity is an ai then add it to the enemy list
                _room.AddDetectedEnemy(entityHealth);
            else if (entityHealth.Team == TEAMS.Player && entityHealth.TryGetComponent<RB_PlayerMovement>(out RB_PlayerMovement playerMovement))//Otherwise if it's a player check the IsPlayerInRoom bool
            {
                _room.IsPlayerInRoom = true;
                print("player in room");
            }
            else if (entityHealth.Team == TEAMS.Player)
                _room.AddDectedAlly(entityHealth);
            _room.AddDetectedEntity(entityHealth);
        }
    }

    private void EnableTriggerStay()
    {
        _triggerStayActivated = true;
    }
    private IEnumerator DisableTriggerStay()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        _triggerStayActivated = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health entityHealth))
        {
            if(entityHealth.Team == TEAMS.Player && (entityHealth.TryGetComponent<RB_PlayerController>(out RB_PlayerController playerController)) && _room.IsPlayerInRoom)
            {
                //If the player leaves then uncheck the IsPlayerInRoom
                print("player exit");
                _room.IsPlayerInRoom = false;
            }
            _room.RemoveDetectedEnemy(entityHealth);
            _room.RemoveDectedAlly(entityHealth);
            _room.RemoveDetectedEntity(entityHealth);
            EnableTriggerStay();
            StartCoroutine(DisableTriggerStay());
        }
    }
}
