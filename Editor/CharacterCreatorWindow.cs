using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CharacterCreatorWindow : EditorWindow
{
    private List<Character> characters = new List<Character>();

    [MenuItem("Tools/Character Creator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterCreatorWindow>("Character Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Crear y Gestionar Personajes", EditorStyles.boldLabel);

        // Mostrar la lista de personajes
        for (int i = 0; i < characters.Count; i++)
        {
            GUILayout.BeginVertical("box");

            // Campo de texto para el nombre del personaje
            characters[i].Name = EditorGUILayout.TextField("Nombre del Personaje", characters[i].Name);

            // Menú desplegable para seleccionar la clase
            characters[i].ClassIndex = EditorGUILayout.Popup("Clase", characters[i].ClassIndex, characters[i].ClassOptions);

            // Botón para eliminar el personaje
            if (GUILayout.Button("Eliminar Personaje"))
            {
                characters.RemoveAt(i);
            }

            GUILayout.EndVertical();
        }

        // Espacio en la UI
        GUILayout.Space(10);

        // Botón para agregar un nuevo personaje
        if (GUILayout.Button("Agregar Personaje"))
        {
            characters.Add(new Character());
        }

        // Botón para imprimir los personajes en la consola
        GUILayout.Space(10);
        if (GUILayout.Button("Crear Todos los Personajes"))
        {
            foreach (var character in characters)
            {
                Debug.Log($"Nombre: {character.Name}, Clase: {character.ClassOptions[character.ClassIndex]}");
            }
        }
    }

    // Clase interna para manejar los personajes
    private class Character
    {
        public string Name = "";
        public int ClassIndex = 0;
        public string[] ClassOptions = new string[] { "Mago", "Arquero", "Guerrero" };
    }
}
