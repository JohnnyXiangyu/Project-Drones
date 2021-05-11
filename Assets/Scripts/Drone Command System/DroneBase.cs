using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for drone behavior.
/// This class should handle drone's interaction with the command system.
/// </summary>
public abstract class DroneBase : MonoBehaviour
{
    Command currentCommand;
    
    /// <summary>
    /// Take the drone back to inventory.
    /// </summary>
    public void Retract() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Reinitialize an idle drone with a command.
    /// This message should be the only interface between user controlled system and drone AI.
    /// </summary>
    /// <param name="newCommand">the command to execute</param>
    public virtual void Deploy(Command newCommand) {
        currentCommand = newCommand;
        gameObject.SetActive(true);
    }

    private void Update() {
        switch (currentCommand.type) {
            case Command.CommandType.CHASE: 
                ChaseUpdate();
                break;
            case Command.CommandType.INVADE:
                InvadeUpdate();
                break;
            case Command.CommandType.SECURE:
                SecureUpdate();
                break;
        }
    }

    /// <summary>
    /// The behavior of secure position command.
    /// </summary>
    protected abstract void SecureUpdate();

    /// <summary>
    /// The behavior of chase command.
    /// </summary>
    protected abstract void ChaseUpdate();

    /// <summary>
    /// The behavior of invade command.
    /// </summary>
    protected abstract void InvadeUpdate();
}
