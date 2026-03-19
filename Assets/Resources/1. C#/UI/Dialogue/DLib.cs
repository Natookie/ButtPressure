using UnityEngine;

public static class DLib
{
    public struct Character
    {
        public readonly string Name;
        public readonly Color Color;
        
        public Character(string name, Color color){
            Name = name;
            Color = color;
        }
    }
    
    public static readonly Character PLAYER = new Character("Udin", new Color32(73, 243, 245, 255));
    public static readonly Character STUDENT = new Character("Student", new Color32(73, 243, 245, 255));

    public static readonly Character KANA = new Character("Kana", new Color32(255, 166, 166, 255));

    public static readonly Character TEACHER = new Character("Teacher", new Color32(73, 243, 117, 255));
    public static readonly Character NARRATOR = new Character("Narrator", new Color32(200, 200, 200, 255));
    public static readonly Character CROWD = new Character("Crowd", new Color32(200, 200, 200, 255));
}