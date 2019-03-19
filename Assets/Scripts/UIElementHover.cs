//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: UIElement that responds to VR hands and generates UnityEvents
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class UIElementHover : MonoBehaviour
	{
		public CustomEvents.UnityEventHand onHandClick;


        protected Hand currentHand;


        float enableTime;
        const float enableDelay = 0.2f; // minimum time after waking up that a button can be pressed. Prevents accidental presses.


        protected virtual void OnEnable()
        {
            enableTime = Time.time;
        }

        //-------------------------------------------------
        protected virtual void Awake()
		{
			Button button = GetComponent<Button>();
			if ( button )
			{
				button.onClick.AddListener( OnButtonClick );
			}
		}


		//-------------------------------------------------
		protected virtual void OnHandHoverBegin( Hand hand )
		{
            if (Time.time - enableTime > enableDelay)
            {
                currentHand = hand;
                //InputModule.instance.HoverBegin( gameObject );
                //ControllerButtonHints.ShowButtonHint( hand, hand.uiInteractAction);
                OnButtonClick();
            }
		}


        //-------------------------------------------------
        protected virtual void OnHandHoverEnd( Hand hand )
		{
			//InputModule.instance.HoverEnd( gameObject );
			//ControllerButtonHints.HideButtonHint( hand, hand.uiInteractAction);
			currentHand = null;
		}


        //-------------------------------------------------
        protected virtual void HandHoverUpdate( Hand hand )
		{
			//if ( hand.uiInteractAction != null && hand.uiInteractAction.GetStateDown(hand.handType) )
			//{
			//	InputModule.instance.Submit( gameObject );
			//	ControllerButtonHints.HideButtonHint( hand, hand.uiInteractAction);
			//}
		}


        //-------------------------------------------------
        protected virtual void OnButtonClick()
		{
			onHandClick.Invoke( currentHand );
		}
	}

#if UNITY_EDITOR
	//-------------------------------------------------------------------------
	[UnityEditor.CustomEditor( typeof( UIElement ) )]
	public class UIElementEditor : UnityEditor.Editor
	{
		//-------------------------------------------------
		// Custom Inspector GUI allows us to click from within the UI
		//-------------------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			UIElement uiElement = (UIElement)target;
			if ( GUILayout.Button( "Click" ) )
			{
				InputModule.instance.Submit( uiElement.gameObject );
			}
		}
	}
#endif
}
