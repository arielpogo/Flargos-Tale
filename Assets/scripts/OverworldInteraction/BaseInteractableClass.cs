using System;
using UnityEngine;

/// <summary>
/// Base class for anything that the player walks up to and presses "E" to interact with.
/// </summary>
public class BaseInteractableClass : MonoBehaviour { 

    /// <summary>
    /// When E is pressed, what to do?
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Interact() {
        throw new NotImplementedException($"{gameObject.name} does not have Interact() implemented yet.");
    }
}
