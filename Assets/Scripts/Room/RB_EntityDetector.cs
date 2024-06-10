using UnityEngine;

public class RB_EntityDetector : MonoBehaviour
{
    //Components
    private RB_Room _room;

    private void Start()
    {
        _room = GetComponentInParent<RB_Room>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health entityHealth))
        {
            if (entityHealth.Team == TEAMS.Ai)//If it the entity is an ai then add it to the enemy list
                _room.AddDetectedEnemy(entityHealth);
            else if (entityHealth.Team == TEAMS.Player && entityHealth.TryGetComponent<RB_PlayerMovement>(out RB_PlayerMovement playerMovement) && RB_RoomManager.Instance.GetPlayerCurrentRoom() == null)//Otherwise if it's a player check the IsPlayerInRoom bool
                _room.IsPlayerInRoom = true;
            else if(entityHealth.Team == TEAMS.Player)
                _room.AddDectedAlly(entityHealth);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health entityHealth))
        {
            if(entityHealth.Team == TEAMS.Player && (entityHealth.TryGetComponent<RB_PlayerController>(out RB_PlayerController playerController)))
            {
                //If the player leaves then uncheck the IsPlayerInRoom
                _room.IsPlayerInRoom = false;
            }
            _room.RemoveDetectedEnemy(entityHealth);
            _room.RemoveDectedAlly(entityHealth);
        }
    }
}
