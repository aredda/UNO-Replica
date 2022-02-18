using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager 
    : Manager
{
    public Dictionary<string, Material> materials;

    void Awake()
    {
        this.materials = new Dictionary<string, Material> ();

        foreach(Material material in Resources.LoadAll<Material>("Materials/Cards"))
            materials.Add(material.name, material);
    }

    public Material GetMaterial(string name)
    {
        if(materials.ContainsKey(name))
            return materials[name];

        throw new System.Exception($"MaterialManager.GetMaterial#Exception: A material that goes by [{name}] could not be found");
    }
}
