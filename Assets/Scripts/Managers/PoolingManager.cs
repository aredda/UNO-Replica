using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager 
    : Manager
{
    public Queue<CardTemplate> templates;

    public void SaveTemplate(CardTemplate template)
    {
        // hide
        template.Disable();
        // check if queue is initialised
        if(this.templates == null)
            this.templates = new Queue<CardTemplate>();
        // save
        this.templates.Enqueue(template);
    }

    public CardTemplate GetTemplate(Transform parent)
    {
        // check if pool is initialised
        if(templates == null)
            this.templates = new Queue<CardTemplate>();
        // if there's nothing on the pool, instantiate a new object
        if(templates.Count == 0)
            return Instantiate(director.prefabManager.cardTemplate, parent);
        // retrieve template
        CardTemplate template = templates.Dequeue();
        // change the container of the template
        template.transform.SetParent(parent);
        template.transform.localPosition = Vector3.zero;
        return template;
    }
}
