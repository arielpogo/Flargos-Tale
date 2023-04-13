using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractableClass : MonoBehaviour { 
    public virtual void Interact() {
        throw new NotImplementedException($"{gameObject.name} does not have Interact() implemented yet.");
    }
}
