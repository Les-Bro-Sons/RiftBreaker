using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RB_Commands
{
    private static RB_RiftBreakerCommandProperties _properties; //The properties of the actual game
    private static RB_PlayerMovement _playerMovement; //The player movement Instance on the player
    private static RB_RoomManager _roomManager; //The room manager Instance
    private static RB_PlayerAction _playerAction; //the player action Instance on the player

    #region commands
    /// <summary>
    /// This function itializes all the instances of the game
    /// </summary>
    public static void InitInstances()
    {
        _properties = RB_RiftBreakerCommandProperties.Instance; //Properties instance
        _playerMovement = RB_PlayerMovement.Instance; //Player movement instance
        _roomManager = RB_RoomManager.Instance; //Room manager instance
        _playerAction = RB_PlayerAction.Instance; //Player action instance
    }

    /// <summary>
    /// This function kills either all the enemies or the player or everything that can move on the current level as the player chose 
    /// </summary>
    /// <param name="args"> The things that the player want to kill (player, enemies, everything..) </param>
    public static void CmdKill(string[] args = null)
    {
        InitInstances(); 
        if (args.Length >= 1)
        {
            RB_Health playerHealth = _playerMovement.GetComponent<RB_Health>(); //Get the script that manage the player life
            switch (args[0])
            {
                case "mobs": //If the script has to kill every enemy on the current level
                    foreach (RB_Health entity in _roomManager.GetAllEntities()) // For each enemy in the level
                    {
                        if (entity != playerHealth)
                            entity.TakeDamage(entity.HpMax); //Kill the enemy
                    }
                    break;
                case "@p": //If the script has to kill the player
                    playerHealth.TakeDamage(playerHealth.HpMax); //Kill the player
                    break;
                case "@e": //If the script has to kill everything
                    foreach (RB_Health entity in _roomManager.GetAllEntities()) // For each enemy in the level
                    {
                        if (entity != playerHealth)
                            entity.TakeDamage(entity.HpMax); //Kill the enemy
                    }
                    playerHealth.TakeDamage(playerHealth.HpMax); //And kill the player
                    break;
                default: //If the player wanted to kill something else return an error message
                    Debug.Log("Commande non reconnue");
                    break;
            }
        }
        else //If no arguments return an error message
        {
            Debug.Log("Commande non reconnue");
        }
    }

    /// <summary>
    /// This function totally resets the level. It means all the enemies return to their initial places, all the broken pots are being repaired etc..
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdResetLevel(string[] args = null)
    {
        InitInstances(); 
        if (!_properties.IsLoadingNewScene)
        {
            _properties.IsLoadingNewScene = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex); //Reload the current scene
        }
    }

    /// <summary>
    /// This function makes the player go to the previous level
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdPreviousLevel(string[] args = null)
    {
        InitInstances();
        if (!_properties.IsLoadingNewScene)
        {
            _properties.IsLoadingNewScene = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1); //Load the previous level
        }
    }

    /// <summary>
    /// This function sets the life of the player as he chose
    /// </summary>
    /// <param name="args"> The HP that the player wants to have </param>
    public static void CmdLife(string[] args = null)
    { 
        InitInstances();
        if (int.TryParse(args[0], out int lifeAmoutInt))
        {
            _properties.LastHp = lifeAmoutInt; //Set the last hp to the current one (for the keep state through scene)
            _playerAction.GetComponent<RB_Health>().Hp = lifeAmoutInt; //Set the HP chose by the player
            _playerAction.GetComponent<RB_Health>().HpMax = lifeAmoutInt;

        }
    }

    /// <summary>
    /// This function makes the player go to the next level
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdNextLevel(string[] args = null)
    {
        InitInstances(); 
        if (!_properties.IsLoadingNewScene)
        {
            _properties.IsLoadingNewScene = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); //Load the next level
        }
    }

    /// <summary>
    /// This function multiplies the damage of the player
    /// </summary>
    /// <param name="args"> The coefficient which will multiply the damages </param>
    public static void CmdDamage(string[] args = null)
    {
        InitInstances(); 
        if (float.TryParse(args[0], out float damageMultiplierFloat)) //Transform the text into float
        {
            _properties.LastDamageMultiplier = damageMultiplierFloat; //Set the last damage multiplier to the current one (for the keep state through scene)
            foreach (RB_Items item in _properties.ItemsOnScene)
            {
                item.AttackDamage *= damageMultiplierFloat; //Multiply the damages of the player by the coefficient chose by the player
                item.ChargedAttackDamage *= damageMultiplierFloat;
                item.SpecialAttackDamage *= damageMultiplierFloat;
            }

        }
    }

    /// <summary>
    /// This function sets the player into god mode (infinite life, infinite damges and no attack cooldown)
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdGodMode(string[] args = null)
    {
        InitInstances();
        _properties.GodModeActivatedFeedbackDebugImage.SetActive(true); //Debug feedback
        _properties.IsGodMode = true;
        _playerAction.GetComponent<RB_Health>().HpMax = float.MaxValue; //Set the player life to infinite
        _playerAction.GetComponent<RB_Health>().Hp = float.MaxValue;
        _properties.LastHp = float.MaxValue; //Set the last hp to the current (for the keep state through scene)
        foreach (RB_Items item in _properties.ItemsOnScene) //Change the damages of every items on the level by infite and set cooldowns to zero
        {
            item.AttackDamage = float.MaxValue;
            item.ChargedAttackDamage *= float.MaxValue;
            item.SpecialAttackDamage *= float.MaxValue;
            item.AttackCooldown(0);
            item.ChargeAttackCooldown(0);
            item.SpecialAttackChargeTime = .1f;
        }

    }

    /// <summary>
    /// This function teleports the player to the beginning of the level
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdBeginning(string[] args = null)
    {
        InitInstances();
        _properties.PlayerRb.position = RB_LevelManager.Instance.BeginningPos; //Set the position of the player to the beginning position
    }

    /// <summary>
    /// This function teleports the player to the weapon of the level and if the plaeyr is in a boss room does nothing
    /// </summary>
    /// <param name="args"> Not used </param>
    public static void CmdWeapon(string[] args = null)
    {
        InitInstances();
        RB_Items foundItem = _properties.FoundItem; //Get the item of the level
        if (foundItem) _properties.PlayerRb.position = foundItem.transform.position; //If there's any weapon on the level, set the player position to its position
    }

    /// <summary>
    /// This function sets the speed of the player
    /// </summary>
    /// <param name="args"> Speed chose by the player </param>
    public static void CmdSpeed(string[] args = null)
    {
        InitInstances();
        if (int.TryParse(args[0], out int speedInt))
        {
            _playerMovement.MovementMaxSpeed = speedInt; //Set the speed of the player to the one chose by him
            _properties.LastSpeed = speedInt; //Set the last speed to the current (for the keep state through scene)
        }
    }

    /// <summary>
    /// This function sets the rewind amount of the player
    /// </summary>
    /// <param name="args"> Amount of rewind chose by the player </param>
    public static void CmdRewind(string[] args = null)
    {
        InitInstances();
        if (int.TryParse(args[0], out int rewindAmountInt))
        {
            _properties.LastRewindAmount = rewindAmountInt; //Set the rewind amount of the player to the one chose by him
            RB_PlayerAction.Instance.RewindLeft = rewindAmountInt; //Set the last rewind amount to the current (for the keep state through scene)
        }
    }

    /// <summary>
    /// This function sets back all the stats of the player to normal
    /// </summary>
    /// <param name="args"> Note used</param>
    public static void CmdNormal(string[] args = null)
    {
        InitInstances();

        _properties.GodModeActivatedFeedbackDebugImage.SetActive(false); //Debug god mode feedback
        _properties.IsGodMode = false; //Remove the god mode state
        _properties.LastHp = _properties.DefaultHp; //Set the last hp to default (for the keep state through scene)
        _playerAction.GetComponent<RB_Health>().HpMax = _properties.DefaultHp; //Set the hp to default 
        _playerAction.GetComponent<RB_Health>().Hp = _properties.DefaultHp;
        _playerMovement.MovementMaxSpeed = _properties.DefaultSpeed; //Set the speed to default
        _playerAction.RewindLeft = _properties.DefaultRewindAmount; //Set the rewind amount to default
        for (int i = 0; i < _properties.ItemsOnScene.Count; i++) //For each item on the scene
        {
            //Set the damages and cooldowns of the weapon back to default
            _properties.ItemsOnScene[i].AttackDamage = _properties.DefaultItemProperties[i].DefaultAttackDamage;
            _properties.ItemsOnScene[i].ChargedAttackDamage = _properties.DefaultItemProperties[i].DefaultChargedAttackDamage;
            _properties.ItemsOnScene[i].SpecialAttackDamage = _properties.DefaultItemProperties[i].DefaultSpecialAttackDamage;
            _properties.ItemsOnScene[i].AttackCooldown(_properties.DefaultItemProperties[i].DefaultAttackCooldown);
            _properties.ItemsOnScene[i].ChargeAttackCooldown(_properties.DefaultItemProperties[i].DefaultAttackCooldown);
            _properties.ItemsOnScene[i].SpecialAttackChargeTime = _properties.DefaultItemProperties[i].DefaultSpecialAttackChargeTime;
        }
    }
    #endregion
}
