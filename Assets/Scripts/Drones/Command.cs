using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public enum CommandType { CHASE, INVADE, SECURE };

    /// <summary>
    /// Mothership of this drone.
    /// </summary>
    public GameObject motherShip;
    
    /// <summary>
    /// Position of the click.
    /// </summary>
    public Vector3 position;
    
    /// <summary>
    /// Type of this command.
    /// Using enumeration here because each drone type will implement its own command behavior.
    /// </summary>
    public CommandType type;
    
    /// <summary>
    /// The input system relies on clicking colliders, thus pass the clicked object in as part of the command.
    /// </summary>
    public GameObject clickedObj;
}
