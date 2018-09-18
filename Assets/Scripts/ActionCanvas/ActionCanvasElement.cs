using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public abstract class ActionCanvasElement
{
    public GameObject element;
    public Image fill { get; set; }
    public Image position { get; set; }
    public Image[] images { get; set; }
    public Text[] texts { get; set; }

    protected Transform transform;

    protected ActionCanvasBase actionCanvas;

    protected bool elementActive;

    protected bool initialOpacitySet = false;

    public virtual void Initialize(ActionCanvasBase actionCanvasBase)
    {
        transform = element.transform;
        actionCanvas = actionCanvasBase;

        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            Transform child = transform.GetChild(childIndex);
            if (child.name == "fill")
            {
                fill = child.GetComponent<Image>();
                if (child.childCount > 0)
                {
                    Transform fillChild = child.GetChild(0);
                    if (fillChild.name == "position")
                        position = fillChild.GetComponent<Image>();
                }

            }
        }

        images = element.GetComponentsInChildren<Image>(true);
        texts = element.GetComponentsInChildren<Text>(true);
    }

    public void SetOpacity(float newOpacity)
    {
        for (int imageIndex = 0; imageIndex < images.Length; imageIndex++)
        {
            Color newColor = images[imageIndex].color;
            newColor.a = newOpacity;
            images[imageIndex].color = newColor;
        }

        for (int textIndex = 0; textIndex < texts.Length; textIndex++)
        {
            Color newColor = texts[textIndex].color;
            newColor.a = newOpacity;
            texts[textIndex].color = newColor;
        }
    }

    public bool IsOpacity(float opacity)
    {
        return (images[0].color.a == opacity);
    }

    protected bool CheckActive(SteamVR_Action_In action, SteamVR_Input_Sources source)
    {
        bool actionActive = action.GetActive(source) && action.actionSet.IsActive();
        
        if (elementActive != actionActive || initialOpacitySet == false)
        {
            float opacity = actionActive ? actionCanvas.activeOpacity : actionCanvas.inactiveOpacity;
            SetOpacity(opacity);

            initialOpacitySet = true;
            elementActive = actionActive;
        }

        return actionActive;
    }

    public void SetFillColor(Color newColor)
    {
        fill.color = newColor;
    }

    public void SetFillAmount(float amount)
    {
        fill.fillAmount = amount;
    }

    protected virtual void SetPosition(Vector2 axis)
    {
        axis.x *= -fill.preferredWidth;
        axis.y *= fill.preferredHeight;
        
        position.rectTransform.localPosition = axis;
    }

    public abstract void Update();
}