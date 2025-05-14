using System.Collections.Generic;
using UnityEngine;

// Este es el único MonoBehaviour que contiene un Update() real.
// Se encarga de llamar al método Tick() de todas las clases registradas.
// Así evitamos tener Update() en muchos scripts.

public class CustomUpdateManager : MonoBehaviour
{
    // Lista de todas las clases que deben recibir el Tick()
    private List<IUpdatable> updatables = new List<IUpdatable>();

    // Unity llama a Update() automáticamente. Nosotros lo usamos para controlar la lógica del juego manualmente.
    void Update()
    {
        foreach (var u in updatables)
        {
            u.Tick(Time.deltaTime); // Llamamos al método Tick de cada clase registrada
        }
    }

    // Método para registrar una clase al sistema de actualización
    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            updatables.Add(updatable);
    }

    // Método para quitar una clase del sistema si ya no necesita actualizarse
    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
    }
}